// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Validators;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Api;

public class HttpRequestDataValidator : BaseValidatorV2<HttpRequestData, ValidatorDataProvider<HttpRequestData>>,
    ITypeDirectedScopeBindedService
{
    protected override void OnValidate()
    {
        RuleFor(dataProvider => dataProvider.Data).NotNull();
    }
}
