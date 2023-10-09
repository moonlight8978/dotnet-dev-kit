// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net;
using DotnetDevelopmentSdk.Lib.Api;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using Newtonsoft.Json;

namespace DotnetTestSdk.Lib.Extensions;

public static class HttpResponseExtensions
{
    public static async Task<T> ReadFromJsonAsync<T>(this HttpContent content)
    {
        var data = JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync());
        data.Should().NotBeNull();
        return data;
    }

    // public static HttpResponseAssertions Should(this HttpResponseMessage instance)
    // {
    //     return new HttpResponseAssertions(instance);
    // }
}

public class HttpResponseAssertions : ReferenceTypeAssertions<HttpResponseMessage, HttpResponseAssertions>
{
    public HttpResponseAssertions(HttpResponseMessage subject) : base(subject)
    {
    }

    protected override string Identifier => "response";

    public AndConstraint<HttpResponseAssertions> HaveSuccessStatus(string because = "", params object[] becauseArgs)
    {
        var response = Subject.Content.ReadAsStringAsync().GetAwaiter().GetResult().Replace("{", "<").Replace("}", ">");
        Execute.Assertion.BecauseOf(because, becauseArgs).ForCondition(Subject.IsSuccessStatusCode)
            .FailWith(
                $"Expected response to have success status code, but receive {Subject.StatusCode}, response {response}");
        return new AndConstraint<HttpResponseAssertions>(this);
    }

    public AndConstraint<HttpResponseAssertions> HaveErrorStatus(int expectedErrorCode, string because = "",
        params object[] becauseArgs)
    {
        Execute.Assertion.BecauseOf(because, becauseArgs)
            .ForCondition(Subject.StatusCode == HttpStatusCode.BadRequest)
            .FailWith($"Expected response to have bad request status code, but receive {Subject.StatusCode}")
            .Then
            .Given(() => Subject.Content.ReadFromJsonAsync<ErrorResponse>().GetAwaiter().GetResult().Code)
            .ForCondition(actualErrorCode => actualErrorCode == expectedErrorCode)
            .FailWith("Expect response contains error code {0}, but got {1} instead", _ => expectedErrorCode,
                actualErrorCode => actualErrorCode);
        return new AndConstraint<HttpResponseAssertions>(this);
    }
}
