namespace LinqExtensions.Extensions;

public static class StringExtensions
{

    public static string SubstringAfter(this string? str, char startChar)
    {
        var posStart = str?.IndexOf(startChar) + 1 ?? -1;
        return posStart != -1 && posStart < str!.Length - 1
        ? str[posStart..]
        : "";
    }

    public static string SubstringUntil(this string? str, char endChar)
    {
        var posEnd = str?.IndexOf(endChar) ?? 0;
        return posEnd > 0 
        ? str![..posEnd]
        : str ?? "";
    }

    public static string StringBetween(this string? str, char startChar, char endChar)
        => str
                ?.SubstringAfter(startChar)
                ?.SubstringUntil(endChar)
                ?? "";

    public static int? ParseInt(this string? str)
        => int.TryParse(str, out var val)
            ? val
            : null;
}

// TODO select and selectMany int => string, string => int