using Domain.Base;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Department:Entity
    { 
        public EntityName Name { get; set; }
        public virtual ICollection<UserRoleInDepartment> UserRoleInDepartments { get; set; }


        public static ResultDTO<Department> CreateDepartment(EntityName name)
        {
            if (name == null || string.IsNullOrEmpty(name.Value))
            {
                return ResultDTO<Department>.Failure("Empty Error", null, "name of Department cannot be empty!");
            }
            Department department = new Department()
            {
                Name = name
            };
            return ResultDTO<Department>.Success("Created", department, "New Department created successfuly.");
        }
    }
}
