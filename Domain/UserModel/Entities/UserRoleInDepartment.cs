using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    class UserRoleInDepartment:Entity
    { 
        public int UserRoleId { get; set; }
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual UserRole UserRole{ get; set; }
    }
}
