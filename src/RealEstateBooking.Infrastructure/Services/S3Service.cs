using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

using Microsoft.Extensions.Options;

using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Options;

namespace RealEstateBooking.Infrastructure.Services;

public class S3Service(IOptions<AwsSettings> options) : IS3Service
{
    private readonly AwsSettings _awsSettings = options.Value;

    public string GeneratePresignedPutUrl(string s3Key, string contentType, int expiryMinutes)
    {
        using IAmazonS3 client = CreateClient();

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = s3Key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        return client.GetPreSignedURL(request);
    }

    public string GetObjectUrl(string s3Key) =>
        IsLocalStack
            ? $"{_awsSettings.ServiceUrl}/{_awsSettings.BucketName}/{s3Key}"
            : $"https://{_awsSettings.BucketName}.s3.{_awsSettings.Region}.amazonaws.com/{s3Key}";

    private bool IsLocalStack => !string.IsNullOrWhiteSpace(_awsSettings.ServiceUrl);

    private AmazonS3Client CreateClient()
    {
        var config = new AmazonS3Config
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_awsSettings.Region)
        };

        if (IsLocalStack)
        {
            config.ServiceURL = _awsSettings.ServiceUrl;
            config.ForcePathStyle = true;
        }

        if (!string.IsNullOrWhiteSpace(_awsSettings.AccessKey) && !string.IsNullOrWhiteSpace(_awsSettings.SecretKey))
        {
            var credentials = new BasicAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey);
            return new AmazonS3Client(credentials, config);
        }

        return new AmazonS3Client(config);
    }
}
