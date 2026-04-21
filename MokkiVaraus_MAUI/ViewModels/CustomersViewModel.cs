using System.Collections.ObjectModel;
using MokkiVaraus_MAUI.Data;
using MokkiVaraus_MAUI.Helpers;
using MokkiVaraus_MAUI.Models;

namespace MokkiVaraus_MAUI.ViewModels;

public sealed class CustomersViewModel : ObservableObject
{
    private readonly AppDatabase _database;
    private bool _isBusy;
    private Customer? _selectedCustomer;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;
    private string _address = string.Empty;

    public CustomersViewModel(AppDatabase database)
    {
        _database = database;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => SelectedCustomer is not null);
        NewCommand = new RelayCommand(ClearForm);
    }

    public ObservableCollection<Customer> Customers { get; } = new();

    public AsyncRelayCommand RefreshCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }
    public RelayCommand NewCommand { get; }

    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetProperty(ref _selectedCustomer, value))
            {
                LoadSelected();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
    public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string Address { get => _address; set => SetProperty(ref _address, value); }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            Customers.Clear();
            foreach (var customer in await _database.GetCustomersAsync())
                Customers.Add(customer);

            SelectedCustomer ??= Customers.FirstOrDefault();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadSelected()
    {
        if (SelectedCustomer is null)
        {
            ClearForm();
            return;
        }

        FirstName = SelectedCustomer.FirstName;
        LastName = SelectedCustomer.LastName;
        Email = SelectedCustomer.Email ?? string.Empty;
        Phone = SelectedCustomer.PhoneNumber ?? string.Empty;
        Address = SelectedCustomer.Address ?? string.Empty;
    }

    private void ClearForm()
    {
        SelectedCustomer = null;
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        Phone = string.Empty;
        Address = string.Empty;
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
            return;

        var customer = SelectedCustomer ?? new Customer();
        customer.FirstName = FirstName.Trim();
        customer.LastName = LastName.Trim();
        customer.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
        customer.PhoneNumber = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
        customer.Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim();

        await _database.SaveCustomerAsync(customer);
        await LoadAsync();
        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCustomer is null)
            return;

        await _database.DeleteCustomerAsync(SelectedCustomer);
        await LoadAsync();
        ClearForm();
    }
}
