// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using FluentValidation;

namespace DummyApp.Features.Pagination;

public class PaginationValidator : AbstractValidator<PaginationRequestData>
{
    public PaginationValidator()
    {
        RuleFor(data => data.Page).GreaterThan(0);
        RuleFor(data => data.PerPage).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
