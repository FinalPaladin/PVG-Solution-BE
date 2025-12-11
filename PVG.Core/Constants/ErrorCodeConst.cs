namespace PVG.Domain.Constants
{
    public class ErrorCodeConst
    {
        public const string ERROR_SYS_ERR = "ERR:00001";
        public const string ERROR_REQUEST_NOT_FOUND = "ERR:00002";
        public const string ERROR_UPLOAD_IMAGE_FAIL = "ERR:00003";
        public const string ERROR_INPUT_INVALID = "ERR:00004";

        public const string ERROR_LOGIN_INVALID_INPUT = "ERR:10001";
        public const string ERROR_LOGIN_USER_NOT_FOUND = "ERR:10002";
        public const string ERROR_LOGIN_USER_INACTIVE = "ERR:10003";
        public const string ERROR_LOGIN_PASSWORD_WRONG = "ERR:10004";
        public const string ERROR_REGISTER_ACCOUNT_EXISTED = "ERR:10005";
        public const string ERROR_SESSION_NOT_FOUND = "ERR:10006";
    }
}