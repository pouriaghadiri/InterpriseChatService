using Application.Features.AuthorizationUseCase.DTOs;
using Domain.Base;
using Domain.Base.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.AuthorizationUseCase.Queries
{
    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, ResultDTO<List<PermissionDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetUserPermissionsQueryHandler(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDTO<List<PermissionDTO>>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var userExist = await _unitOfWork.Users.ExistsAsync(x => x.Id == request.userId, cancellationToken);
            if (!userExist)
            {
                return ResultDTO<List<PermissionDTO>>.Failure("Not Exist Error", new List<string> { "The selected user doesn't have exist!" }, "Please select a valid user.");
            }
            var userPermissions = await _unitOfWork.UserPermissions.GetUserPermissionsAsync(request.userId, request.departmentId, cancellationToken);
            if (userPermissions == null || !userPermissions.Any())
            {
                return ResultDTO<List<PermissionDTO>>.Failure("No Permissions", new List<string> { "The user has no assigned permissions." }, "No permissions found for the user.");
            }
            var permissionDTOs = userPermissions.Select(s => s.Permission)
                                                .Select(p => new PermissionDTO
            {
                Id = p.Id,
                Name = p.Name.Value,
                Description = p.Description
            }).ToList();
            return ResultDTO<List<PermissionDTO>>.Success("Permissions Retrieved", permissionDTOs, "User permissions retrieved successfully.");
        }
    }
}
