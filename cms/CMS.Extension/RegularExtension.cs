using System.Text.RegularExpressions;

namespace Extension
{
    public static class RegularExtension
    {
        public static readonly Regex SqlField = new Regex("^([0-9a-zA-Z]+)$");
        
        public static readonly Regex MobilePhone = new Regex("^1[3456789]\\d{9}$");
        
        public static readonly Regex FolderName = new Regex("^([a-zA-Z]{1})([a-zA-Z0-9]+)$");

        public static bool IsSqlField(this string fieldName)
        {
            return SqlField.IsMatch(fieldName);
        }
    }
}