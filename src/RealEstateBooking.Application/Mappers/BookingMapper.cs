using Mapster;

using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Domain.Entities;

namespace RealEstateBooking.Application.Mappers;

public class BookingMapper
{
    public static BookingResponseDto FromBookingToBookingResponseDto(Booking booking) =>
        booking.Adapt<BookingResponseDto>();

    public static Booking FromBookingCreateRequestDtoToBooking(BookingCreateRequestDto dto) =>
        dto.Adapt<Booking>();

    public static BookingSlotDto FromBookingToBookingSlotDto(Booking booking) =>
        booking.Adapt<BookingSlotDto>();

    public static void Configure()
    {
        TypeAdapterConfig<BookingCreateRequestDto, Booking>.NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.Version)
            .Ignore(dest => dest.EndTime)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.Visitor)
            .Ignore(dest => dest.VisitorId)
            .Ignore(dest => dest.Property);

        TypeAdapterConfig<Booking, BookingResponseDto>.NewConfig()
            .Map(dest => dest.PropertyTitle, src => src.Property.Title)
            .Map(dest => dest.VisitorUsername, src => src.Visitor.Username);

        TypeAdapterConfig<Booking, BookingSlotDto>.NewConfig()
            .Map(dest => dest.StartTime, src => src.StartTime)
            .Map(dest => dest.EndTime, src => src.EndTime);
    }
}
