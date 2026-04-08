using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class ExtraServiceService
{
    private readonly AppDatabase _db;
    public ExtraServiceService(AppDatabase db) => _db = db;

    public List<ExtraService> GetAll() => _db.Connection.Table<ExtraService>().ToList();

    public void Save(ExtraService item)
    {
        if (item.Id == 0) _db.Connection.Insert(item);
        else _db.Connection.Update(item);
    }

    public void Delete(ExtraService item) => _db.Connection.Delete(item);
}