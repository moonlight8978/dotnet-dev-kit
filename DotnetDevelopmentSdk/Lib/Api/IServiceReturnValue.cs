// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Api;

public interface IServiceReturnValueFormatter
{
}

public interface IServiceReturnValue<T> : IServiceReturnValue where T : class
{
    T Data { get; set; }
}

public interface IServiceReturnValue
{
    int Code { get; set; }
    string FailReason { get; set; }
}
