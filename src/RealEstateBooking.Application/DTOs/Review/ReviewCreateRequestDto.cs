namespace RealEstateBooking.Application.DTOs.Review;

public class ReviewCreateRequestDto
{
    public int PropertyId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
