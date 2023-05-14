using DSharpPlus;
using DSharpPlus.Entities;

namespace DeployFiles
{
    public class DiscordBot
    {
        private DiscordClient client;

        private DiscordChannel channel;

        async public Task Authorize()
        {
            var config = new DiscordConfiguration()
            {
                Token = Config.discordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            client = new DiscordClient(config);

            channel = await client.GetChannelAsync(Config.discordChannelId);
        }

        async public Task Disconnect() => await client.DisconnectAsync();

        public async Task SendMessage(string message) => await channel.SendMessageAsync(message);

        public async Task NewMessage(string mod) => await SendMessage("🆕 Добавлен файл: " + mod);

        public async Task UpdateMessage(string mod) => await SendMessage("🔃 Изменён файл: " + mod);

        public async Task DeleteMessage(string mod) => await SendMessage("🗑 Удален файл: " + mod);
        
        public async Task DeleteFolderMessage(string mod) => await SendMessage("🗑 Удалена папка: " + mod);

    }
}
