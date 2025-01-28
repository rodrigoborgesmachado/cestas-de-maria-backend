namespace CestasDeMaria.Domain.ModelClasses
{
    public class Settings
    {
        public string UserAdmin { get; set; }
        public string AdminPass { get; set; }
        public string SendGridApiKey { get; set; }
        public string EmailCredential { get; set; }
        public string EmailSellerDefault { get; set; }
        public string S3AccessKey { get; set; }
        public string S3SecretKey { get; set; }
        public string S3Region { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string AzureStorageDefaultContainer { get; set; }
        public string AzureStorageBaseLink { get; set; }
        public string AwsS3DefaultBucket { get; set; }
        public string ForceMailTo { get; set; }
        public string PortalUrl { get; set; }
    }
}
