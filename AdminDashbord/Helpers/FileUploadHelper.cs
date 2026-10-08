namespace AdminDashbord.Helpers
{
    /// <summary>
    /// Small shared helper to save an uploaded image into wwwroot/uploads/{folder}
    /// and return only the file name (which is what we store in the database).
    /// </summary>
    public static class FileUploadHelper
    {
        public static async Task<string?> SaveImageAsync(IFormFile? file, IWebHostEnvironment env, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", folder);

            // Create the folder (and any parent folders) if it does not exist yet.
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public static void DeleteImage(string? fileName, IWebHostEnvironment env, string folder)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            var filePath = Path.Combine(env.WebRootPath, "uploads", folder, fileName);
            if (File.Exists(filePath))
            {
                try { File.Delete(filePath); } catch { /* ignore delete failures */ }
            }
        }
    }
}
