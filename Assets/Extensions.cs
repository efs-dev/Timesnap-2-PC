using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class ListLink<T, U>
{
    public List<T> Keys;
    public List<U> Values;

    public ListLink(List<T> keys, List<U> values)
    {
        Keys = keys;
        Values = values;
    }

    public U Get(T key)
    {
        return Values.GetFrom(key, Keys);
    }

    public void Set(T key, U value)
    {
        Values.SetFrom(value, key, Keys);
    }
}

public static class Extensions
{
    public static void LookAt2D(this Transform a, Transform b)
    {
        var distX = b.position.x - a.position.x;
        var distY = b.position.y - a.position.y;

        float angle = Mathf.Atan2(distY, distX) * 180 / Mathf.PI;

        var angles = a.eulerAngles;
        angles.z = angle;
        a.eulerAngles = angles;
    }

    public static void Zoom(this Camera cam, float orthographicSize, float duration)
    {
        DG.Tweening.DOTween.To(() => cam.orthographicSize, (val) => cam.orthographicSize = val, orthographicSize, duration);
    }

    public static T GetAs<T>(this Dictionary<object, object> dic, object key)
    {
        if (!dic.ContainsKey(key))
            return default;

        return (T)dic[key];
    }

    public static T GetFrom<T, U>(this List<T> values, U key, List<U> keys)
    {
        return values[keys.IndexOf(key)];
    }

    public static void SetFrom<T, U>(this List<T> values, T value, U key, List<U> keys)
    {
        values[keys.IndexOf(key)] = value;
    }

    public static ListLink<T, U> LinkValues<T, U>(this List<T> keys, List<U> values)
    {
        return new ListLink<T, U>(keys, values);
    }

