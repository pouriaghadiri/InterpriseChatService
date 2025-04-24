using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.Exceptions
{
    class DomainException: Exception
    {
        public int ErrorCode { get; }

        public DomainException(string message, int errorCode = 0) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
