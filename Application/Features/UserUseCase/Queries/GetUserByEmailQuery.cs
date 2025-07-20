using Application.Features.UserUseCase.DTOs;
using Domain.Base;
using Domain.Common.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.Queries
{
    public record GetUserByEmailQuery(string Email): IRequest<ResultDTO<UserDTO>>;
}
