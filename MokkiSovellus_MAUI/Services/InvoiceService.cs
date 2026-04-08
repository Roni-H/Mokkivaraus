using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class InvoiceService
{
    private readonly AppDatabase _db;
    public InvoiceService(AppDatabase db) => _db = db;

    public List<Invoice> GetAll() => _db.Connection.Table<Invoice>().ToList();

    public void Save(Invoice invoice)
    {
        if (invoice.Id == 0) _db.Connection.Insert(invoice);
        else _db.Connection.Update(invoice);
    }

    public void Delete(Invoice invoice) => _db.Connection.Delete(invoice);
}