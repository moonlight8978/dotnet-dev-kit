// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace DotnetDevelopmentSdkTests.Validators;

public class MinimalValidatorTest
{
    private readonly UpdateUserInfoRequestMinimalValidator _updateUserInfoRequestMinimalValidator;

    public MinimalValidatorTest()
    {
        _updateUserInfoRequestMinimalValidator = new UpdateUserInfoRequestMinimalValidator();
    }

    [Test]
    public async Task ItReturnsSuccessResult()
    {
        var dataProvider = new DefaultValidator.DataProvider<UpdateUserInfoRequestData>()
        {
            Data = new UpdateUserInfoRequestData()
            {
                Age = 20,
                Email = "Test@example.Com",
                Username = "username",
                FriendIds = new List<int>()
            }
        };
        var result = await _updateUserInfoRequestMinimalValidator.ValidateV2Async(dataProvider);
        result.IsValid.Should().BeTrue();
        dataProvider.Data.Email.Should().Be("test@example.com");
        result.FailureReasons.Should().BeEmpty();
    }

    [Test]
    public async Task ItReturnsFailureResult()
    {
        var updateUserInfoRequestData = new UpdateUserInfoRequestData()
        {
            Age = -1,
            Email = "Testexample.com",
            Username = "username",
            FriendIds = new List<int>()
        };
        var result = await _updateUserInfoRequestMinimalValidator.ValidateV2Async(updateUserInfoRequestData);
        result.IsInvalid.Should().BeTrue();
        result.FailureReasons.Should().HaveCount(2);
    }
}

internal class
    UpdateUserInfoRequestMinimalValidator : BaseValidator<UpdateUserInfoRequestData,
        DefaultValidator.DataProvider<UpdateUserInfoRequestData>,
        DefaultValidator.DataValidator<UpdateUserInfoRequestData>>
{
    protected override void OnValidate()
    {
        RuleFor(dataProvider => dataProvider.Data.Email).Must(email => email.Contains('@'))
            .WithMessage("Email is invalid");

        RuleFor(dataProvider => dataProvider.Data.Age).GreaterThan(0).WithMessage("Age must be greater than 0")
            .LessThan(50).WithMessage("Age must be less than 50");
    }

    protected override void OnNormalizeData(UpdateUserInfoRequestData data)
    {
        data.Email = data.Email.ToLower();
    }
}
