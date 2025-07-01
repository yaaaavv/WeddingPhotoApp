using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WeddingPhotoApp.Data
{
    public static class SeedData

    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Photos.Any())
            {
                // 既にあれば処理終了（必要に応じて削除して再登録も可）
                return;
            }

            var photoDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photo");
            Console.WriteLine("今見ているパス: " + photoDir);
            if (!Directory.Exists(photoDir))
            {
                Console.WriteLine("写真フォルダが存在しません: " + photoDir);
                return;
            }

            var photoFiles = Directory.GetFiles(photoDir)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".png"))
                .Select(Path.GetFileName);

            foreach (var file in photoFiles)
            {
                context.Photos.Add(new Photo
                {
                    FileName = file!,
                    UploadDate = DateTime.Now
                });
            }

            context.SaveChanges();
            Console.WriteLine("Seed completed: 登録件数 = " + context.Photos.Count());
        }
    }

}

