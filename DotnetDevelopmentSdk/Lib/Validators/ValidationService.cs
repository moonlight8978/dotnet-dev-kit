// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Validators;

public class Validator<TData> : AbstractValidator<TData>
{
}

public interface IEnumErrorCodeValidator<TErrorCode> where TErrorCode : Enum
{
}

public class Validator<TData, TErrorCode> : Validator<TData>, IEnumErrorCodeValidator<TErrorCode>
    where TErrorCode : Enum
{
    
}

public class ValidationService
{
    private readonly ServiceProvider _serviceProvider;

    public ValidationService(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResult> PerformAsync<TValidator, TData>(TData data)
        where TValidator : Validator<TData>
    {
        var validator = ActivatorUtilities.CreateInstance<TValidator>(_serviceProvider);
        var result = await validator.ValidateAsync(data);

        return new ValidationResult
        {
            FailureReasons = result.Errors.Select(error => error.ErrorMessage).ToList(),
            IsInvalid = !result.IsValid,
            IsValid = result.IsValid,
            ErrorCodes = result.Errors.Select(error => error.ErrorCode).ToList()
        };
    }
}
