using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DEInHome.ViewModels
{
    public partial class OrdersViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<Order> _orders = null!;

        [ObservableProperty]
        private bool _isVisibleAdmin;

        [ObservableProperty]
        private string? _loginedFullName;

        public OrdersViewModel() 
        {
            _ = Init();
        }

        private async Task Init()
        {
            GetUser();
            await LoadOrders();
        }

        [RelayCommand]
        private void NavigateBack()
        {
            MainWindowViewModel.Instance.CurrentViewModel = new ProductsViewModel();
        }

        [RelayCommand]
        private void AddOrder()
        {
            MainWindowViewModel.Instance.CurrentViewModel = new AddAndEditOrdersViewModel(null);
        }

        [RelayCommand]
        private void EditOrder(Order order)
        {
            MainWindowViewModel.Instance.CurrentViewModel = new AddAndEditOrdersViewModel(order);
        }

        [RelayCommand]
        private async Task RemoveOrder(Order order)
        {
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            List<Order> orders = await _db.Orders.Include(o => o.User)
                .Include(o => o.PickupAdress)
                .Include(o => o.OrderStatus)
                .ToListAsync();

            Orders = new ObservableCollection<Order>(orders);
        }

        private void GetUser()
        {
            if (_loginedUser is null)
                return;

            LoginedFullName = _loginedUser.FirstName;

            if (_loginedUser.UserRole.Id == 1)
                IsVisibleAdmin = true;
            else 
                IsVisibleAdmin = false;
        }
    }
}
