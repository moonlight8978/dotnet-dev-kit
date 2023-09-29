// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Validators;

public abstract class BaseValidatorDataProvider<TValidationData>
{
    public TValidationData Data { get; set; } = default!;
}

public class ValidateResult
{
    public bool IsValid { get; set; }
    public bool IsInvalid { get; set; }
    public List<string> FailureReasons { get; set; } = new();
}

public static class DefaultValidator
{
    public class DataProvider<T> : BaseValidatorDataProvider<T>
    {
    }

    public class DataValidator<T> : AbstractValidator<T>
    {
    }
}

public abstract class
    BaseValidator<TValidationData, TValidatorDataProvider, TValidationDataValidator> : AbstractValidator<
            TValidatorDataProvider>, ITypeDirectedScopeBindedService
    where TValidatorDataProvider : BaseValidatorDataProvider<TValidationData>, new()
    where TValidationDataValidator : AbstractValidator<TValidationData>, new()
{
    protected BaseValidator()
    {
        RuleFor(data => data.Data).NotNull().DependentRules(() =>
        {
            RuleFor(data => data.Data).SetValidator(new TValidationDataValidator()).DependentRules(() =>
            {
                RuleFor(data => data).MustAsync(async (dataProvider, _) =>
                {
                    OnNormalizeData(dataProvider.Data);
                    await OnPrepareAsync(dataProvider);
                    return true;
                }).DependentRules(OnValidate);
            });
        });
    }

    public async Task<ValidateResult> ValidateV2Async(TValidationData data,
        bool throwOnInvalid = false,
        CancellationToken cancellationToken = new())
    {
        var dataProvider = new TValidatorDataProvider { Data = data };
        return await ValidateV2Async(dataProvider, throwOnInvalid, cancellationToken);
    }

    public async Task<ValidateResult> ValidateV2Async(TValidatorDataProvider dataProvider,
        bool throwOnInvalid = false,
        CancellationToken cancellationToken = new())
    {
        var result = await ValidateAsync(dataProvider, cancellationToken);

        if (!result.IsValid && throwOnInvalid)
        {
            throw new Exception("Invalidddddd");
        }

        return new ValidateResult()
        {
            FailureReasons = result.Errors.Select(error => error.ErrorMessage).ToList(),
            IsInvalid = !result.IsValid,
            IsValid = result.IsValid
        };
    }

    protected virtual Task OnPrepareAsync(TValidatorDataProvider dataProvider)
    {
        return Task.CompletedTask;
    }

    protected abstract void OnValidate();

    protected virtual void OnNormalizeData(TValidationData validationData)
    {
    }
}
