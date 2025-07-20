using Domain.Common.ValueObjects;

namespace Application.Features.UserUseCase.DTOs
{
    public class UserRoleInDepartmentDTO
    {
        public Guid DepartmentID { get; set; }
        public EntityName DepartmentName { get; set; }
        public Guid RoleID { get; set; }
        public EntityName RoleName { get; set; }
    }
}