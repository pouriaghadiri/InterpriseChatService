using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class PersonFullName:ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public PersonFullName() { }
        private PersonFullName(string firstName, string lastName)
        { 
            FirstName = firstName;
            LastName = lastName;
        }
        public static ResultDTO<PersonFullName> Create(string firstName, string lastName)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add("Firstname is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add("Lastname is required.");
            if (errors.Count > 0)
                return ResultDTO<PersonFullName>.Failure("Invalid Name", errors, "Please fix the name input.");
            return ResultDTO<PersonFullName>.Success("Valid Name", new PersonFullName(firstName, lastName), "Person full name created successfully");
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName.ToLowerInvariant();
            yield return LastName.ToLowerInvariant();
        }
    }
}
