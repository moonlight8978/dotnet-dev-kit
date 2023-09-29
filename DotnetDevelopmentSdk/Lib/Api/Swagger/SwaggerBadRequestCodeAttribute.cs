// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace DotnetDevelopmentSdk.Lib.Api.Swagger;

public class SwaggerBadRequestCodeAttribute : SwaggerErrorCodeAttribute
{
    public SwaggerBadRequestCodeAttribute(params object[] errorCodeOrDescriptions) : base(
        new object[] { CommonApiCode.InvalidRequestData, "Validation error" }.Concat(errorCodeOrDescriptions).ToArray())
    {
    }
}

public class SwaggerNotFoundCodeAttribute : SwaggerErrorCodeAttribute
{
    public SwaggerNotFoundCodeAttribute(params object[] errorCodeOrDescriptions) : base(
        new object[] { CommonApiCode.NotFound, "Not found" }.Concat(errorCodeOrDescriptions).ToArray())
    {
    }
}

public class SwaggerInternalServerErrorCodeAttribute : SwaggerErrorCodeAttribute
{
    public SwaggerInternalServerErrorCodeAttribute(params object[] errorCodeOrDescriptions) : base(
        new object[] { CommonApiCode.UnexpectedError, "Unexpected error" }.Concat(errorCodeOrDescriptions).ToArray())
    {
    }
}

public class SwaggerUnauthenticatedCodeAttribute : SwaggerErrorCodeAttribute
{
    public SwaggerUnauthenticatedCodeAttribute(params object[] errorCodeOrDescriptions) : base(
        new object[] { CommonApiCode.Unauthenticated, "Token is expired or invalid" }.Concat(errorCodeOrDescriptions)
            .ToArray())
    {
    }
}

public class SwaggerErrorCodeAttribute : SwaggerSchemaAttribute
{
    public SwaggerErrorCodeAttribute(params object[] errorCodeOrDescriptions)
    {
        Description = string.Empty;

        if (errorCodeOrDescriptions.Length > 0)
        {
            errorCodeOrDescriptions.ForEachWithIndex((errorCodeOrDescription, index) =>
            {
                if (index.IsEven())
                {
                    Description += $"- {(int)errorCodeOrDescription}: ";
                }
                else
                {
                    Description += $"{errorCodeOrDescription}\n";
                }
            });
        }
        else
        {
            Description =
                "No code defined yet. Please contact to server team if you need more code to handle seperate case.";
        }

        Description = Description.TrimEnd();
    }
}
