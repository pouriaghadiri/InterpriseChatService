using Domain.Base;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    class Department:Entity
    { 
        public EntityName Name { get; set; }
        public virtual ICollection<UserRoleInDepartment> UserRoleInDepartments { get; set; }

    }
}
