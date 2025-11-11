using Microsoft.AspNetCore.Http;

namespace PVG.Core.BaseModels
{
    public class BaseResponse
    {
        /// <summary>
        /// giá trị true : thành công - false : thất bại
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// mã kết quả trả về
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// thông tin phải hồi
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// đối tượng trả về
        /// </summary>
        public object? Result { get; set; }

        /// <summary>
        /// thông tin lỗi nếu thất bại
        /// </summary>
        public object? Error { get; set; }

        public BaseResponse()
        {
            StatusCode = StatusCodes.Status200OK;
            IsSuccess = true;
        }
    }

    public class BaseResponse<T>
    {
        /// <summary>
        /// giá trị true : thành công - false : thất bại
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// mã kết quả trả về
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// thông tin phải hồi
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// đối tượng trả về
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// thông tin lỗi nếu thất bại
        /// </summary>
        public object? Error { get; set; }

        public BaseResponse()
        {
            StatusCode = StatusCodes.Status200OK;
            IsSuccess = true;
        }
    }

    public class ErrorModel
    {
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}