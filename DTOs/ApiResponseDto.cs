namespace IbgeStats.DTOs
{
    public class ApiResponseDto<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public List<string> Errors { get; set; } = new();

        public static ApiResponseDto<T> SuccessResponse(T data, string message = "")
        {
            return new ApiResponseDto<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponseDto<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponseDto<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}