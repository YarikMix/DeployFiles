using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace DeployFiles
{
    class VkBot
    {
        private VkApi vk;

        public void Authorize()
        {
            vk = new VkApi();
            vk.Authorize(new ApiAuthParams
            {
               AccessToken = Config.vkToken
            });
        }

        public void SendMessage(string message)
        {
            vk.Messages.Send(new MessagesSendParams
            {
                 PeerId =  + Config.vkChatId,
                 Message = message,
                 RandomId = new Random().Next(-100000000, 100000000)
            });
        }

        public void NewMessage(string mod) => SendMessage("🆕 Добавлен файл: " + mod);

        public void UpdateMessage(string mod) => SendMessage("🔃 Изменён файл: " + mod);

        public void DeleteMessage(string mod) => SendMessage("🗑 Удален файл: " + mod);

        public void DeleteFolderMessage(string mod) => SendMessage("🗑 Удалена папка: " + mod);
    }
}
