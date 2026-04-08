using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class CottageService
{
    private readonly AppDatabase _db;
    public CottageService(AppDatabase db) => _db = db;

    public List<Cottage> GetAll() => _db.Connection.Table<Cottage>().ToList();

    public void Save(Cottage cottage)
    {
        if (cottage.Id == 0) _db.Connection.Insert(cottage);
        else _db.Connection.Update(cottage);
    }

    public void Delete(Cottage cottage) => _db.Connection.Delete(cottage);
}