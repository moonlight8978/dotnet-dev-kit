// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net;
using System.Reflection;
using DotnetDevelopmentSdk.Lib.Services;
using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Validators;
using Serilog;

namespace DotnetDevelopmentSdk.Lib.Api;

public static class CommonApiCode
{
    public static int Success = 0;
    public static int InvalidRequestData = 1000;
    public static int Unauthenticated = 1001;
    public static int NotFound = 1002;
    public static int UnexpectedError = 1100;

    public static void Configure(Dictionary<string, int> codeNameToValue)
    {
        foreach (var codeNameAndValue in codeNameToValue)
        {
            var (codeName, codeValue) = codeNameAndValue;
            var fieldInfo = typeof(CommonApiCode).GetField(codeName, BindingFlags.Public | BindingFlags.Static);
            if (fieldInfo == null)
            {
                throw new ArgumentOutOfRangeException(nameof(codeNameAndValue), "code do not exist");
            }

            fieldInfo.SetValue(null, codeValue);
        }
    }
}

public class ApiActionServiceResult
{
    public int Code { get; set; }
    public HttpResponseData? ResponseData { get; set; }
    public HttpErrorResponseData? ErrorData { get; set; }

    public bool IsSuccess()
    {
        return Code == CommonApiCode.Success;
    }
}

public class ApiActionServiceResultFactory<TApiResponseData, TApiErrorCode>
    where TApiResponseData : class, new() where TApiErrorCode : Enum
{
    public ApiActionServiceResult Success(TApiResponseData data)
    {
        return new ApiActionServiceResult
        {
            Code = CommonApiCode.Success,
            ErrorData = null,
            ResponseData = new HttpResponseData<TApiResponseData> { Data = data }
        };
    }

    public ApiActionServiceResult Success()
    {
        return new ApiActionServiceResult
        {
            Code = CommonApiCode.Success,
            ErrorData = null,
            ResponseData = new HttpResponseData { Data = new object() }
        };
    }

    public ApiActionServiceResult Error(TApiErrorCode apiErrorCode, List<string>? messages = null,
        object? errorData = null)
    {
        return Error((int)(object)apiErrorCode, messages, errorData);
    }

    public ApiActionServiceResult Error(int apiErrorCode, List<string>? messages = null,
        object? errorData = null)
    {
        var errorMessages = messages ?? new List<string>();
        return new ApiActionServiceResult
        {
            Code = apiErrorCode,
            ErrorData = new HttpErrorResponseData
            {
                Code = apiErrorCode,
                Data = errorData ?? new object(),
                Message = errorMessages.FirstOrDefault() ?? string.Empty,
                Messages = errorMessages,
                Status = (int)HttpStatusCode.BadRequest
            },
            ResponseData = null
        };
    }
}

