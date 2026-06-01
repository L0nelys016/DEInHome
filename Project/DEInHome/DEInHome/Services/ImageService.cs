using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DEInHome.Services
{
    public static class ImageService
    {
        private static readonly string _imageFolder = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Images")
            );

        public static async Task<string?> SelectAndSaveImageAsync(Window window, string? oldImage = null)
        {
            IStorageFile? file = await PickupImageAsync(window);

            if (file is null)
                return null;

            Directory.CreateDirectory(_imageFolder);

            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
            var newPath = Path.Combine(_imageFolder, filename);

            await using Stream src = await file.OpenReadAsync();
            await using FileStream dst = File.Create(newPath);

            await src.CopyToAsync(dst);

            DeleteOldImage(oldImage);

            return filename;
        }

        private static async Task<IStorageFile?> PickupImageAsync(Window window)
        {
            IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(
                new FilePickerOpenOptions
                {
                    AllowMultiple = false,
                    FileTypeFilter =
                    [
                        new FilePickerFileType("Image")
                        {
                            Patterns = ["*.jpg", "*.png", "*.jpeg"],
                        },
                    ]
                }
            );

            return files.FirstOrDefault();
        }

        private static void DeleteOldImage(string? oldImage)
        {
            if (string.IsNullOrWhiteSpace(oldImage))
                return;

            var oldPath = Path.Combine(_imageFolder, oldImage);

            if (!File.Exists(oldPath))
                File.Delete(oldPath);
        }
    }
}
