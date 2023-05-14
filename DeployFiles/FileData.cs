namespace DeployFiles
{
    class FileData
    {
        private string localPath { get; set; }

        private string remotePath { get; set; }

        private string hash { get; set; }


        public FileData(string localPath)
        {
            this.localPath = localPath;
            this.remotePath = localPath.Replace(Config.MainDir, "");
            this.hash = Utils.GetSHA1HashFromFile(localPath);
        }

        public FileData(string remotePath, string hash)
        {
            this.remotePath = remotePath;
            this.hash = hash;
        }

        public string GetLocalPath() => localPath;

        public string GetRemotePath() => remotePath;

        public string GetHash() => hash;
    }
}
