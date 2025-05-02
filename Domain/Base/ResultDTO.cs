using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Base
{
    public class ResultDTO<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? SuccessMessage { get; set; }
        public ResultDTO(bool isSuccess, T? data, string? errorMessage, string? successMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            SuccessMessage = successMessage;
        }

        public static ResultDTO<T> Success(T data, string? successMessage)
        {
            return new(true, data, null, successMessage);
        }
        public static ResultDTO<T> Failure(string errorMessage)
        {
            return new(false, default, errorMessage, null);
        }
    }
}
