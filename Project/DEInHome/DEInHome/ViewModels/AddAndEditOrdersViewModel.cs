using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using DEInHome.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DEInHome.ViewModels
{
    public partial class AddAndEditOrdersViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Order? _selectedOrder;

        [ObservableProperty]
        private ObservableCollection<User> _users = null!;

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private ObservableCollection<PickupAdress> _pickupAdresses = null!;

        [ObservableProperty]
        private PickupAdress? _selectedPickupAdress;

        [ObservableProperty]
        private ObservableCollection<OrderStatus> _statuses = null!;

        [ObservableProperty]
        private OrderStatus? _selectedStatus;

        [ObservableProperty]
        private bool _isEditMode;

        public AddAndEditOrdersViewModel(Order? order, bool isEditMode = false)
        {
            IsEditMode = isEditMode;
            _ = Init(order);
        }

        private async Task Init(Order? order)
        {
            await LoadAll();
            await LoadSelectedOrder(order);
        }

        private async Task LoadAll()
        {
            List<User> users = await _db.Users.ToListAsync();
            List<PickupAdress> pickupAdresses = await _db.PickupAdresses.ToListAsync();
            List<OrderStatus> orderStatuses = await _db.OrderStatuses.ToListAsync();

            Users = new ObservableCollection<User>(users);
            PickupAdresses = new ObservableCollection<PickupAdress>(pickupAdresses);
            Statuses = new ObservableCollection<OrderStatus>(orderStatuses);
        }

        private async Task LoadSelectedOrder(Order? order)
        {
            if (order is not null)
            {
                SelectedOrder = await _db.Orders
                    .Include(o => o.User)
                    .Include(o => o.PickupAdress)
                    .Include(o => o.OrderStatus)
                    .FirstOrDefaultAsync(p => p.Id == order.Id);

                if (SelectedOrder is not null)
                {
                    SelectedPickupAdress = SelectedOrder.PickupAdress;
                    SelectedUser = SelectedOrder.User;
                    SelectedStatus = SelectedOrder.OrderStatus;
                }
            }
            else
            {
                SelectedOrder = new();
            }
        }

        [RelayCommand]
        private async Task Save()
        {
            if (SelectedOrder is null
                || SelectedUser is null
                || SelectedPickupAdress is null
                || SelectedStatus is null)
                return;

            SelectedOrder.UserId = SelectedUser.Id;
            SelectedOrder.PickupAdressId = SelectedPickupAdress.Id;
            SelectedOrder.OrderStatusId = SelectedStatus.Id;

            if (IsEditMode)
            {
                _db.Orders.Update(SelectedOrder);
                MainWindow.NotificationManager?.Show(new Notification("Успех!", "Заказ изменен", NotificationType.Success));
            }
            else
            {
                await _db.Orders.AddAsync(SelectedOrder);
                MainWindow.NotificationManager?.Show(new Notification("Успех!", "Заказ добавлен", NotificationType.Success));
            }

            await _db.SaveChangesAsync();

            MainWindowViewModel.Instance.CurrentViewModel = new OrdersViewModel();
        }

        [RelayCommand]
        private void NavigateBack()
        {
            MainWindowViewModel.Instance.CurrentViewModel = new OrdersViewModel();
        }
    }
}
