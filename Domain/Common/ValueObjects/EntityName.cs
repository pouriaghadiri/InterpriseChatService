using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class EntityName : ValueObject
    { 
        public string Value { get; private set; }
        public EntityName()
        {
            
        }
        private EntityName(string value)
        {
            Value = value;
        }
        public static ResultDTO<EntityName> Create(string value)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(value))
                errors.Add("Name content is required.");
            else if (value.Length > 100)
                errors.Add("Name Content is too long");
            if (errors.Count > 0)
                return ResultDTO<EntityName>.Failure("Invalid Name", errors, "Please fix the name input.");
            return ResultDTO<EntityName>.Success("Valid Name", new EntityName(value), "Name created successfully");
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
