// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Randomization;

public interface IRandomNumberGenerator
{
    double GetDouble(double multiplier = 1.0);
    double GetDouble(int multiplier = 1);
    int GetInt(int multiplier = 1, int minValue = 0, int maxValue = 100);
}

public class SystemRng : IRandomNumberGenerator
{
    private readonly Random _random = new();

    public double GetDouble(double multiplier = 1)
    {
        return _random.NextDouble() * multiplier;
    }

    public double GetDouble(int multiplier = 1)
    {
        return GetDouble((double)multiplier);
    }

    public int GetInt(int multiplier = 1, int minValue = 0, int maxValue = 100)
    {
        return _random.Next(minValue, maxValue) * multiplier;
    }
}
