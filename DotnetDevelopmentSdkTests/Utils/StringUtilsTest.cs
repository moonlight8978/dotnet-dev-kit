// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace DotnetDevelopmentSdkTests.Utils;

public class StringUtilsTest
{
    private static readonly string[] s_subject = { "mirai", "studio", "icetea", "labs" };

    [Test]
    public void TestWithPharseWithSplitIntoSingleWordSubPharses()
    {
        var result = StringUtils.SplitPharseIntoSubPharse(s_subject, 1).ToList();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo("mirai", "studio", "icetea", "labs");
    }

    [Test]
    public void TestWith2WordsSubPharses()
    {
        var result = StringUtils.SplitPharseIntoSubPharse(s_subject, 2).ToList();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo("mirai studio", "studio icetea", "icetea labs");
    }

    [Test]
    public void TestWith3WordsSubPharses()
    {
        var result = StringUtils.SplitPharseIntoSubPharse(s_subject, 3).ToList();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo("mirai studio icetea", "studio icetea labs");
    }

    [Test]
    public void TestWith4WordsSubPharses()
    {
        var result = StringUtils.SplitPharseIntoSubPharse(s_subject, 4).ToList();
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo("mirai studio icetea labs");
    }
}
