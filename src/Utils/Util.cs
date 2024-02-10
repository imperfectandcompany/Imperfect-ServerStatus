using System.Text.Json;

namespace IGDiscord.Utils
{
    public static class Util
    {
        public static void PrintLog(string msg)
        {
            Console.WriteLine($"\u001b[37m[IGDiscordPlugin] \u001b[31m{msg}");
        }
    }
}
