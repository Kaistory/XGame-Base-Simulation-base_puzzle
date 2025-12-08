using System;
using System.Globalization;
using System.Text.RegularExpressions;

public enum StringCase
{
    TitleCase, // Viết hoa chữ cái đầu từng từ
    LowerCase, // thường hết
    UpperCase // HOA hết
}

public static class EnumExtensions
{
    public static string ToFormattedString(this Enum value, bool isConvert = true)
    {
        string str = value.ToString();
        return isConvert ? str.Replace("_", " ") : str;
    }
    
    public static string ToReadableString(this Enum type, StringCase stringCase = StringCase.TitleCase)
    {
        // Lấy tên enum
        string name = type.ToString();

        // Chèn khoảng trắng trước chữ hoa (camelCase, PascalCase)
        string withSpaces = Regex.Replace(name, "([a-z])([A-Z])", "$1 $2");

        switch (stringCase)
        {
            case StringCase.LowerCase:
                return withSpaces.ToLower();

            case StringCase.UpperCase:
                return withSpaces.ToUpper();

            default:
                TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
                return textInfo.ToTitleCase(withSpaces.ToLower());
        }
    }
    
    /// <summary>
    /// Convert enum name PascalCase thành snake_case (localize key).
    /// </summary>
    public static string ToSnakeCase(this System.Enum value)
    {
        if (value == null) return string.Empty;

        string name = value.ToString();

        // Regex: chèn "_" trước chữ hoa và chuyển về thường
        string snake = Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2")
            .ToLower();

        return snake;
    }
}