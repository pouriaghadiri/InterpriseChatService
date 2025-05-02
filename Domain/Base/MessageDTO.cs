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
        public string? SuccessMessage { get; set; }
        public bool IsFailure => !IsSuccess;

        private MessageDTO(bool isSuccess, string? errorMessage, string? successMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            SuccessMessage = successMessage;
        }

        public static MessageDTO Success(string successMessage) => new(true, null, successMessage);
        public static MessageDTO Failure(string errorMessage) => new(false, errorMessage, null);
    }
}
