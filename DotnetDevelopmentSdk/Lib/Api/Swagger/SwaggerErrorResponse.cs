// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Swashbuckle.AspNetCore.Annotations;

namespace DotnetDevelopmentSdk.Lib.Api.Swagger;

public class SwaggerBadRequestErrorResponseData : HttpErrorResponseData
{
    /// <example>400</example>
    [SwaggerSchema("value is always `400`")]
    public new int Status { get; set; }
}

public class SwaggerUnauthenticatedErrorResponseData : HttpErrorResponseData
{
    [SwaggerUnauthenticatedCode] public new int Code { get; set; }

    /// <example>401</example>
    [SwaggerSchema("value is always `401`")]
    public new int Status { get; set; }
}

public class SwaggerNotFoundErrorResponseData : HttpErrorResponseData
{
    [SwaggerNotFoundCode] public new int Code { get; set; }

    /// <example>404</example>
    [SwaggerSchema("value is always `404`")]
    public new int Status { get; set; }
}

public class SwaggerInternalServerErrorResponseData : HttpErrorResponseData
{
    [SwaggerInternalServerErrorCode] public new int Code { get; set; }

    /// <example>500</example>
    [SwaggerSchema("value is always `500`")]
    public new int Status { get; set; }
}
