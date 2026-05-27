namespace RealEstateBooking.Domain.Entities;

public class Review : Audit
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;

    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    public int ReviewerId { get; set; }
    public User Reviewer { get; set; } = null!;
}
