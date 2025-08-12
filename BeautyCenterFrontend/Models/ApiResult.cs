namespace BeautyCenterFrontend.Models
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static ApiResult<T> SuccessResult(T data)
        {
            return new ApiResult<T>
            {
                Success = true,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResult<T> ErrorResult(string errorMessage, int statusCode = 400)
        {
            return new ApiResult<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }

    public class ApiResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static ApiResult SuccessResult()
        {
            return new ApiResult
            {
                Success = true,
                StatusCode = 200
            };
        }

        public static ApiResult ErrorResult(string errorMessage, int statusCode = 400)
        {
            return new ApiResult
            {
                Success = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }
}