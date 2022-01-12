using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileManagerController : ControllerBase
    {
        [HttpGet(Name = "file")]
        public IActionResult GetFileAsync(string path)
        {
            path = NormailizePath(path);
            var file = Path.Combine(PathUtilities.Uploads, path);

            if (!System.IO.File.Exists(file))
            {
                return this.NotFound();
            }
            return this.PhysicalFile(file, "application/octet-stream", true);
        }

        [HttpGet(Name = "files")]
        public IActionResult GetFilesAsync(string? path="")
        {
            path = NormailizePath(path);

            var directory = Path.Combine(PathUtilities.Uploads, path);

            if (!Directory.Exists(directory))
            {
                return this.BadRequest(new
                {
                    Message = "路径不存在"
                });
            }

            var files = Directory.GetFiles(directory);

            var dtos = files
                .Select(o => new FileInfo(o))
                .Select(o => new
                {
                    o.Name,
                    Path = Path.GetRelativePath(PathUtilities.Uploads, o.FullName),
                    o.Length,
                    o.LastWriteTimeUtc,
                    o.CreationTimeUtc,
                    o.LastAccessTimeUtc,
                    o.Extension
                })
                .ToArray();
            return this.Ok(dtos);
        }

        [HttpGet(Name = "foldres")]
        public IActionResult GetFoldersAsync(string? path = "")
        {
            path = NormailizePath(path);

            var directory = Path.Combine(PathUtilities.Uploads, path);

            if (!Directory.Exists(directory))
            {
                return this.BadRequest(new
                {
                    Message = "路径不存在"
                });
            }

            var directories = Directory.GetDirectories(directory);

            var dtos = directories
                .Select(o => new DirectoryInfo(o))
                .Select(o => new
                {
                    o.Name,
                    Path = Path.GetRelativePath(PathUtilities.Uploads, o.FullName),
                    o.CreationTimeUtc,
                    o.LastWriteTimeUtc,
                    o.LastAccessTimeUtc
                })
                .ToArray();
            return this.Ok(dtos);
        }

        [HttpPost(Name = "upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string? path = "")
        {
            path = NormailizePath(path);
            var directory = Path.Combine(PathUtilities.Uploads, path);
            Directory.CreateDirectory(directory);

            try
            {
                var destinationFileName = Path.Combine(directory, file.FileName);
                using var destinationStream = System.IO.File.OpenWrite(destinationFileName);

                using var srouceStream = file.OpenReadStream();
                await srouceStream.CopyToAsync(destinationStream);
                return this.Ok();
            }
            catch (IOException exception)
            {
                return this.BadRequest(new
                {
                    exception.Message
                });
            }
        }

        private static string NormailizePath(string? path)
        {
            if (path == null)
                return "";
            return path.TrimStart('/').TrimStart('\\');
        }
    }
}