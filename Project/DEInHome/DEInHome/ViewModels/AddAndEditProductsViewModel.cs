using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using DEInHome.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DEInHome.ViewModels
{
    public partial class AddAndEditProductsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private Product? _selectedProduct = null;

        [ObservableProperty]
        private ObservableCollection<ProductCategory> _productCategories = null!;

        [ObservableProperty]
        private ProductCategory? _selectedProductCategory = null;

        [ObservableProperty]
        private ObservableCollection<Manufacture> _manufactures = null!;

        [ObservableProperty]
        private Manufacture? _selectedManufacture = null;

        [ObservableProperty]
        private ObservableCollection<Supplier> _suppliers = null!;

        [ObservableProperty]
        private Supplier? _selectedSupplier = null;

        [ObservableProperty]
        private ObservableCollection<Unit> _units = null!;

        [ObservableProperty]
        private Unit? _selectedUnit = null;

        [ObservableProperty]
        private bool _isEditMode = false;

        public AddAndEditProductsViewModel(Product? product, bool isEditMode = false)
        {
            IsEditMode = isEditMode;
            _ = Init(product);
        }

        private async Task Init(Product? product)
        {
            await LoadCategories();
            await LoadManufactures();
            await LoadSuppliers();
            await LoadUnits();
            await LoadSelectedProduct(product);
        }

        private async Task LoadCategories() =>
            ProductCategories = await Loader.LoadAsync(_db.ProductCategories);

        private async Task LoadManufactures() =>
            Manufactures = await Loader.LoadAsync(_db.Manufactures);

        private async Task LoadSuppliers() =>
            Suppliers = await Loader.LoadAsync(_db.Suppliers);

        private async Task LoadUnits() =>
            Units = await Loader.LoadAsync(_db.Units);

        private async Task LoadSelectedProduct(Product? product)
        {
            if (product is not null)
            {
                SelectedProduct = await _db.Products
               .Include(p => p.Category)
               .Include(p => p.Manufacture)
               .Include(p => p.Supplier)
               .Include(p => p.Unit)
               .FirstOrDefaultAsync(p => p.Id == product.Id);

                if (SelectedProduct != null)
                {
                    SelectedProductCategory = SelectedProduct.Category;
                    SelectedManufacture = SelectedProduct.Manufacture;
                    SelectedSupplier = SelectedProduct.Supplier;
                    SelectedUnit = SelectedProduct.Unit;
                }
            }
            else
            {
                SelectedProduct = new();
            }
        }

        [RelayCommand]
        public async Task Save()
        {
            if (SelectedProduct is null
                || SelectedProductCategory is null
                || SelectedManufacture is null
                || SelectedSupplier is null
                || SelectedUnit is null)
                return;

            SelectedProduct.CategoryId = SelectedProductCategory.Id;
            SelectedProduct.ManufactureId = SelectedManufacture.Id;
            SelectedProduct.SupplierId = SelectedSupplier.Id;
            SelectedProduct.UnitId = SelectedUnit.Id;

            if (IsEditMode)
                _db.Products.Update(SelectedProduct);
            else
                await _db.Products.AddAsync(SelectedProduct);

            await _db.SaveChangesAsync();

            MainWindowViewModel.Instance.CurrentViewModel = new ProductsViewModel();
        }

        [RelayCommand]
        private void NavigateBack()
        {
            MainWindowViewModel.Instance.CurrentViewModel = new AuthViewModel();
        }

        [RelayCommand]
        private async Task SelectedImage()
        {
            Window? window = Avalonia.Application.Current?.ApplicationLifetime
                is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (window is null || SelectedProduct is null)
                return;

            var imageName = await ImageService.SelectAndSaveImageAsync(window, SelectedProduct.Image);

            if (imageName is not null)
            {
                SelectedProduct.Image = imageName;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
    }
}
