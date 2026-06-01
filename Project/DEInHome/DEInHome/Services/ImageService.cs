using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DEInHome.Services;

public static class ImageService
{
    private static readonly string ImageFolder = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Images"));

    private static readonly FilePickerFileType ImageFileType = new("Image")
    {
        Patterns = ["*.jpg", "*.png", "*.jpeg"]
    };

    public static async Task<string?> SelectAndSaveImageAsync(Window window, string? oldImage = null)
    {
        var file = (await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [ImageFileType]
        })).FirstOrDefault();

        if (file is null) return null;

        Directory.CreateDirectory(ImageFolder);

        var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.Name)}";
        var newPath = Path.Combine(ImageFolder, filename);

        await using var src = await file.OpenReadAsync();
        await using var dst = File.Create(newPath);
        await src.CopyToAsync(dst);

        DeleteOldImage(oldImage);
        return filename;
    }

    private static void DeleteOldImage(string? oldImage)
    {
        if (!string.IsNullOrWhiteSpace(oldImage))
        {
            var oldPath = Path.Combine(ImageFolder, oldImage);
            if (File.Exists(oldPath)) File.Delete(oldPath);
        }
    }
}