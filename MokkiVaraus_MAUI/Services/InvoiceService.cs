using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.Services;

public sealed class InvoiceService
{
    private readonly AppDatabase _database;

    public InvoiceService(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Invoice>> GetOpenInvoicesAsync()
    {
        var invoices = await _database.GetInvoicesAsync();
        return invoices.Where(x => !x.IsPaid).OrderBy(x => x.DueDate).ToList();
    }

    public async Task<List<Invoice>> GetOverdueInvoicesAsync()
    {
        var invoices = await _database.GetInvoicesAsync();
        var today = DateTime.Today;
        return invoices.Where(x => !x.IsPaid && x.DueDate < today).OrderBy(x => x.DueDate).ToList();
    }

    public async Task<Invoice> CreateInvoiceForBookingAsync(Booking booking, decimal amount, InvoiceDeliveryMethod deliveryMethod, string? notes = null)
    {
        var invoice = new Invoice
        {
            BookingId = booking.Id,
            CustomerId = booking.CustomerId,
            InvoiceNumber = $"LASKU-{DateTime.Now:yyyy}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}",
            IssuedAt = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),
            Amount = amount,
            DeliveryMethod = deliveryMethod,
            IsPaid = false,
            Notes = notes
        };

        await _database.SaveInvoiceAsync(invoice);
        return invoice;
    }

    public Task MarkAsPaidAsync(int invoiceId) => _database.MarkInvoiceAsPaidAsync(invoiceId);
}
