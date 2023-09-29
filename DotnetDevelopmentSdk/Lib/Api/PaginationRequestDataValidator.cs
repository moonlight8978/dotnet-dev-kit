// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Api;

public class PaginationRequestDataValidator : BaseValidatorV2<PaginationRequestData, ValidatorDataProvider<PaginationRequestData>>
{
    protected override void OnValidate()
    {
        RuleFor(dataProvider => dataProvider.Data.Page).GreaterThan(0);
        RuleFor(dataProvider => dataProvider.Data.PerPage).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
