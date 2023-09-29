// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace DotnetDevelopmentSdk.Lib.Exceptions;

public abstract class BaseException : Exception
{
    public virtual bool ShouldBeLogged => false;
    public virtual int StatusCode => StatusCodes.Status400BadRequest;
    public virtual int InternalCode => StatusCodes.Status500InternalServerError;
    public string PublicMessage { get; protected init; }

    protected BaseException(string message, Exception? innerException = null) : base(message, innerException)
    {
        PublicMessage = message;
    }
}
