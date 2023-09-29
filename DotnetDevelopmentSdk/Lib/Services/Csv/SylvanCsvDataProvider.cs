// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;
using Sylvan.Data.Csv;

namespace DotnetDevelopmentSdk.Lib.Services.Csv;

public class SylvanCsvDataProvider : ICsvDataProvider
{
    private readonly CsvDataReader _csv;
    private readonly List<string> _headers;

    public SylvanCsvDataProvider(CsvDataReader csv)
    {
        _csv = csv;
        _headers = Enumerable.Repeat(0, csv.FieldCount).Select((_, index) => csv.GetName(index)).ToList();
    }

    public string? GetColumnValue(string columnName)
    {
        var ordinal = _csv.GetOrdinal(columnName);
        return _csv.GetString(ordinal);
    }

    public List<string> GetDictionaryKeys(string propertyName)
    {
        return _headers.Select(item =>
        {
            var match = new Regex(@"(?:^" + propertyName + @"_)([^_]+)(?:_|$)").Match(item);
            return match;
        }).Where(match => match.Success).Select(match => match.Groups[^1].Value).Distinct().ToList();
    }
}
