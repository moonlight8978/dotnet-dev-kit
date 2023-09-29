// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DotnetDevelopmentSdk.Lib.Services.Authentication;

/// <summary>
/// Inherited classes should be injected as Singleton
/// </summary>
public abstract class BaseSymmetricJwtTokenSigner : IJwtCredentialsHandler
{
    public SymmetricSecurityKey SecurityKey { get; }

    public SigningCredentials SigningCredentials { get; }

    protected BaseSymmetricJwtTokenSigner(string secret)
    {
        SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public ClaimsPrincipal Validate(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = SecurityKey
        };
        return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
    }

    protected JwtTokenSignedData GenerateToken<T>(T subject, DateTime currentTime, TimeSpan expiration)
        where T : IJwtSubject
    {
        var utcExpirationTime = currentTime.Add(expiration);
        var token = new JwtSecurityToken(
            claims: subject.ToClaims(),
            notBefore: currentTime,
            expires: utcExpirationTime,
            signingCredentials: SigningCredentials);
        return new JwtTokenSignedData()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpireTime = utcExpirationTime
        };
    }
}
