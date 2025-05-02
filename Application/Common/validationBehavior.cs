using FluentValidation;
using MediatR;
using Domain.Base;

namespace Application.Common
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
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
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Any())
                {
                    var errors = failures.Select(f => f.ErrorMessage).ToList();

                    // اگر TResponse از نوع MessageDTO باشد
                    if (typeof(TResponse) == typeof(MessageDTO))
                    {
                        var result = MessageDTO.Failure("Validation Failed", errors, "There was an error in your input.");
                        return (TResponse)(object)result!;
                    }

                    // اگر TResponse از نوع ResultDTO<T> باشد
                    var responseType = typeof(TResponse);
                    if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(ResultDTO<>))
                    {
                        var genericArg = responseType.GetGenericArguments()[0];
                        var method = typeof(ResultDTO<>)
                            .MakeGenericType(genericArg)
                            .GetMethod(nameof(ResultDTO<object>.Failure));

                        var result = method!.Invoke(null, new object[] { "Validation Failed", errors, "There was an error in your input." });
                        return (TResponse)result!;
                    }

                    // در غیر این صورت throw exception
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}
