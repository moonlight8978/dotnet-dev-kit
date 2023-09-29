// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using RestSharp;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient.QueryStringTypeConverters;

public class QueryStringBasicConverter : IQueryStringTypeConverter
{
    public void AppendToRequest(RestRequest request, string key, object value)
    {
        var stringValue = value?.ToString();
        if (stringValue == null)
        {
            return;
        }

        request.AddQueryParameter(key, stringValue);
    }
}
