namespace PVG.Domain.Settings
{
    public class AppSettings
    {
        public CloudflareSetting CloudflareR2 { get; set; }
        public RecaptchaSetting Recaptcha { get; set; }
    }

    public class CloudflareSetting
    {
        public string AccountId { get; set; }
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName { get; set; }
        public string S3APIUrl { get; set; }
        public string PublicBaseUrl { get; set; }
    }

    public class RecaptchaSetting
    {
        public string Key { get; set; }
        public string Url { get; set; }
    }
}