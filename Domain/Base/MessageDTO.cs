using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base
{
    public class MessageDTO
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }

        public bool IsFailure => !IsSuccess;

        private MessageDTO(bool isSuccess, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static MessageDTO Success() => new(true, null);
        public static MessageDTO Failure(string errorMessage) => new(false, errorMessage);
    }
}
