namespace BitirmeProjesiWeb.Utilities
{
    public interface IEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string encryptedText);
    }
}