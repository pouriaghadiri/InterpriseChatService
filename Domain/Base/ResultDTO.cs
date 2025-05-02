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
        public string Title { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; private set; }
        public ResultDTO(bool isSuccess, T? data,string title, List<string>? errors, string? message)
        {
            IsSuccess = isSuccess;
            Data = data;
            Title = title;
            Message = message;
            Errors = errors;
        }

        public static ResultDTO<T> Success(string title, T data, string? message)
        {
            return new(true, data, title, null, message);
        }
        public static ResultDTO<T> Failure(string title, List<string>? errors,  string message)
        {
            return new(false, default, title, errors, message);
        }
    }
}
