using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class AreaService
{
    private readonly AppDatabase _db;
    public AreaService(AppDatabase db) => _db = db;

    public List<Area> GetAll() => _db.Connection.Table<Area>().ToList();

    public void Save(Area area)
    {
        if (area.Id == 0) _db.Connection.Insert(area);
        else _db.Connection.Update(area);
    }

    public void Delete(Area area) => _db.Connection.Delete(area);
}