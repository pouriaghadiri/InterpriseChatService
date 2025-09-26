using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserPermission : Entity
    { 
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public Guid DepartmentId { get; set; }

        public virtual User User { get; set; }
        public virtual Permission Permission{ get; set; }
        public virtual Department Department { get; set; }
    }
}
