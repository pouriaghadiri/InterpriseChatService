using Domain.Base;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role : Entity
    {
        public EntityName Name { get; set; }
        public string Description { get; set; }



        public static ResultDTO<Role> CreateRole(EntityName name, string description)
        {
            if (name == null || string.IsNullOrEmpty(name.Value))
            {
                return ResultDTO<Role>.Failure("Empty Error", null, "name of role cannot be empty!");
            }
            Role role = new Role()
            {
                Name = name,
                Description = description
            };
            return ResultDTO<Role>.Success("Created", role, "New Role created successfuly.");
        }
    }
}
