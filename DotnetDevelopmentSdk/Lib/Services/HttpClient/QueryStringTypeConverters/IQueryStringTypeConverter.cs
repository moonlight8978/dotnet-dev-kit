// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using RestSharp;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient.QueryStringTypeConverters;

public interface IQueryStringTypeConverter
{
    void AppendToRequest(RestRequest request, string key, object value);
}
