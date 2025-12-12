using JOBPORTALWEB.APPLICATION.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName)
        {
            // Tạo thư mục lưu trữ nếu chưa tồn tại (ví dụ: wwwroot/Resumes)
            var folderPath = Path.Combine(_env.WebRootPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fullPath = Path.Combine(folderPath, fileName);

            // Lưu file vào thư mục
            using (var output = new FileStream(fullPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(output);
            }

            // Trả về đường dẫn tương đối (để lưu vào DB)
            return $"/{folderName}/{fileName}";
        }

        public async Task<(Stream Stream, string ContentType)?> GetFileAsync(string filePath)
        {
            // 1. Chuyển đường dẫn tương đối
            var provider = new FileExtensionContentTypeProvider();

            // Loại bỏ ký tự '/' đầu tiên (nếu có)
            var relativePath = filePath.TrimStart('/');
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!File.Exists(fullPath))
            {
                return null;
            }

            // 2. Xác định Content Type
            if (!provider.TryGetContentType(fullPath, out string? contentType))
            {
                contentType = "application/octet-stream";
            }

            // 3. Mở Stream của File
            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            // 4. Trả về Stream và Content Type
            return (fileStream, contentType);
        }
    }
}