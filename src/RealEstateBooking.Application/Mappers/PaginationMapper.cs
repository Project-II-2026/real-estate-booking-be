using RealEstateBooking.Application.DTOs.Common;

namespace RealEstateBooking.Application.Mappers;

public static class PaginationMapper
{
    public static PaginationResponseDto<TDto> FromPagedResultToPaginationResponseDto<TDto>(
        IEnumerable<TDto> items,
        int page,
        int pageSize,
        int totalCount)
    {
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PaginationResponseDto<TDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }
}