    /// <summary>
    /// Scales a rect by a given amount around its center point
    /// </summary>
    /// <param name="rect">The given rect</param>
    /// <param name="scale">The scale factor</param>
    /// <returns>The given rect scaled around its center</returns>
    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }

    /// <summary>
    /// Scales a rect by a given amount and around a given point
    /// </summary>
    /// <param name="rect">The rect to size</param>
    /// <param name="scale">The scale factor</param>
    /// <param name="pivotPoint">The point to scale around</param>
    /// <returns>The rect, scaled around the given pivot point</returns>
    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;

        //"translate" the top left to something like an origin
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;

        //Scale the rect
        result.xMin *= scale;
        result.yMin *= scale;
        result.xMax *= scale;
        result.yMax *= scale;

        //"translate" the top left back to its original position
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;

        return result;
    }

    public static string SpacesToUnderlines(this string text)
    {
        return text.Replace(" ", "_");
    }

    public static string StripCommentsAndNewlines(this string text)
    {
        var lines = text.Split('\n').ToList();
        return lines.FindAll(x => !x.Contains("//")).Aggregate((x, y) => x + y);
    }

    public static string CombinePaths(this string path, params string[] paths)
    {
        var combined = System.IO.Path.Combine(path, paths[0]);

        for (var i = 1; i < paths.Length; i++)
        {
            combined = System.IO.Path.Combine(combined, paths[i]);
        }

        return combined;
    }

    public static List<T> Merge<T>(this List<T> list, List<T> list2, params List<T>[] lists)
    {
        var newList = list.ToList();
        newList.AddRange(list2);

        for (var i = 0; i < lists.Length; i++)
            newList.AddRange(lists[i]);

        return newList;
    }

    public static string ToQuote(this string text)
    {
        return "\"" + text + "\"";
    }

    public static List<int> IndicesOf(this string text, string value)
    {
        var indices = new List<int>();

        var index = text.IndexOf(value, 0);

        while (index > -1 && index + value.Length < text.Length)
        {
            indices.Add(index);
            index = text.IndexOf(value, index + value.Length);
        }

        return indices;
    }

    public static int IndexExcluding(this string text, List<string> exclude, int startIndex, bool includeEndOfString = false)
    {
        for (var i = startIndex; i < text.Length; i++)
        {
            if (!exclude.Contains(text.Substring(i, 1)))
                return i;
        }

        return includeEndOfString ? text.Length : -1;
    }
    

    public static string Capitalize(this string text, bool eachWord = false)
    {
        if (text == null)
            return null;
        if (text.Length < 2)
            return text.ToUpper();

        text = text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1).ToLower();
        if (!eachWord)
        {
            return text;
        }
        else
        {
            for (var i = 0; i < text.Length; i++)
            {
                var letter = text.Substring(i, 1);

                if (letter == " " && i < text.Length - 1)
                {
                    var prefix = text.Substring(0, i + 1);
                    var change = text.Substring(i + 1, 1).ToUpper();
                    var suffix = i < text.Length - 2 ? text.Substring(i + 2, text.Length - (i + 2)) : "";

                    text = prefix + change + suffix;
                }
            }
            return text;
        }
    }

    public static string ToArgs(this List<string> list, bool inQuotes = true)
    {
        if (list.Count == 0)
            return "";
        if (inQuotes)
            return list.ConvertAll(x => "\"" + x + "\"").Aggregate((x, y) => x + "," + y);
        else
            return list.Aggregate((x, y) => x + "," + y);
    }

    public static List<string> FromArgs(this string args)
    {
        return args.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static void DestroyChildren(this Transform transform)
    {
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            child.parent = null;
            GameObject.Destroy(child.gameObject);
        }
    }

    public static string AutoFormat(this string target, string indentString = "     ")
    {
        if (target.IndexOf("\n") == -1)
            return target;

        var lines = target.Split(new string[] { "\n" }, StringSplitOptions.None);

        var indentLevel = 0;


        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (i >= 2)
            {
                var prevLine = lines[i - 1].Trim();

                if (!prevLine.StartsWith("{"))
                {
                    var checkIndentLine = lines[i - 2].Trim();
                    if (!checkIndentLine.EndsWith("{") && (checkIndentLine.StartsWith("if") || checkIndentLine.StartsWith("else")))
                        indentLevel--;
                }
            }

            if (line.StartsWith("}"))
                indentLevel--;

            var newLine = "";
            for (var j = 0; j < indentLevel; j++)
            {
                newLine += indentString;
            }
            newLine += line;

            if (line.StartsWith("{") || line.EndsWith("{"))
                indentLevel++;
            else if ((line.StartsWith("if") || line.StartsWith("else") || line.StartsWith("for")) && i + 1 < lines.Length)
            {
                var nextLine = lines[i + 1].Trim();

                if (!nextLine.StartsWith("{"))
                    indentLevel++;
            }

            lines[i] = newLine;
        }

        return lines.Aggregate((x, next) => x + "\n" + next);
    }

    public static string ToProperName(this string target)
    {
        string name = "";
        string[] words = target.Split(new string[] { " " }, StringSplitOptions.None);

        for (int i = 0; i < words.Length; i++)
        {
            if (i > 0)
                name += " ";

            string word = words[i];
            word = word.Substring(0, 1).ToUpper() + word.Substring(1, word.Length - 1).ToLower();
            name += word;
        }

        return name;
    }

    public static string StripExtension(this string target)
    {
        var index = target.LastIndexOf(".");

        if (index == -1)
            return target;

        return target.Substring(0, index);
    }

    public static string StringFirstSlash(this string target)
    {
        var index = target.IndexOf("/");
        if (index != 0)
            return target;

        return target.Substring(1, target.Length - 1);
    }

    public static string AsResource(this string target)
    {
        return target.StringFirstSlash().StripExtension();
    }

    public static T Random<T>(this List<T> list)
    {
        if (list.Count == 0)
            return default(T);
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T Random<T>(this List<T> list, List<T> exclude)
    {
        if (list.Count == 0)
            return default(T);

        list = list.Except(exclude).ToList();

        return list[UnityEngine.Random.Range(0, list.Count)];
    }


    public static List<T> RandomExclusive<T>(this List<T> list, int count)
    {
        if (list.Count == 0)
            return new List<T>();

        List<T> rList = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            rList.Add(list[i]);
        }

        List<T> finalList = new List<T>();

        for (int i = 0; i < count; i++)
        {
            T item = rList.Random();
            rList.Remove(item);
            finalList.Add(item);
        }

        return finalList;
    }


    public static void Alpha(this SpriteRenderer sprite, float alpha)
    {
        Color c;
        c = sprite.color;
        c.a = alpha;
        sprite.color = c;
        var sprites = sprite.GetComponentsInChildren<SpriteRenderer>();
        for (var i = 0; i < sprites.Length; i++)
        {
            var child = sprites[i];
            c = child.color;
            c.a = alpha;
            child.color = c;
        }
    }

    /// <summary>
    /// Pops items off a stack, running each through the `predicate` Func until it returns false
    /// </summary>
    /// <typeparam name="T">The type of the stack</typeparam>
    /// <param name="stack">The stack to pop from</param>
    /// <param name="predicate">Determines whether to continue popping items off of the stack</param>
    /// <returns></returns>
    public static IEnumerable<T> PopWhile<T>(this Stack<T> stack, Func<T, bool> predicate)
    {
        while (true)
        {
            if (stack.Count == 0)
                yield break;
            if (!predicate(stack.Peek()))
            {
                yield break;
            }
            yield return stack.Pop();
        }
    }
}