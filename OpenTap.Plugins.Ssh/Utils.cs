namespace OpenTap.Plugins.Ssh
{
    public class Utils
    {
        public static string FriendlySize(long size)
        {
            if (size < 1024)
                return $"{size} B";
            if (size < ((1024 * 1024)))
                return $"{(size/1024.0):F1} kB";
            return $"{(size/ (1024 * 1024)):F1} MB";
        }
    }
}
