using System.Collections.Generic;

public static class Utils
{
    public static string JoinToString(this IEnumerable<string> l, string separator = "")
    {
        return string.Join(separator, l);
    }
}