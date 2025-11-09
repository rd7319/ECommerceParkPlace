using MediatR;

namespace IplEcommerce.Application.Common;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static BaseResponse<T> Success(T data, string? message = null)
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static BaseResponse<T> Failure(string message, List<string>? errors = null)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}