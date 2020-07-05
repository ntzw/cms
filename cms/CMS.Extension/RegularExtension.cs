using System.Text.RegularExpressions;

namespace Extension
{
    public static class RegularExtension
    {
        static readonly Regex SqlField = new Regex("^([0-9a-zA-Z]+)$");

        public static bool IsSqlField(this string fieldName)
        {
            return SqlField.IsMatch(fieldName);
        }
    }
}