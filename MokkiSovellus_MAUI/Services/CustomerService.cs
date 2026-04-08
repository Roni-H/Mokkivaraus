using MokkiSovellus_MAUI.Data;
using MokkiSovellus_MAUI.Models;

namespace MokkiSovellus_MAUI.Services;

public class CustomerService
{
    private readonly AppDatabase _db;
    public CustomerService(AppDatabase db) => _db = db;

    public List<Customer> GetAll() => _db.Connection.Table<Customer>().ToList();

    public void Save(Customer customer)
    {
        if (customer.Id == 0) _db.Connection.Insert(customer);
        else _db.Connection.Update(customer);
    }

    public void Delete(Customer customer) => _db.Connection.Delete(customer);
}