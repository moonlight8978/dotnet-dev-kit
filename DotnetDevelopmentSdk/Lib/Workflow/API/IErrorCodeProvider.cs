// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Workflow.API;

public interface IErrorCodeProvider
{
    public int ValidationError { get; }
    public int Unauthenticated { get; }
    public int NotFound { get; }
    public int Unauthorized { get; }
    public bool IsClientError(int errorCode);
    public bool IsServerError(int errorCode);
}
