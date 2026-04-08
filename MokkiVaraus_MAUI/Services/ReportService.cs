using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.Services;

public sealed class ReportService
{
    private readonly AppDatabase _database;

    public ReportService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<BookingReportRow>> GetAccommodationReportAsync(DateTime from, DateTime to, int? areaId = null)
    {
        var areas = await _database.GetAreasAsync();
        var cottages = await _database.GetCottagesAsync();
        var customers = await _database.GetCustomersAsync();
        var bookings = await _database.GetBookingsAsync();

        if (areaId.HasValue)
            cottages = cottages.Where(c => c.AreaId == areaId.Value).ToList();

        var result = bookings
            .Where(b => b.StartDate <= to && b.EndDate >= from)
            .Join(cottages, b => b.CottageId, c => c.Id, (b, c) => new { b, c })
            .Join(areas, bc => bc.c.AreaId, a => a.Id, (bc, a) => new { bc.b, bc.c, a })
            .Join(customers, bca => bca.b.CustomerId, cu => cu.Id, (bca, cu) => new BookingReportRow
            {
                BookingId = bca.b.Id,
                AreaName = bca.a.Name,
                CottageName = bca.c.Name,
                CustomerName = cu.FullName,
                StartDate = bca.b.StartDate,
                EndDate = bca.b.EndDate,
                TotalAmount = Math.Max(1, (bca.b.EndDate - bca.b.StartDate).Days) * bca.c.NightlyPrice,
                Status = bca.b.Status.ToString()
            })
            .OrderByDescending(x => x.StartDate)
            .ToList();

        return result;
    }

    public async Task<List<ServiceReportRow>> GetServiceReportAsync(DateTime from, DateTime to, int? areaId = null)
    {
        var areas = await _database.GetAreasAsync();
        var cottages = await _database.GetCottagesAsync();
        var bookings = await _database.GetBookingsAsync();
        var rows = await _database.GetBookingExtraServicesAsync();
        var services = await _database.GetExtraServicesAsync();

        var bookingLookup = bookings
            .Where(b => b.StartDate <= to && b.EndDate >= from)
            .ToDictionary(b => b.Id);

        var cottageLookup = cottages.ToDictionary(c => c.Id);
        var areaLookup = areas.ToDictionary(a => a.Id);
        var serviceLookup = services.ToDictionary(s => s.Id);

        var result = new List<ServiceReportRow>();

        foreach (var row in rows)
        {
            if (!bookingLookup.TryGetValue(row.BookingId, out var booking))
                continue;

            if (!cottageLookup.TryGetValue(booking.CottageId, out var cottage))
                continue;

            if (areaId.HasValue && cottage.AreaId != areaId.Value)
                continue;

            if (!areaLookup.TryGetValue(cottage.AreaId, out var area))
                continue;

            if (!serviceLookup.TryGetValue(row.ExtraServiceId, out var service))
                continue;

            result.Add(new ServiceReportRow
            {
                AreaName = area.Name,
                ServiceName = service.Name,
                Quantity = row.Quantity,
                Revenue = row.Quantity * row.UnitPrice,
                BookingDate = booking.StartDate
            });
        }

        return result
            .OrderByDescending(x => x.BookingDate)
            .ThenBy(x => x.AreaName)
            .ThenBy(x => x.ServiceName)
            .ToList();
    }
}
