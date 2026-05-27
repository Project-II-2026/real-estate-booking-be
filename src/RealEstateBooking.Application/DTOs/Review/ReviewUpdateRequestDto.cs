namespace RealEstateBooking.Application.DTOs.Review;

public class ReviewUpdateRequestDto
{
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}
