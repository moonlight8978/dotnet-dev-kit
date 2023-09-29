// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections;
using RestSharp;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient.QueryStringTypeConverters;

public class QueryStringListConverter : IQueryStringTypeConverter
{
    public void AppendToRequest(RestRequest request, string key, object value)
    {
        if (value == null)
        {
            return;
        }

        var values = (IList)value;
        // TODO: Support nested object inside array
        foreach (var val in values)
        {
            BaseHttpClient.GetTypeConverter<QueryStringBasicConverter>().AppendToRequest(request, key, val);
        }
    }
}
