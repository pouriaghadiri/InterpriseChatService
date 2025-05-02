﻿using Domain.Base;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{ 
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var failures = _validators
                    .Select(v => v.Validate(context))
                    .SelectMany(result => result.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    var errors = failures.Select(x => x.ErrorMessage).ToArray();
                    // فرض می‌گیریم ResultDTO<T> یا MessageDTO داشته باشی:
                    var responseType = typeof(TResponse);

                    if (responseType == typeof(ResultDTO<Guid>))
                    {
                        return (TResponse)(object)ResultDTO<Guid>.Failure(string.Join(" | ", errors));
                    }
                    else if (responseType == typeof(MessageDTO))
                    {
                        return (TResponse)(object)MessageDTO.Failure(string.Join(" | ", errors));
                    }

                    throw new ValidationException(failures); // fallback
                }
            }

            return await next();
        }
    }

}
