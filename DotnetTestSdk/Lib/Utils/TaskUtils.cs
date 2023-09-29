// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetTestSdk.Lib.Utils;

public static class TaskUtils
{
    public static async Task<bool> WaitUntil(Func<bool> checkCondition, int timeout)
    {
        var waitTask = WaitUntil(checkCondition);
        var timeoutCancellationTokenSource = new CancellationTokenSource();
        var completedTask = await Task.WhenAny(waitTask,
            Task.Delay(TimeSpan.FromMilliseconds(timeout), timeoutCancellationTokenSource.Token));
        if (completedTask == waitTask)
        {
            return await waitTask;
        }

        timeoutCancellationTokenSource.Cancel();
        return false;
    }

    private static async Task<bool> WaitUntil(Func<bool> checkCondition)
    {
        var delayTime = TimeSpan.FromMilliseconds(400);
        var isTruthy = checkCondition.Invoke();
        while (!isTruthy)
        {
            await Task.Delay(delayTime);
            isTruthy = checkCondition.Invoke();
        }

        return true;
    }
}
