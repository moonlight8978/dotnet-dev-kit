// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Validators;

public static class StringValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeInEmailFormat<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(email => Regex.IsMatch(email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase)
        ).WithMessage("{PropertyName} not in valid email format");
    }

    public static IRuleBuilderOptions<T, string> MustBeInWalletAddressFormat<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(walletAddress =>
        {
            return Regex.IsMatch(walletAddress, @"\A0x[a-fA-F0-9]{40}\Z", RegexOptions.IgnoreCase);
        }).WithMessage("{PropertyName} must be valid wallet address");
    }
}
