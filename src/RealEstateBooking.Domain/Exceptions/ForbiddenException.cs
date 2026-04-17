namespace RealEstateBooking.Domain.Exceptions;

public class ForbiddenException(string message) : AppException(message, 403);