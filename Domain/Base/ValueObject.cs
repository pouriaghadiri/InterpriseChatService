using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base
{

    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Aggregate(1, (current, obj) => HashCode.Combine(current, obj));
        }

        public static bool operator ==(ValueObject? a, ValueObject? b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(ValueObject? a, ValueObject? b)
        {
            return !Equals(a, b);
        }
    }
}
