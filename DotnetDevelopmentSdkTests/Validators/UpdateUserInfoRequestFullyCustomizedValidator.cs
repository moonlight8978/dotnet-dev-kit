// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using FluentValidation;

namespace DotnetDevelopmentSdkTests.Validators;

internal class UpdateUserInfoRequestData
{
    public string Username { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public List<int> FriendIds { get; set; }
}

internal class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; }
}

internal class UpdateUserInfoRequestValidatorDataProvider : BaseValidatorDataProvider<UpdateUserInfoRequestData>
{
    public List<UserInfo> Friends { get; set; }
}

internal class UpdateUserInfoRequestDataValidator : AbstractValidator<UpdateUserInfoRequestData>
{
    public UpdateUserInfoRequestDataValidator()
    {
        RuleFor(data => data.FriendIds).NotNull()
            .When(data => data.FriendIds == null, ApplyConditionTo.CurrentValidator)
            .Must(friendIds => friendIds.Count > 0)
            .WithMessage("Must have at least 1 friend")
            .When(data => data.FriendIds != null, ApplyConditionTo.CurrentValidator);
        RuleFor(data => data.Email).NotNull();
        RuleFor(data => data.Age).NotNull();
        RuleFor(data => data.Username).NotNull();
    }
}
