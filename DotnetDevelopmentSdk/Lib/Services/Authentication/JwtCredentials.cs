// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Services.Authentication;

public interface IJwtSubject
{
    public List<Claim> ToClaims();
}

public interface IJwtCredentialsHandler
{
}

public static class JwtCredentialsExtensions
{
    public static void AddJwtCredentialsHandlers(this IServiceCollection services, params IJwtCredentialsHandler[] jwtCredentialsArray)
    {
        foreach (var jwtCredentials in jwtCredentialsArray)
        {
            services.AddSingleton(jwtCredentials.GetType(), jwtCredentials);
        }
    }
}
