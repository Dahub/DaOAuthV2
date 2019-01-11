using System;
using System.Text;

namespace DaOAuthV2.Service.ExtensionsMethods
{
    public static class StringExtensions
    {
        private const int MIN_LENGHT_PASSWORD = 7;
        private const int MIN_LENGHT_CLIENT_SECRET = 12;

        public static bool IsMatchPasswordPolicy(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length < MIN_LENGHT_PASSWORD)
                return false;

            return true;
        }

        public static bool IsMatchClientSecretPolicy(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            if (value.Length < MIN_LENGHT_CLIENT_SECRET)
                return false;

            return true;
        }

        public static string ToScopeWording(this string value, bool readWrite)
        {
            StringBuilder result = new StringBuilder();

            if (readWrite)
                result.Append("RW_");
            else
                result.Append("R_");

            value = value.Replace("_", " ");
            foreach(var s in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                result.Append(s.ToLower());
                result.Append("_");
            }

            result.Remove(result.Length - 1, 1);

            return result.ToString();
        }
    }
}
