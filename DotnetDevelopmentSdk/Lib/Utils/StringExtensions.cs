// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public static class StringExtensions
{
    public static string TitleCaseToCamelCase(this string value)
    {
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}

public static class StringUtils
{
    public static IEnumerable<string> SplitPharseIntoSubPharse(string[] words, int wordsCount)
    {
        if (wordsCount == 1)
        {
            return words;
        }

        var result = new List<string>();
        for (var wordPosition = 0; wordPosition <= words.Length - wordsCount; wordPosition++)
        {
            var subWords = words.Skip(wordPosition).Take(wordsCount);
            result.Add(string.Join(" ", subWords));
        }

        return result;
    }
}
