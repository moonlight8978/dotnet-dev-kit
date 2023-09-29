// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace DotnetDevelopmentSdk.Lib.Services.Authentication;

/// <summary>
/// Inherited classes should be injected as Singleton
/// </summary>
public abstract class BaseAsymmetricJwtTokenSigner : IJwtCredentialsHandler
{
    public SigningCredentials SigningCredentials { get; }

    protected BaseAsymmetricJwtTokenSigner(string privateKey)
    {
        var rsa = RSA.Create();

        var privateKeyBytes = Convert.FromBase64String(privateKey);
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    }

    protected JwtTokenSignedData GenerateToken<T>(T subject, DateTime currentTime, TimeSpan expiration)
        where T : IJwtSubject
    {
        var claims = subject.ToClaims();
        var expirationTime = currentTime.Add(expiration);
        var token = new JwtSecurityToken(claims: claims, notBefore: currentTime, expires: expirationTime,
            signingCredentials: SigningCredentials);

        return new JwtTokenSignedData()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpireTime = expirationTime
        };
    }
}
