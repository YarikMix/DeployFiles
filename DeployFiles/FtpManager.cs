using FluentFTP;

namespace DeployFiles
{
    class FtpManager
    {
        private FtpClient client;
        private DiscordBot discordBot;
        private VkBot vkBot;

        public static List<FileData> local_files = new List<FileData>();
        public static List<FileData> server_files = new List<FileData>();

        public void Authorize(DiscordBot discordBot, VkBot vkBot)
        {
            this.client = new FtpClient(Config.FTP_Host, Config.FTP_Username, Config.FTP_Password, Config.FTP_Port);
            this.discordBot = discordBot;
            this.vkBot = vkBot;
        }

        public async Task Start()
        {
            client.Connect();

            Console.WriteLine("Получаем локальные файлы");
            Config.LocalFilesPath.ForEach(path => GetLocalFiles(path));


            Console.WriteLine("Получаем файлы с сервера");
            GetServerFiles();


            Console.WriteLine("Обновляем файл со списком модов");
            UpdateServerModlist();


            Console.WriteLine("Проверка файлов");
            CheckFiles();


            
            Console.WriteLine("Закрываем соединения");

            client.Disconnect();

            await discordBot.Disconnect();
        }

        public void CheckFiles()
        {
            local_files.ForEach(local_file =>
            {
                var found = false;

                server_files.ForEach(server_file =>
                {
                    if (local_file.GetRemotePath() == server_file.GetRemotePath())
                    {
                        found = true;

                        if (local_file.GetHash() != server_file.GetHash())
                        {
                            UpdateFile(local_file);
                        }
                    }
                });

                if (!found)
                {
                    NewFile(local_file);
                }

            });



            server_files.ForEach(server_file =>
            {
                var found = false;

                local_files.ForEach(local_file =>
                {
                    if (local_file.GetRemotePath() == server_file.GetRemotePath())
                    {
                        found = true;
                    }
                });

                if (!found)
                {
                    DeleteFile(server_file);
                }

            });
        }

        public void UpdateFile(FileData file)
        {
            var status = client.UploadFile(file.GetLocalPath(), file.GetRemotePath(), FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            if (status == FtpStatus.Failed)
            {
                Console.WriteLine("Не удалось обновить файл на сервере: " + file.GetRemotePath());
            }
            else if (status == FtpStatus.Success)
            {
                Console.WriteLine("Обновлен файл на сервере: " + file.GetRemotePath());
                discordBot.UpdateMessage(file.GetRemotePath()).Wait();
                vkBot.UpdateMessage(file.GetRemotePath());
            }
        }

        public void NewFile(FileData file)
        {
            var status = client.UploadFile(file.GetLocalPath(), file.GetRemotePath(), FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            if (status == FtpStatus.Failed)
            {
                Console.WriteLine("Не удалось загрузить файл на сервер: " + file.GetRemotePath());
            }
            else if (status == FtpStatus.Success)
            {
                Console.WriteLine("Файл добавлен на сервер: " + file.GetRemotePath());
                discordBot.NewMessage(file.GetRemotePath()).Wait();
                vkBot.NewMessage(file.GetRemotePath());
            }
        }

        public void DeleteFile(FileData file)
        {
            client.DeleteFile(file.GetRemotePath());
            Console.WriteLine("Файл удален с сервера: " + file.GetRemotePath());
            discordBot.DeleteMessage(file.GetRemotePath()).Wait();
            vkBot.DeleteMessage(file.GetRemotePath());
        }


        public void GetLocalFiles(string path)
        {
            if (Config.LocalExcludeFilesPath.Any(p => p == path))
            {
                return;
            }

            if (File.Exists(path))
            {
                local_files.Add(new FileData(path));
            }
            else if (Directory.Exists(path))
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    local_files.Add(new FileData(f));
                }

                foreach (string d in Directory.GetDirectories(path))
                {
                    GetLocalFiles(d);
                }
            }
        }

        public void GetServerFiles()
        {
            using (HttpClient client = new HttpClient())
            {
                using (Stream stream = client.GetStreamAsync(Config.ServerFilesListUrl).Result)
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string results = sr.ReadToEnd();
                        using (StringReader text = new StringReader(results))
                        {
                            string line;
                            while ((line = text.ReadLine()) != null)
                            {
                                var data = line.Split(';');
                                var relativePath = data[0];
                                var hash = data[1];

                                server_files.Add(new FileData(relativePath, hash));
                            }
                        }
                    }
                }
            }
        }

        public void UpdateServerModlist()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(local_files.Count);
                    writer.Flush();
                    stream.Position = 0;

                    client.UploadStream(stream, "vl_count.txt", FtpRemoteExists.Overwrite, false);
                }
            }

            using (var stream = new MemoryStream())
            { 
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    local_files.ForEach(file =>
                    {
                        writer.WriteLine(file.GetRemotePath() + ";" + file.GetHash());
                    }); 
                    
                    writer.Flush();
                    stream.Position = 0;

                    client.UploadStream(stream, "vl_files.list", FtpRemoteExists.Overwrite, false);
                }            
            }
        }
    }
}
