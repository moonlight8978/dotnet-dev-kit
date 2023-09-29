// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Validators;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithErrorCode<TCode, T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, TCode errorCode) where TCode : Enum
    {
        rule.WithErrorCode(errorCode.ToString());
        return rule;
    }
}
