// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Validators;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public bool IsInvalid { get; set; }
    public List<string> FailureReasons { get; set; } = new();
    public List<string> ErrorCodes { get; set; } = new();

    public bool Contains(Type errorCodeType, out int? errorCode)
    {
        var errorCodeInNumbers = ErrorCodes.Select(errorCode =>
        {
            if (!Enum.TryParse(errorCodeType, errorCode, true, out var result))
            {
                return -999;
            }

            return (int)result;
        }).Where(code => code != -999).ToList();

        if (errorCodeInNumbers.Count == 0)
        {
            errorCode = -1;
            return false;
        }

        errorCode = errorCodeInNumbers.First();
        return true;
    }

    public bool Contains<TErrorCode>(out TErrorCode? errorCode) where TErrorCode : Enum
    {
        var isContained = Contains(typeof(TErrorCode), out var errorCodeInt);

        if (isContained)
        {
            errorCode = (TErrorCode)Enum.ToObject(typeof(TErrorCode), errorCodeInt!.Value);
        }
        else
        {
            errorCode = default;
        }

        return isContained;
    }
}

public interface IValidatorDataProviderV2<TData>
{
    public new TData Data { get; set; }
}

public interface IValidatorV2<TData>
{
    Task<ValidationResult> ValidateV2Async(TData data, CancellationToken cancellationToken = new());

    Task<ValidationResult> ValidateV2Async(IValidatorDataProviderV2<TData> dataProvider,
        CancellationToken cancellationToken = new());
}

public class ValidatorDataProvider<TData> : IValidatorDataProviderV2<TData> where TData : new()
{
    public TData Data { get; set; } = new();
}

public abstract class BaseValidatorV2<TData, TValidatorDataProvider> : AbstractValidator<TValidatorDataProvider>,
    ITypeDirectedScopeBindedService,
    IValidatorV2<TData> where TValidatorDataProvider : IValidatorDataProviderV2<TData> where TData : new()
{
    protected virtual AbstractValidator<TData>? DataFormatValidator => null;

    protected BaseValidatorV2()
    {
        RuleFor(data => data.Data).NotNull().DependentRules(() =>
        {
            var dataFormatValidator = DataFormatValidator;
            if (dataFormatValidator != null)
            {
                RuleFor(data => data.Data).SetValidator(dataFormatValidator).DependentRules(CreateValidationRule);
            }
            else
            {
                CreateValidationRule();
            }
        });
    }

    public async Task<ValidationResult> ValidateV2Async(TData data, CancellationToken cancellationToken = new())
    {
        return await ValidateV2Async(new ValidatorDataProvider<TData> { Data = data }, cancellationToken);
    }

    public async Task<ValidationResult> ValidateV2Async(IValidatorDataProviderV2<TData> dataProvider,
        CancellationToken cancellationToken = new())
    {
        var result = await ValidateAsync((TValidatorDataProvider)dataProvider, cancellationToken);

        return new ValidationResult
        {
            FailureReasons = result.Errors.Select(error => error.ErrorMessage).ToList(),
            IsInvalid = !result.IsValid,
            IsValid = result.IsValid,
            ErrorCodes = result.Errors.Select(error => error.ErrorCode).ToList()
        };
    }

    protected virtual Task OnPrepareAsync(TValidatorDataProvider dataProvider)
    {
        return Task.CompletedTask;
    }

    protected abstract void OnValidate();

    protected virtual void OnNormalizeData(TData data)
    {
    }

    private void CreateValidationRule()
    {
        RuleFor(data => data).MustAsync(async (dataProvider, _) =>
        {
            OnNormalizeData(dataProvider.Data);
            await OnPrepareAsync(dataProvider);
            return true;
        }).DependentRules(OnValidate);
    }
}
