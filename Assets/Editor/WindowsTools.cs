
    public class WindowsTools
    {
        public static string UnixPath2WSLPath(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace(@":\", "/");
            path = path.Replace(@":/", "/");
            path = path.Replace(@"\", "/");
            path = $"/mnt/{path}";
#endif
            return path;
        }



        public static string WindowsPath2CommonPath(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace(@"\", "/");
#endif

            return path;
        }



        public static string UnitCommand2WSLCommand(string command)
        {
#if UNITY_EDITOR_WIN
            return $"wsl {command.ToLower()}";
#else
        return command;
#endif

        }
    }
