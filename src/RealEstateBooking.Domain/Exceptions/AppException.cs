namespace RealEstateBooking.Domain.Exceptions;

public class AppException(string message, int statusCode = 500) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}