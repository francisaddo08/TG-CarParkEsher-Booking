namespace TG.CarParkEsher.Booking
{
    public sealed class ContextResult<T> where T : class
    {
        private ContextResult(T? result, string? error, bool isSuccess, bool isFailure, bool isServerError)
        {
            Result = result;
            Error = error;
            IsSuccess = isSuccess;
            IsFailure = isFailure;
            IsServerError = isServerError;
        }
        public static ContextResult<T> Success(T result)
        {
            return new(result, null, true, false, false);
        }

        public static ContextResult<T> Failure(string error)
        {
            return new(null, error, false, true, false);
        }
        public static ContextResult<T> Failure(T result, bool isServerError = false)
        {
            return new(result, string.Empty, isServerError, true, false);
        }
        public static ContextResult<T> Failure(string error, bool isServerError)
        {
            return new(null, error, false, true, isServerError);
        }
        public T? Result { get; }
        public string? Error { get; }
        public bool IsServerError { get; }
        public bool IsFailure { get; }
        public bool IsSuccess { get; }
    }
}
