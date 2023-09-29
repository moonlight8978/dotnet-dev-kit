// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Reflection;
using DotnetDevelopmentSdk.Lib.Services.HttpClient.QueryStringTypeConverters;
using DotnetDevelopmentSdk.Lib.Utils;
using HybridModelBinding;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;
using Serilog;
using SerilogTimings;

namespace DotnetDevelopmentSdk.Lib.Services.HttpClient;

public abstract class BaseHttpClient : IDisposable, ITypeDirectedSingletonBindedService
{
    private static readonly Dictionary<Type, IQueryStringTypeConverter> s_typeConverters = new()
    {
        { typeof(QueryStringBasicConverter), new QueryStringBasicConverter() },
        { typeof(QueryStringListConverter), new QueryStringListConverter() }
    };

    public static T GetTypeConverter<T>() where T : IQueryStringTypeConverter => (T)s_typeConverters[typeof(T)];

    protected readonly RestClient Client;
    protected readonly ILogger Logger;

    protected BaseHttpClient(string baseUrl, int timeoutInMilliseconds = 10000)
    {
        Logger = Log.ForContext(GetType());
        var options = new RestClientOptions(baseUrl) { ThrowOnAnyError = true, MaxTimeout = timeoutInMilliseconds };
        Client = new RestClient(options, configureSerialization: cfg => cfg.UseNewtonsoftJson());
    }

    protected Task<TResponseData> SendRequest<TResponseData>(string path, Method httpMethod,
        Action<RestRequest>? onRequest = null,
        Func<RestResponse, TResponseData, TResponseData>? onSuccess = null,
        Action<RestRequest, RestResponse>? onError = null)
    {
        return SendRequest(new HttpRequestOptions<object, TResponseData>
        {
            Path = path,
            Data = null,
            Method = httpMethod,
            OnError = onError,
            OnRequest = onRequest,
            OnSuccess = onSuccess
        });
    }

    protected async Task<TResponseData> SendRequest<TRequestData, TResponseData>(
        HttpRequestOptions<TRequestData, TResponseData> httpRequestOptions)
    {
        var request = new RestRequest(httpRequestOptions.Path, httpRequestOptions.Method);

        var data = httpRequestOptions.Data;
        var httpMethod = httpRequestOptions.Method;

        if (data != null)
        {
            switch (httpMethod)
            {
                case Method.Get:
                    OnGetRequest(request, data);
                    break;
                case Method.Post:
                    OnPostRequest(request, data);
                    break;
                case Method.Put:
                    OnPutRequest(request, data);
                    break;
                case Method.Delete:
                case Method.Head:
                case Method.Options:
                case Method.Patch:
                case Method.Merge:
                case Method.Copy:
                case Method.Search:
                default:
                    break;
            }
        }

        httpRequestOptions.OnRequest?.Invoke(request);

        var loggerPrefix = $"HTTP Request {request.Method} {Client.BuildUri(request)} ";
        Logger.Debug(loggerPrefix + $"Data: {JsonConvert.SerializeObject(data)}");

        RestResponse response;
        try
        {
            using (Operation.Time(loggerPrefix))
            {
                response = await Client.ExecuteAsync(request);
            }
        }
        catch (Exception ex)
        {
            httpRequestOptions.OnError?.Invoke(request, null);
            LogError(new RestResponse(), loggerPrefix, ex);
            throw new HttpRequestException(new RestResponse(), ex);
        }

        if (!response.IsSuccessful)
        {
            httpRequestOptions.OnError?.Invoke(request, response);
            LogError(response, loggerPrefix);
            throw new HttpRequestException(response);
        }

        Logger.Information(
            $"{loggerPrefix} successfully. Status: {response.StatusCode}");

        try
        {
            var result = JsonConvert.DeserializeObject<TResponseData>(response.Content!);
            if (httpRequestOptions.OnSuccess != null)
            {
                result = httpRequestOptions.OnSuccess.Invoke(response, result);
            }

            return result;
        }
        catch (Exception ex)
        {
            httpRequestOptions.OnError?.Invoke(request, response);
            LogError(response, loggerPrefix, ex);
            throw new HttpRequestException(response, ex);
        }
    }

    #region Handle request data for each request type

    private static void OnGetRequest<TRequestData>(RestRequest request, TRequestData requestData)
    {
        request.AddQueryData(requestData);
    }

    private static void OnPostRequest<TRequestData>(RestRequest request, TRequestData requestData)
    {
        request.AddQueryData(requestData);
        request.AddBody(requestData!, "application/json");
    }

    private static void OnPutRequest<TRequestData>(RestRequest request, TRequestData requestData)
    {
        OnPostRequest(request, requestData);
    }

    #endregion

    private void LogError(RestResponseBase response, string loggerPrefix, Exception exception = null)
    {
        Logger.Error(exception,
            $@"{loggerPrefix} Attemps: {response.Request?.Attempts} Status: {response.StatusCode} {response.StatusDescription} Data: {response.Content}");
    }

    public void Dispose()
    {
        Client?.Dispose();
        GC.SuppressFinalize(this);
    }
}

public static class RestRequestExtensions
{
    public static void AddQueryData<TRequestData>(this RestRequest request, TRequestData requestData)
    {
        var requestDataType = requestData!.GetType();
        var propertiesInfo = requestDataType.GetProperties();
        foreach (var propertyInfo in propertiesInfo)
        {
            var propertyType = propertyInfo.PropertyType;

            var hybridBindingAttribute = propertyInfo.GetCustomAttribute<HybridBindPropertyAttribute>();
            if (hybridBindingAttribute == null || !hybridBindingAttribute.ValueProviders.Contains(Source.QueryString))
            {
                continue;
            }

            var jsonPropertyAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            var paramName = jsonPropertyAttribute?.PropertyName ?? propertyInfo.Name.TitleCaseToCamelCase();
            IQueryStringTypeConverter typeConverter;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                typeConverter = BaseHttpClient.GetTypeConverter<QueryStringListConverter>();
            }
            else
            {
                typeConverter = BaseHttpClient.GetTypeConverter<QueryStringBasicConverter>();
            }

            typeConverter.AppendToRequest(request, paramName, propertyInfo.GetValue(requestData));
        }
    }
}

public class HttpRequestOptions<TRequestData, TResponseData>
{
    public string Path { get; set; }
    public Method Method { get; set; }
    public TRequestData? Data { get; set; }
    public Action<RestRequest>? OnRequest { get; set; }
    public Func<RestResponse, TResponseData, TResponseData>? OnSuccess { get; set; }
    public Action<RestRequest, RestResponse>? OnError { get; set; }

    public HttpRequestOptions(string path, Method httpMethod, TRequestData? data)
    {
        Path = path;
        Method = httpMethod;
        Data = data;
    }

    public HttpRequestOptions(string path, TRequestData? data)
    {
        Path = path;
        Data = data;
    }

    public HttpRequestOptions()
    {
        Path = "/";
        Method = Method.Get;
        Data = default;
        OnRequest = null;
        OnSuccess = null;
        OnError = null;
    }
}
