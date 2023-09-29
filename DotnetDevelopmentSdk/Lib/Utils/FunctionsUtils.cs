// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public static class FunctionUtils
{
    public static T RetryUntil<T>(this Func<T> action, Func<T, bool> evaluateCondition)
    {
        var result = action.Invoke();
        while (!evaluateCondition.Invoke(result))
        {
            result = action.Invoke();
        }

        return result;
    }

    public static void RetryUntil(this Action action, Func<bool> evaluateCondition)
    {
        if (evaluateCondition.Invoke())
        {
            return;
        }

        RetryUntil<object>(() =>
        {
            action.Invoke();
            return new { };
        }, _ => evaluateCondition.Invoke());
    }
}
