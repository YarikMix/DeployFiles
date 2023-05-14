namespace DeployFiles
{
    public static class Config
    {
        public static string MainDir = @"C:\Program Files (x86)\Steam\steamapps\common\Valheim\";

        public static List<string> LocalFilesPath = new List<string>()
        {
            MainDir + @"BepInEx\",
            MainDir + @"doorstop_libs\",
            MainDir + @"unstripped_corlib\",
            MainDir + "doorstop_config.ini",
            MainDir + "start_game_bepinex.sh",
            MainDir + "start_server_bepinex.sh",
            MainDir + "winhttp.dll"
        };

        public static List<string> LocalExcludeFilesPath = new List<string>()
        {
            MainDir + @"BepInEx\LogOutput.log"
        };


        public static string ServerFilesListUrl = "";

        public static string FTP_Host = "";

        public static string FTP_Username = "";

        public static string FTP_Password = "";

        public static int FTP_Port = 0;




        public static string vkToken = "";

        public static long vkChatId = 2000000000 + 1;


        public static string discordToken = "";

        public static ulong discordChannelId = 0;
    }
}