public abstract class
    BaseApiActionServiceV2<TApiRequestData, TApiResponseData, TApiErrorCode> : ITypeDirectedScopeBindedService,
        ISingleResponsibilityServiceAsync<HttpRequestData<TApiRequestData>, ApiActionServiceResult>
    where TApiRequestData : class, new() where TApiResponseData : class, new() where TApiErrorCode : Enum
{
    private readonly IValidatorV2<TApiRequestData> _requestValidator;
    private readonly HttpRequestDataValidator _httpRequestDataValidator = new();

    protected virtual IValidatorDataProviderV2<TApiRequestData> ValidatorDataProvider { get; } =
        new ValidatorDataProvider<TApiRequestData>();

    protected readonly ILogger Logger;

    protected readonly ApiActionServiceResultFactory<TApiResponseData, TApiErrorCode> ResultFactory = new();

    protected BaseApiActionServiceV2(IValidatorV2<TApiRequestData> requestValidator)
    {
        _requestValidator = requestValidator;
        Logger = Log.ForContext(GetType());
    }

    public async Task<ApiActionServiceResult> PerformAsync(HttpRequestData<TApiRequestData> httpRequestData)
    {
        ValidatorDataProvider.Data = httpRequestData.Data;

        await OnPrepareAsync(httpRequestData);
        var httpRequestDataValidationResult = await _httpRequestDataValidator.ValidateV2Async(httpRequestData);
        if (!httpRequestDataValidationResult.IsValid)
        {
            return ResultFactory.Error(CommonApiCode.InvalidRequestData,
                httpRequestDataValidationResult.FailureReasons);
        }

        var result = await OnValidateRequestAndProcessAsync(httpRequestData.Data);

        if (result.IsSuccess())
        {
            await OnSuccessAsync(httpRequestData.Data, (HttpResponseData<TApiResponseData>)result.ResponseData!);
        }
        else
        {
            await OnErrorAsync(httpRequestData.Data, result.ErrorData!);
        }

        return result;
    }

    protected virtual async Task<ApiActionServiceResult> OnValidateRequestAndProcessAsync(
        TApiRequestData apiRequestData)
    {
        var validationResult = await OnValidateRequestDataAsync();

        if (validationResult.Contains<TApiErrorCode>(out var errorCode))
        {
            return ResultFactory.Error(errorCode!, validationResult.FailureReasons);
        }

        if (validationResult.IsInvalid)
        {
            return ResultFactory.Error(CommonApiCode.InvalidRequestData, validationResult.FailureReasons);
        }

        var result = await OnProcessAsync(apiRequestData);

        return result;
    }

    protected abstract Task<ApiActionServiceResult> OnProcessAsync(TApiRequestData apiRequestData);

    protected virtual Task OnSuccessAsync(TApiRequestData apiRequestData,
        HttpResponseData<TApiResponseData> httpResponseData)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnErrorAsync(TApiRequestData apiRequestData, HttpErrorResponseData httpErrorResponseData)
    {
        return Task.CompletedTask;
    }

    protected virtual Task OnPrepareAsync(HttpRequestData<TApiRequestData> httpRequestData)
    {
        return Task.CompletedTask;
    }

    protected virtual async Task<ValidationResult> OnValidateRequestDataAsync()
    {
        return await _requestValidator.ValidateV2Async(ValidatorDataProvider);
    }
}

public abstract class
    BasePaginationApiActionService<TApiRequestData, TApiResponseData, TApiErrorCode> : BaseApiActionServiceV2<
        TApiRequestData, TApiResponseData, TApiErrorCode>
    where TApiRequestData : class, IPaginationRequestData, new()
    where TApiResponseData : class, IPaginationResponseData, new()
    where TApiErrorCode : Enum
{
    protected readonly PaginationService PaginationService;

    protected BasePaginationApiActionService(IValidatorV2<TApiRequestData> requestValidator,
        PaginationService paginationService) : base(requestValidator)
    {
        PaginationService = paginationService;
    }

    protected override async Task<ApiActionServiceResult> OnValidateRequestAndProcessAsync(
        TApiRequestData apiRequestData)
    {
        var paginationRequestData = (IPaginationRequestData)apiRequestData;

        IValidatorV2<PaginationRequestData> paginationRequestDataValidator = new PaginationRequestDataValidator();
        var validationResult = await paginationRequestDataValidator.ValidateV2Async(new PaginationRequestData()
        {
            Page = paginationRequestData.Page,
            PerPage = paginationRequestData.PerPage
        });

        if (validationResult.IsInvalid)
        {
            return ResultFactory.Error(CommonApiCode.InvalidRequestData, validationResult.FailureReasons);
        }

        PaginationService.SetPaginationInfoFromRequest(paginationRequestData);

        return await base.OnValidateRequestAndProcessAsync(apiRequestData);
    }

    protected override async Task OnSuccessAsync(TApiRequestData apiRequestData,
        HttpResponseData<TApiResponseData> httpResponseData)
    {
        await base.OnSuccessAsync(apiRequestData, httpResponseData);

        var paginationResponseData = (IPaginationResponseData)httpResponseData.Data;
        PaginationService.SetPaginationInfoToResponse(paginationResponseData);
    }
}
