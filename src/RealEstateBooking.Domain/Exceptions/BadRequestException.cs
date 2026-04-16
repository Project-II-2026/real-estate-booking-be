namespace RealEstateBooking.Domain.Exceptions;

public class BadRequestException(string message) : AppException(message, 400);