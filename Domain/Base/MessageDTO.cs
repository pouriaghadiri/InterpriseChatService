using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Domain.Base
{
    public class MessageDTO
    {
        public bool IsSuccess { get; }
        public string Title { get; set; }
        public string? Message { get; }
        public List<string>? Errors { get; set; }
        public bool IsFailure => !IsSuccess;

        private MessageDTO(bool isSuccess, string title, List<string>? errors, string? message)
        {
            IsSuccess = isSuccess; 
            Title = title;
            Message = message;
            Errors = errors;
        }

        public static MessageDTO Success(string title, string? message) => new(true, title, null, message);
        public static MessageDTO Failure(string title, List<string>? errors, string message) => new(false, title, errors, message);
    }
}
