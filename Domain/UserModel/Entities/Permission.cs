using Domain.Base;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : Entity
    {
        public EntityName Name { get; set; }
        public string Description { get; set; }



        public static ResultDTO<Permission> CreatePermission(EntityName name, string description)
        {
            if (name == null || string.IsNullOrEmpty(name.Value))
            {
                return ResultDTO<Permission>.Failure("Empty Error", null, "name of Permission cannot be empty!");
            }
            Permission permission = new Permission()
            {
                Name = name,
                Description = description
            };
            return ResultDTO<Permission>.Success("Created", permission, "New Permission created successfuly.");
        }
    }
}
