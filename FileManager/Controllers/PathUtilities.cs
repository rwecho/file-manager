namespace FileManager.Controllers
{
    public class PathUtilities
    {
        private static string? _appData;
        private static string? _uploads;

        public static string AppData => LazyInitializer.EnsureInitialized(ref _appData, InitializeAppData);

        public static string Uploads => LazyInitializer.EnsureInitialized(ref _uploads, InitializeUploads);

        private static string InitializeUploads()
        {
            var directory = Path.Combine(AppData, Consts.Uploads);

            Directory.CreateDirectory(directory);
            return directory;
        }

        private static string InitializeAppData()
        {
            var directory = Path.Combine(Path.GetDirectoryName(typeof(PathUtilities).Assembly.Location)!, Consts.AppData);

            Directory.CreateDirectory(directory);
            return directory;
        }
    }
}