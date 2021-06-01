using System;

namespace Business
{
    public static class BusinessResponse
    {
        public static BusinessResponse<T> Ok<T>(T data, string message = default) => 
            new BusinessResponse<T>
            {
                Data = data,
                Message = message,
                ResponseCode = BusinessResponseCodes.Success
            };
        public static BusinessResponse<T> Fail<T>(Enum responseCode, string message = default, T data = default) => 
            new BusinessResponse<T>
            {
                Data = data,
                Message = message,
                ResponseCode = responseCode
            };
        
    }
    public class BusinessResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public Enum ResponseCode { get; set; }
    }
}