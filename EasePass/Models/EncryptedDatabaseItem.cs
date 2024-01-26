namespace EasePass.Models
{
    internal class EncryptedDatabaseItem
    {
        public EncryptedDatabaseItem(string hash, string salt, byte[] data)
        {
            this.PasswordHash = hash;
            this.Salt = salt;
            this.Data = data;
        }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public byte[] Data { get; set; }
    }
}
