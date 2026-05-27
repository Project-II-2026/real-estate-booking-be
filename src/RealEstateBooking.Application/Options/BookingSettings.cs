using System.ComponentModel.DataAnnotations;

namespace RealEstateBooking.Application.Options;

public class BookingSettings
{
    [Range(1, 1440)]
    public int SlotDurationMinutes { get; set; } = 30;

    [Range(0, 720)]
    public int CancellationCutoffHours { get; set; } = 12;
}
