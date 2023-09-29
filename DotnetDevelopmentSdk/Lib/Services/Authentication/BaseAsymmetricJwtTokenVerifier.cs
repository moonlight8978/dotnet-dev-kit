// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DotnetDevelopmentSdk.Lib.Services.Authentication;

/// <summary>
/// Inherited classes should be injected as Singleton
/// </summary>
public abstract class BaseAsymmetricJwtTokenVerifier : IJwtCredentialsHandler
{
    protected BaseAsymmetricJwtTokenVerifier(string publicKey)
    {
        var rsa = RSA.Create();

        var publicKeyBytes = Convert.FromBase64String(publicKey);
        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        _securityKey = new RsaSecurityKey(rsa);
    }

    private readonly RsaSecurityKey _securityKey;

    public TokenValidationParameters TokenValidationParameters => new()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = _securityKey,
        LifetimeValidator = LifetimeValidator,
        ClockSkew = TimeSpan.Zero
    };

    private static bool LifetimeValidator(DateTime? notBefore,
        DateTime? expires,
        SecurityToken securityToken,
        TokenValidationParameters validationParameters)
    {
        return expires != null && expires > DateTime.UtcNow;
    }
}
