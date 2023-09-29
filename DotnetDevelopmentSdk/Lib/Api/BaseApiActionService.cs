// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Api;

public interface IApiActionService
{
}

public abstract class BaseApiActionService<TResponseData, TActionInputData> : BaseApiActionService<TResponseData,
    TActionInputData, ServiceReturnValueFormatter<TResponseData>, ServiceReturnValue<TResponseData>>
    where TResponseData : class
{
    protected override ServiceReturnValueFormatter<TResponseData> ReturnValueFormatter { get; } =
        ServiceFormatter.ForReturnType<TResponseData>();
}

public abstract class BaseApiActionService<TActionInputData> : BaseApiActionService<object,
    TActionInputData, AnyReturnValueFormatter, ServiceReturnValue>
{
    protected override AnyReturnValueFormatter ReturnValueFormatter { get; } = ServiceFormatter.AnyReturnType();
}

public abstract class
    BaseApiActionService<TResponseData, TActionInputData, TServiceReturnValueFormatter,
        TServiceReturnValue> : IApiActionService where TResponseData : class
    where TServiceReturnValueFormatter : IServiceReturnValueFormatter
    where TServiceReturnValue : IServiceReturnValue<TResponseData>
{
    protected abstract TServiceReturnValueFormatter ReturnValueFormatter { get; }

    public virtual async Task<TServiceReturnValue> Perform(TActionInputData actionInputData)
    {
        await OnPrepare(actionInputData);
        var result = await OnProcessAction(actionInputData);
        await OnComplete(actionInputData, result);
        return result;
    }

    protected virtual Task OnPrepare(TActionInputData actionInputData)
    {
        return Task.CompletedTask;
    }

    protected abstract Task<TServiceReturnValue> OnProcessAction(TActionInputData actionInputData);

    protected virtual Task OnComplete(TActionInputData actionInputData, TServiceReturnValue serviceReturnValue)
    {
        return Task.CompletedTask;
    }
}
