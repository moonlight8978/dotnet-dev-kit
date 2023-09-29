// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Sylvan.Data.Csv;

namespace DotnetDevelopmentSdk.Lib.Services.Csv;

public class FlatCsvImporter
{
    private readonly CsvTypeConverterProvider _csvTypeConverterProvider;

    public FlatCsvImporter(CsvTypeConverterProvider csvTypeConverterProvider)
    {
        _csvTypeConverterProvider = csvTypeConverterProvider;
    }

    public async Task<List<T>> ImportAsync<T>(string filename) where T : class
    {
        var csv = await CsvDataReader.CreateAsync(filename, new CsvDataReaderOptions() { HasHeaders = true });
        var sylvanCsvDataProvider = new SylvanCsvDataProvider(csv);

        var rows = new List<T>();

        var typeConverter = _csvTypeConverterProvider.Flat(typeof(T));

        while (await csv.ReadAsync())
        {
            var record = (T?)typeConverter.ToOriginal("", null, typeof(T), sylvanCsvDataProvider);
            if (record != null)
            {
                rows.Add(record);
            }
        }

        return rows;
    }
}
