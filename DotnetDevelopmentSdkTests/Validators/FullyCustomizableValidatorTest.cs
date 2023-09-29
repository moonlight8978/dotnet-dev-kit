// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Validators;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace DotnetDevelopmentSdkTests.Validators;

public class FullyCustomizableValidatorTest
{
    private readonly UpdateUserInfoRequestFullyCustomizedValidator _updateUserInfoRequestValidator;

    public FullyCustomizableValidatorTest()
    {
        _updateUserInfoRequestValidator = new UpdateUserInfoRequestFullyCustomizedValidator();
    }

    [Test]
    public async Task ItValidateAndTypeCastCorrectly()
    {
        var dataProvider = new UpdateUserInfoRequestValidatorDataProvider()
        {
            Data = new UpdateUserInfoRequestData
            {
                Age = 18,
                Email = "Test@example.io",
                Username = "username",
                FriendIds = new List<int> { 1, 2 }
            }
        };

        var validateResult =
            await _updateUserInfoRequestValidator.ValidateAsync(dataProvider);
        validateResult.IsValid.Should().BeTrue();
        dataProvider.Friends.Count.Should().Be(2);
        dataProvider.Data.Email.Should().Be("test@example.io");
    }

    [Test]
    public async Task ItReturnInvalidAtFormatLevel()
    {
        var updateUserInfoRequestData =
            new UpdateUserInfoRequestData { Email = "Test@example.io" };

        var validateResult =
            await _updateUserInfoRequestValidator.ValidateV2Async(updateUserInfoRequestData);
        validateResult.IsInvalid.Should().BeTrue();
    }

    [Test]
    public async Task ItValidateNullability()
    {
        var validateResult =
            await _updateUserInfoRequestValidator.ValidateV2Async(
                new UpdateUserInfoRequestValidatorDataProvider() { Data = null! });
        validateResult.IsInvalid.Should().BeTrue();
    }
}

internal class
    UpdateUserInfoRequestFullyCustomizedValidator : BaseValidator<UpdateUserInfoRequestData,
        UpdateUserInfoRequestValidatorDataProvider, UpdateUserInfoRequestDataValidator>
{
    protected override void OnValidate()
    {
        RuleFor(dataProvider => dataProvider).MustAsync((dataProvider, _) =>
        {
            var validUserIds = new HashSet<int> { 1, 2, 3, 4 };
            dataProvider.Friends =
                new List<UserInfo> { new() { Id = 1, Username = "1" }, new() { Id = 2, Username = "2" } };
            return Task.FromResult(dataProvider.Data.FriendIds.All(friendId => validUserIds.Contains(friendId)));
        }).WithMessage("Friend not exist");

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
