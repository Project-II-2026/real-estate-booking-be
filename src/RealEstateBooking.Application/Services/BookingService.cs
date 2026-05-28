using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealEstateBooking.Application.DTOs.Booking;
using RealEstateBooking.Application.DTOs.Common;
using RealEstateBooking.Application.Interfaces.Repositories;
using RealEstateBooking.Application.Interfaces.Services;
using RealEstateBooking.Application.Mappers;
using RealEstateBooking.Application.Options;
using RealEstateBooking.Domain.Enums;
using RealEstateBooking.Domain.Exceptions;

namespace RealEstateBooking.Application.Services;

public class BookingService(
    IBookingRepository bookingRepository,
    IPropertyRepository propertyRepository,
    IUserRepository userRepository,
    IOptions<BookingSettings> bookingOptions,
    ILogger<BookingService> logger
) : IBookingService
{
    private readonly BookingSettings _settings = bookingOptions.Value;

    public async Task<BookingResponseDto> CreateAsync(BookingCreateRequestDto request, int visitorId)
    {
        var property = await propertyRepository.GetByIdAsync(request.PropertyId)
                       ?? throw new NotFoundException($"Property with id {request.PropertyId} was not found.");

        var visitor = await userRepository.GetByIdAsync(visitorId)
                      ?? throw new NotFoundException("User not found.");

        if (property.OwnerId == visitorId)
            throw new ForbiddenException("You cannot book your own property.");

        DateTime startUtc = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);

        if (startUtc <= DateTime.UtcNow)
            throw new BadRequestException("Start time must be in the future.");

        if (!IsAlignedToSlotGrid(startUtc))
            throw new BadRequestException($"Start time must be aligned to a {_settings.SlotDurationMinutes}-minute slot.");

        DateTime endUtc = startUtc.AddMinutes(_settings.SlotDurationMinutes);

        if (await bookingRepository.HasOverlapAsync(property.Id, startUtc, endUtc))
            throw new ConflictException("This time slot is already booked.");

        var booking = BookingMapper.FromBookingCreateRequestDtoToBooking(request);
        booking.StartTime = startUtc;
        booking.EndTime = endUtc;
        booking.Status = BookingStatus.Confirmed;
        booking.VisitorId = visitor.Id;
        booking.Visitor = visitor;
        booking.PropertyId = property.Id;
        booking.Property = property;

        await bookingRepository.AddAsync(booking);

        logger.LogInformation("Booking {BookingId} created by user {VisitorId} on property {PropertyId} at {StartTime}.",
            booking.Id, visitor.Id, property.Id, booking.StartTime);

        return BookingMapper.FromBookingToBookingResponseDto(booking);
    }

    public async Task<BookingResponseDto> GetByIdAsync(int id, int requestingUserId, bool isAdmin = false)
    {
        var booking = await bookingRepository.GetByIdAsync(id)
                      ?? throw new NotFoundException($"Booking with id {id} was not found.");

        if (!isAdmin && booking.VisitorId != requestingUserId && booking.Property.OwnerId != requestingUserId)
            throw new ForbiddenException("You do not have access to this booking.");

        return BookingMapper.FromBookingToBookingResponseDto(booking);
    }

    public async Task<PaginationResponseDto<BookingResponseDto>> GetMineAsync(
        int visitorId, PaginationRequestDto parameters, BookingFilterDto? filter = null)
    {
        var (items, totalCount) = await bookingRepository.GetPagedForVisitorAsync(visitorId, parameters.Page, parameters.PageSize, filter);
        var dtos = items.Select(BookingMapper.FromBookingToBookingResponseDto);
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<PaginationResponseDto<BookingResponseDto>> GetForPropertyAsync(
        int propertyId, int requestingUserId, PaginationRequestDto parameters, BookingFilterDto? filter = null, bool isAdmin = false)
    {
        var property = await propertyRepository.GetByIdAsync(propertyId)
                       ?? throw new NotFoundException($"Property with id {propertyId} was not found.");

        if (!isAdmin && property.OwnerId != requestingUserId)
            throw new ForbiddenException("You are not the owner of this property.");

        var (items, totalCount) = await bookingRepository.GetPagedForPropertyAsync(propertyId, parameters.Page, parameters.PageSize, filter);
        var dtos = items.Select(BookingMapper.FromBookingToBookingResponseDto);
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }

    public async Task<IEnumerable<BookingSlotDto>> GetAvailabilityAsync(int propertyId)
    {
        _ = await propertyRepository.GetByIdAsync(propertyId)
            ?? throw new NotFoundException($"Property with id {propertyId} was not found.");

        var taken = await bookingRepository.GetTakenSlotsAsync(propertyId, DateTime.UtcNow);
        return taken.Select(BookingMapper.FromBookingToBookingSlotDto);
    }

    public async Task CancelAsync(int id, int requestingUserId, bool isAdmin = false)
    {
        var booking = await bookingRepository.GetByIdAsync(id)
                      ?? throw new NotFoundException($"Booking with id {id} was not found.");

        if (!isAdmin && booking.VisitorId != requestingUserId)
            throw new ForbiddenException("You cannot cancel another user's booking.");

        if (booking.Status != BookingStatus.Confirmed)
            throw new BadRequestException("Booking is already cancelled.");

        TimeSpan timeUntilVisit = booking.StartTime - DateTime.UtcNow;
        if (timeUntilVisit < TimeSpan.FromHours(_settings.CancellationCutoffHours))
            throw new BadRequestException($"Bookings can only be cancelled at least {_settings.CancellationCutoffHours} hours before the visit.");

        booking.Status = BookingStatus.Cancelled;
        await bookingRepository.UpdateAsync(booking);

        logger.LogInformation("Booking {BookingId} cancelled by user {VisitorId}.", booking.Id, requestingUserId);
    }

    public async Task<BookingResponseDto> AdminUpdateAsync(int id, BookingAdminUpdateRequestDto request)
    {
        var booking = await bookingRepository.GetByIdAsync(id)
                      ?? throw new NotFoundException($"Booking with id {id} was not found.");

        booking.StartTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);
        booking.EndTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc);
        booking.Status = request.Status;

        await bookingRepository.UpdateAsync(booking);

        logger.LogInformation("Booking {BookingId} modified by admin.", booking.Id);

        return BookingMapper.FromBookingToBookingResponseDto(booking);
    }

    public async Task AdminDeleteAsync(int id)
    {
        var booking = await bookingRepository.GetByIdAsync(id)
                      ?? throw new NotFoundException($"Booking with id {id} was not found.");

        await bookingRepository.DeleteAsync(booking);

        logger.LogInformation("Booking {BookingId} hard-deleted by admin.", id);
    }

    public async Task<PaginationResponseDto<BookingResponseDto>> AdminGetAllAsync(
        PaginationRequestDto parameters, BookingFilterDto? filter = null)
    {
        var (items, totalCount) = await bookingRepository.GetPagedAsync(parameters.Page, parameters.PageSize, filter);
        var dtos = items.Select(BookingMapper.FromBookingToBookingResponseDto);
        return PaginationMapper.FromPagedResultToPaginationResponseDto(dtos, parameters.Page, parameters.PageSize, totalCount);
    }

    private bool IsAlignedToSlotGrid(DateTime startUtc) =>
        startUtc.Second == 0
        && startUtc.Millisecond == 0
        && startUtc.Minute % _settings.SlotDurationMinutes == 0
        && (startUtc.Minute + startUtc.Hour * 60) % _settings.SlotDurationMinutes == 0;
}
