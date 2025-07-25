﻿using Domain.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class UserRole: Entity
    { 
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<UserRoleInDepartment> UserRoleInDepartments { get; set; } = new Collection<UserRoleInDepartment>();

    }
}
