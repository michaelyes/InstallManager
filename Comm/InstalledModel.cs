namespace STCT.DBInstall
{
    public class InstalledModel
    {
        public string displayName { get; set; }
        public string DisplayVersion { get; set; }
        public string installLocation { get; set; }
        public string uninstallString { get; set; }
        public string releaseType { get; set; }
        public string InstallDate { get; set; }

        public string AppDir { get; set; }

        public string ProductCode { get; set; }
    }
}
