using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserRoleInDepartment:Entity
    { 
        public Guid UserRoleId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual UserRole UserRole{ get; set; }
    }
}
