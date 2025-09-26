using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RolePermission : Entity
    { 
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual Role Role{ get; set; }
        public virtual Department Department{ get; set; }
    }
}
