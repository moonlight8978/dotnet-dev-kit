// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections;
using DotnetDevelopmentSdk.Lib.Services.Csv.Flat;
using DotnetDevelopmentSdk.Lib.Services.Csv.Interfaces;

namespace DotnetDevelopmentSdk.Lib.Services.Csv;

public class CsvTypeConverterProvider
{
    private readonly Dictionary<Type, ICsvTypeConverter> _typeToTypeConverter = new();

    public CsvTypeConverterProvider()
    {
        _typeToTypeConverter.Add(typeof(int), new Int32FlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(string), new StringFlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(Enum), new EnumFlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(IDictionary), new DictionaryFlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(IList), new ListFlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(bool), new BooleanFlatTypeConverter(this));
        _typeToTypeConverter.Add(typeof(object), new UserDefinedClassFlatTypeConverter(this));
    }

    public T Flat<T>() where T : ICsvTypeConverter
    {
        return (T)Flat(typeof(T));
    }

    public ICsvTypeConverter Flat(Type type)
    {
        if (type.IsEnum)
        {
            return _typeToTypeConverter[typeof(Enum)];
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            return _typeToTypeConverter[typeof(IDictionary)];
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            return _typeToTypeConverter[typeof(IList)];
        }

        return _typeToTypeConverter.TryGetValue(type, out var typeConverter)
            ? typeConverter
            : _typeToTypeConverter[typeof(object)];
    }
}
