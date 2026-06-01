using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DEInHome.Models;
using DEInHome.Services;
using DEInHome.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<ProductType> _productTypes = null!;

        [ObservableProperty]
        private ProductType? _selectedProductType = null;

        [ObservableProperty]
        private bool _isEditMode = false;

        public AddAndEditProductsViewModel(Product? product, bool isEditMode = false)
        {
            IsEditMode = isEditMode;
            _ = Init(product);
        }

        private async Task Init(Product? product)
        {
            await LoadAll();
            await LoadSelectedProduct(product);
        }

        private async Task LoadAll()
        {
            List<Manufacture> manufactures = await _db.Manufactures.AsNoTracking().ToListAsync();
            List<ProductCategory> categories = await _db.ProductCategories.AsNoTracking().ToListAsync();
            List<ProductType> productTypes = await _db.ProductTypes.AsNoTracking().ToListAsync();
            List<Supplier> suppliers = await _db.Suppliers.AsNoTracking().ToListAsync();
            List<Unit> units = await _db.Units.AsNoTracking().ToListAsync();

            Manufactures = new ObservableCollection<Manufacture>( manufactures );
            ProductCategories = new ObservableCollection<ProductCategory>( categories );
            ProductTypes = new ObservableCollection<ProductType>(productTypes);
            Suppliers = new ObservableCollection<Supplier>( suppliers );
            Units = new ObservableCollection<Unit>( units );
        }

        private async Task LoadSelectedProduct(Product? product)
        {
            if (product is not null)
            {
                SelectedProduct = await _db.Products.AsNoTracking()
               .Include(p => p.Category)
               .Include(p => p.Manufacture)
               .Include(p => p.Supplier)
               .Include(p => p.Unit)
               .FirstOrDefaultAsync(p => p.Id == product.Id);

                if (SelectedProduct != null)
                {
                    SelectedProductCategory = ProductCategories.FirstOrDefault(p => p.Id == SelectedProduct.CategoryId);
                    SelectedManufacture = Manufactures.FirstOrDefault(p => p.Id == SelectedProduct.ManufactureId);
                    SelectedSupplier = Suppliers.FirstOrDefault(p => p.Id == SelectedProduct.SupplierId);
                    SelectedUnit = Units.FirstOrDefault(p => p.Id == SelectedProduct.UnitId);
                    SelectedProductType = ProductTypes.FirstOrDefault(p => p.Id == SelectedProduct.ProductTypeId);
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
                || SelectedUnit is null
                || SelectedProductType is null)
                return;

            SelectedProduct.CategoryId = SelectedProductCategory.Id;
            SelectedProduct.ManufactureId = SelectedManufacture.Id;
            SelectedProduct.SupplierId = SelectedSupplier.Id;
            SelectedProduct.UnitId = SelectedUnit.Id;
            SelectedProduct.ProductTypeId = SelectedProductType.Id;

            if (IsEditMode)
            {
                _db.Products.Update(SelectedProduct);
                MainWindow.NotificationManager?.Show(new Notification("Успех!", "Продукт изменен", NotificationType.Success));
            }
            else
            {
                await _db.Products.AddAsync(SelectedProduct);
                MainWindow.NotificationManager?.Show(new Notification("Успех!", "Продукт добавлен", NotificationType.Success));
            }
                

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
            if (Application.Current?.ApplicationLifetime
                is not IClassicDesktopStyleApplicationLifetime { MainWindow: { } window }
                || SelectedProduct is null)
                return;

            if (await ImageService.SelectAndSaveImageAsync(window, SelectedProduct.Image) is { } imageName)
            {
                SelectedProduct.Image = imageName;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
    }
}
