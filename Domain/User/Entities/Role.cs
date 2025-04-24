using Domain.Base;
using Domain.Common.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role: Entity
    { 
        public EntityName Name { get; set; }
        public string Description { get; set; }

    }
}
