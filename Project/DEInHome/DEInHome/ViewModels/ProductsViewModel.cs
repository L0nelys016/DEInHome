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
        [ObservableProperty]
        private ObservableCollection<Product> _products= null!;

        [ObservableProperty]
        private string? _searchText;

        [ObservableProperty]
        private bool _isVisibleAdmin = false;

        [ObservableProperty]
        private bool _isVisibleManager = false;

        [ObservableProperty]
        private string? _userFullName;

        [ObservableProperty]
        private ObservableCollection<string> _sortedParameters = new()
        { 
            "Без сортировки",
            "По возрастанию (Цена)", 
            "По убыванию (Цена)",
            "По возрастанию (Количество)", 
            "По убыванию (Количество)" 
        };

        [ObservableProperty]
        private string? _selectedSortedParameters = "Без сортировки";

        [ObservableProperty]
        private ObservableCollection<string> _filterParametrs = new() { "Все товары", "0-10.99%", "11-14.99%", "15% и более" };

        [ObservableProperty]
        private string? _selectedFilterParameters = "Все товары";

        public ProductsViewModel()
        {
            _ = Init();
        }

        private async Task Init()
        {
            GetUser();
            await LoadProducts();
        }

        partial void OnSelectedFilterParametersChanged(string? value) => _ = Filter();

        partial void OnSelectedSortedParametersChanged(string? value) => _ = Filter();

        partial void OnSearchTextChanged(string? value) => _ = Filter();

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
            MainWindowViewModel.Instance.CurrentViewModel = new AddAndEditProductsViewModel(product, true);
        }

        [RelayCommand]
        private async Task RemoveProduct(Product product)
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            List<Product> products = await _db.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacture)
                .Include(p => p.Supplier)
                .Include(p => p.Unit)
                .Include(p => p.ProductType)
                .ToListAsync();

            Products = new ObservableCollection<Product>(products);
        }

        private async Task Filter()
        {
            List<Product> products = await FilterProduct(SortedProduct(SearchByText(_db.Products))).ToListAsync();
            Products = new ObservableCollection<Product>(products);
        }

        private IQueryable<Product> SearchByText(IQueryable<Product> productQuery)
        {
            if (string.IsNullOrEmpty(SearchText))
                return productQuery;

            string lowerSearchText = SearchText.ToLower();

            return productQuery
                .Where(p => p.Manufacture.Name.ToLower().Contains(lowerSearchText)
                || p.Category.Name.ToLower().Contains(lowerSearchText)
                || p.ProductType.Name.ToLower().Contains(lowerSearchText)
                || p.Supplier.Name.ToLower().Contains(lowerSearchText)
                || p.Unit.Name.ToLower().Contains(lowerSearchText)
                || p.Price.ToString().Contains(lowerSearchText)
                || p.Quantity.ToString().Contains(lowerSearchText)
                || p.Discount.ToString().Contains(lowerSearchText));
        }

        private IQueryable<Product> FilterProduct(IQueryable<Product> productQuery)
        {
            if(string.IsNullOrEmpty(SelectedFilterParameters) || SelectedFilterParameters == "Все товары")
                return productQuery;

            return SelectedFilterParameters switch
            {
                "0-10.99%" => productQuery.Where(p => p.Discount >= 0 && p.Discount <= 11.99),
                "11-14.99%" => productQuery.Where(p => p.Discount >= 11 && p.Discount <= 14.99),
                "15% и более" => productQuery.Where(p => p.Discount >= 15),
                _ => productQuery
            };
        }

        private IQueryable<Product> SortedProduct(IQueryable<Product> productQuery)
        {
            if (string.IsNullOrEmpty(SelectedSortedParameters) || SelectedSortedParameters == "Без сортировки")
                return productQuery;

            if (SelectedSortedParameters == "По возрастанию (Цена)")
                return productQuery.OrderBy(p => p.Price);
            else if (SelectedSortedParameters == "По убыванию (Цена)")
                return productQuery.OrderByDescending(p => p.Price);
            else if (SelectedSortedParameters == "По возрастанию (Количество)")
                return productQuery.OrderBy(p => p.Quantity);
            else
                return productQuery.OrderByDescending(p => p.Quantity);
        }

        private void GetUser()
        {
            if (_loginedUser is null)
            {
                UserFullName = "Гость";
                return;
            }

            UserFullName = _loginedUser.FullName;

            if (_loginedUser.UserRoleId == 1)
            {
                IsVisibleAdmin = true;
                IsVisibleManager = true;
            }
            else if (_loginedUser.UserRoleId == 2)
            {
                IsVisibleAdmin = false;
                IsVisibleManager = true;
            }
        }
    }
}
