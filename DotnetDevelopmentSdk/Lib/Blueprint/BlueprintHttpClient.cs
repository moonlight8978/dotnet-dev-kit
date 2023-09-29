// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Api;
using DotnetDevelopmentSdk.Lib.Services.HttpClient;
using Microsoft.Extensions.Options;
using RestSharp;

namespace DotnetDevelopmentSdk.Lib.Blueprint;

public class BlueprintHttpClient : BaseHttpClient
{
    public BlueprintHttpClient(IOptions<BlueprintConfiguration> blueprintConfiguration)
        : base(blueprintConfiguration.Value.BaseUrl)
    {
    }

    public async Task<GetBlueprintDataResponseData> GetBlueprintData(GetBlueprintDataRequestData dto)
    {
        var response =
            await SendRequest(
                new HttpRequestOptions<GetBlueprintDataRequestData, HttpResponseData<GetBlueprintDataResponseData>>
                {
                    Data = dto,
                    Method = Method.Get,
                    Path = $"projects/{dto.Project}/blueprints/{dto.Version}/data"
                });

        return response.Data;
    }

    public async Task<GetLatestBlueprintInfoResponseData> GetLatestBlueprint(GetLatestBlueprintInfoRequestData dto)
    {
        var response =
            await SendRequest(
                new HttpRequestOptions<GetLatestBlueprintInfoRequestData,
                    HttpResponseData<GetLatestBlueprintInfoResponseData>>
                {
                    Data = dto,
                    Path = $"projects/{dto.Project}/blueprints/latest/info",
                    Method = Method.Get
                });

        return response.Data;
    }

    public async Task<GetBlueprintInfoResponseData> GetInfo(GetBlueprintInfoRequestData dto)
    {
        var response =
            await SendRequest(
                new HttpRequestOptions<GetBlueprintInfoRequestData,
                    HttpResponseData<GetBlueprintInfoResponseData>>
                {
                    Data = dto,
                    Path = $"projects/{dto.Project}/blueprints/{dto.Version}/info",
                    Method = Method.Get
                });

        return response.Data;
    }
}
