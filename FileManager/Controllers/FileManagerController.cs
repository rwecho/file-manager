using Microsoft.AspNetCore.Mvc;

namespace FileManager.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileManagerController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetFileAsync(string path)
        {
            path = NormailizePath(path);
            var file = Path.Combine(PathUtilities.Uploads, path);

            if (!System.IO.File.Exists(file))
            {
                return this.NotFound();
            }

            var fileName = Path.GetFileName(file);
            return this.PhysicalFile(file, "application/octet-stream", fileName, true);
        }


        [HttpGet]
        public IActionResult GetFilesAsync(string? path = "")
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
                    Path = NormailizePath(Path.GetRelativePath(PathUtilities.Uploads, o.FullName)),
                    o.Length,
                    o.LastWriteTimeUtc,
                    o.CreationTimeUtc,
                    o.LastAccessTimeUtc,
                    o.Extension
                })
                .ToArray();
            return this.Ok(dtos);
        }

        [HttpGet]
        public IActionResult GetFolderAsync(string? path = "")
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

            var directoryInfo = new DirectoryInfo(directory);

            if (!IsUploadsSubFolder(directoryInfo.FullName))
            {
                directoryInfo = new DirectoryInfo(PathUtilities.Uploads);
            }
            return this.Ok(new
            {
                directoryInfo.Name,
                Path = NormailizePath(Path.GetRelativePath(PathUtilities.Uploads, directoryInfo.FullName)),
                directoryInfo.CreationTimeUtc,
                directoryInfo.LastWriteTimeUtc,
                directoryInfo.LastAccessTimeUtc
            });
        }

        [HttpGet]
        public IActionResult GetParentFolderAsync(string? path = "")
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

            var parentDirectoryInfo = Directory.GetParent(directory)!;

            if (!IsUploadsSubFolder(parentDirectoryInfo.FullName))
            {
                parentDirectoryInfo = new DirectoryInfo(PathUtilities.Uploads);
            }

            return this.Ok(new
            {
                parentDirectoryInfo.Name,
                Path = NormailizePath(Path.GetRelativePath(PathUtilities.Uploads, parentDirectoryInfo.FullName)),
                parentDirectoryInfo.CreationTimeUtc,
                parentDirectoryInfo.LastWriteTimeUtc,
                parentDirectoryInfo.LastAccessTimeUtc
            });
        }

        private static bool IsUploadsSubFolder(string path)
        {
            var uploadDirectoryInfo = new DirectoryInfo(PathUtilities.Uploads);
            var pathDirectoryInfo = new DirectoryInfo(path);

            return pathDirectoryInfo.FullName.StartsWith(uploadDirectoryInfo.FullName);
        }

        [HttpGet]
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

            if (!IsUploadsSubFolder(directory))
            {
                directory = PathUtilities.Uploads;
            }

            var directories = Directory.GetDirectories(directory);

            var dtos = directories
                .Select(o => new DirectoryInfo(o))
                .Select(o => new
                {
                    o.Name,
                    Path = NormailizePath(Path.GetRelativePath(PathUtilities.Uploads, o.FullName)),
                    o.CreationTimeUtc,
                    o.LastWriteTimeUtc,
                    o.LastAccessTimeUtc
                })
                .ToArray();
            return this.Ok(dtos);
        }

        [HttpPost]
        public IActionResult CreateFolderAsync(string path)
        {
            path = NormailizePath(path);
            var directory = Path.Combine(PathUtilities.Uploads, path);

            if (!IsUploadsSubFolder(directory))
            {
                return this.BadRequest(new
                {
                    Message = "非法路径"
                });
            }

            Directory.CreateDirectory(directory);

            return this.GetFolderAsync(path);
        }

        [HttpPost]
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

                return this.GetFileAsync(destinationFileName);
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
            return path.TrimStart('/').TrimStart('\\').Replace("\\", "/");
        }
    }
}