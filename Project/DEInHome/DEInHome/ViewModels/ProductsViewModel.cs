using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DEInHome.ViewModels
{
    public partial class ProductsViewModel : ViewModelBase
    {
        private List<Product> _allProduct = null!;

        private Supplier _firstSupplier = new Supplier()
        {
            Id = 0,
            Name = "Все поставщики"
        };

        private bool _isAscending;

        [ObservableProperty]
        private ObservableCollection<Product> _products= null!;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = null!;

        [ObservableProperty]
        private Supplier? _selectedSupplier;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private bool _isVisibleAdmin = false;

        [ObservableProperty]
        private bool _isVisibleManager = false;

        [ObservableProperty]
        private string? _userFullName;

        [ObservableProperty]
        private string? _sortedButtonName = "Сортировать";

        public ProductsViewModel()
        {
            _ = Init();
        }

        private async Task Init()
        {
            await GetUser();
            await LoadProducts();
            await LoadSuppliers();
        }

        partial void OnSearchTextChanged(string? value) => _ = ApplyFilter();

        partial void OnSelectedSupplierChanged(Supplier? value) => _ = ApplyFilter();

        [RelayCommand]
        private void NavigateBack()
        {
            _loginedUser = null;
            MainWindowViewModel.Instance.CurrentViewModel = new AuthViewModel();
        }

        [RelayCommand]
        private void AddProduct()
        {
            MainWindowViewModel.Instance.CurrentViewModel = new AddAndEditProductsViewModel(null);
        }

        [RelayCommand]
        private void EditProduct(Product product)
        {
            MainWindowViewModel.Instance.CurrentViewModel = new AddAndEditProductsViewModel(product);
        }

        [RelayCommand]
        private async Task RemoveProduct(Product product)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            await LoadProducts();
        }

        [RelayCommand]
        private void ToggleQuantitySorted()
        {
            _isAscending = !_isAscending;
            SortedButtonName = _isAscending ? "По возвростанию" : "По убыванию";
            ApplySorting();
        }

        private void ApplySorting()
        {
            IEnumerable<Product> sorted = _isAscending
                ? Products.OrderBy(p => p.Quantity)
                : Products.OrderByDescending(p => p.Quantity);

            Products = new ObservableCollection<Product>(sorted);
        }


        private async Task LoadProducts()
        {
            _allProduct = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacture)
                .Include(p => p.Supplier)
                .Include(p => p.Unit)
                .ToListAsync();

            await ApplyFilter();
        }

        private async Task LoadSuppliers()
        {
            List<Supplier> suppliers = await _db.Suppliers.ToListAsync();
            Suppliers = [_firstSupplier, .. suppliers];
            SelectedSupplier = _firstSupplier;
        }

        private async Task ApplyFilter()
        {
            IEnumerable<Product> query = _allProduct.AsEnumerable();

            if (!string.IsNullOrEmpty(SearchText))
            {
                string search = SearchText.ToLower();
                query = query.Where(p =>
                p.Category.Name.ToLower().Contains(search)
                || p.Manufacture.Name.ToLower().Contains(search)
                || p.Supplier.Name.ToLower().Contains(search)
                || p.Unit.Name.ToLower().Contains(search)
                || p.Description!.ToLower().Contains(search)
                || p.Price.ToString().Contains(search)
                || p.Discount.ToString().Contains(search)
                || p.Quantity.ToString().Contains(search));
            }

            if (SelectedSupplier is not null && SelectedSupplier.Id != 0)
                query = query.Where(p => p.SupplierId == SelectedSupplier.Id);

            Products = new ObservableCollection<Product>(query);
        }

        private async Task GetUser()
        {
            if (_loginedUser is null)
            {
                UserFullName = "Гость";
                return;
            }

            User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _loginedUser.Id);

            if (user is null)
                return;

            UserFullName = user.FullName;

            if (user.UserRoleId == 1)
            {
                IsVisibleAdmin = true;
                IsVisibleManager = true;
            }
            else if (user.UserRoleId == 2)
            {
                IsVisibleAdmin = false;
                IsVisibleManager = true;
            }
        }
    }
}
