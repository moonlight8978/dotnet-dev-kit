// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;

namespace DotnetDevelopmentSdk.Lib.Api;
                                    
public class ServiceReturnValue : IServiceReturnValue<object>
{
    public const int Success = 0;

    public int Code { get; set; } = Success;
    public string FailReason { get; set; } = string.Empty;
    public object Data { get; set; } = new();
}

public class ServiceReturnValue<T> : IServiceReturnValue<T> where T : class
{
    public int Code { get; set; }
    public string FailReason { get; set; }
    public T Data { get; set; }
}

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class ServiceReturnValueFormatter<T> : IServiceReturnValueFormatter where T : class
{
    public ServiceReturnValue<T> Format(T data, int code, string failReason = "")
    {
        return new ServiceReturnValue<T>() { Data = data, Code = code, FailReason = failReason };
    }

    public ServiceReturnValue<T> Error(int code, string reason = "")
    {
        return new ServiceReturnValue<T>() { Code = code, FailReason = reason };
    }

    public ServiceReturnValue<T> Success(T data)
    {
        return new ServiceReturnValue<T>() { Code = ServiceReturnValue.Success, Data = data };
    }
}

[SuppressMessage("Performance", "CA1822:Mark members as static")]
public class AnyReturnValueFormatter : IServiceReturnValueFormatter
{
    public ServiceReturnValue Format(int code, string failReason = "")
    {
        return new ServiceReturnValue() { Code = code, FailReason = failReason };
    }

    public ServiceReturnValue Error(int code, string reason = "")
    {
        return new ServiceReturnValue() { Code = code, FailReason = reason };
    }

    public ServiceReturnValue Success()
    {
        return new ServiceReturnValue() { Code = ServiceReturnValue.Success };
    }
}

public static class ServiceFormatter
{
    public static ServiceReturnValueFormatter<T> ForReturnType<T>() where T : class
    {
        return new ServiceReturnValueFormatter<T>();
    }

    public static AnyReturnValueFormatter AnyReturnType()
    {
        return new AnyReturnValueFormatter();
    }
}
