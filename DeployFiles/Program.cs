namespace DeployFiles
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        
        private async Task MainAsync()
        {
            var discordBot = new DiscordBot();
            await discordBot.Authorize();


            var vkBot = new VkBot();
            vkBot.Authorize();


            var ftpManager = new FtpManager();
            ftpManager.Authorize(discordBot, vkBot);

            await ftpManager.Start();
        }
    }
}