// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Utils;

public interface IDateTimeProvider
{
    DateTime Now(bool cache = true);
    DateTime Today();
    DateTime Tomorrow();
}

public class DateTimeProvider : IDateTimeProvider
{
    private DateTime? _now;

    public DateTime Now(bool cache = true)
    {
        if (cache)
        {
            return _now ??= DateTime.UtcNow;
        }

        return DateTime.UtcNow;
    }

    public DateTime Today() => DateTime.Today;

    public DateTime Tomorrow() => Today().AddDays(1);
}
