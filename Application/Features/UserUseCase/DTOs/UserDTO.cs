using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserUseCase.DTOs
{
    public class UserDTO
    {
        public Guid ID { get; set; }
        public PersonFullName FullName{ get; set; }
        public List<UserRoleInDepartmentDTO> UserRoleInDepartments { get; set; }
    }
}
