namespace RealEstateBooking.Application.Interfaces.Services;

public interface IS3Service
{
    string GeneratePresignedPutUrl(string s3Key, string contentType, int expiryMinutes);
    string GetObjectUrl(string s3Key);
}
