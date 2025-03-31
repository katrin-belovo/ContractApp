using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;

namespace ContractApp.Utilities
{
    public static class RegistryHelper
    {
        private const string RegPath = @"Software\ContractApp";
        private const string KeyName = "PasswordHash";

        public static bool IsPasswordSet()
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegPath);
            return key?.GetValue(KeyName) != null;
        }

        public static void SavePassword(string password)
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegPath);
            key.SetValue(KeyName, HashPassword(password));
        }

        public static bool ValidatePassword(string password)
        {
            using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegPath);
            string storedHash = key?.GetValue(KeyName)?.ToString();
            return storedHash == HashPassword(password);
        }

        private static string HashPassword(string password)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}



//ВЕРСИЯ 1. ПО УМОЛЧАНИЮ

//using Microsoft.Win32;
//using System.Security.Cryptography;
//using System.Text;

//public static class RegistryHelper
//{
//    private const string RegPath = @"Software\ContractApp";
//    private const string KeyName = "PasswordHash";

//    public static bool IsPasswordSet()
//    {
//        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegPath);
//        return key?.GetValue(KeyName) != null;
//    }

//    public static void SavePassword(string password)
//    {
//        using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegPath);
//        key.SetValue(KeyName, HashPassword(password));
//    }

//    public static bool ValidatePassword(string password)
//    {
//        using RegistryKey key = Registry.CurrentUser.OpenSubKey(RegPath);
//        string storedHash = key?.GetValue(KeyName)?.ToString();
//        return storedHash == HashPassword(password);
//    }

//    private static string HashPassword(string password)
//    {
//        using SHA256 sha256 = SHA256.Create();
//        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//        return Convert.ToBase64String(bytes);
//    }
//}