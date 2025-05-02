using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public class ResultDTO<T>
    {
        public bool IsSuccess { get;private set; }
        public T Data { get; private set; }
        public string ErrorMessage { get; private set; }

        public ResultDTO(bool isSuccess, T data, string errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        public static ResultDTO<T> Success(T data)
        {
            return new(true, data, null);
        }
        public static ResultDTO<T> Failure(string errorMessage)
        {
            return new(false, default, errorMessage);
        }
    }
}
