// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Randomization;

public class ProportionPool<T>
{
    private readonly IRandomNumberGenerator _rng;
    private readonly List<ProportionValue<T>> _proportionValues = new();

    public ProportionPool(IRandomNumberGenerator rng)
    {
        _rng = rng;
    }

    public void Add(ProportionValue<T> proportionValue)
    {
        _proportionValues.Add(proportionValue);
    }

    public void Add(T value, double proportion)
    {
        Add(new ProportionValue<T> { Proportion = proportion, Value = value });
    }

    public T GetRandom()
    {
        var totalWeight = _proportionValues.Sum(proportionValue => proportionValue.Proportion);
        var lotteryLuckyNumber = _rng.GetDouble(totalWeight);
        var selector = 0D;
        var lotteryResult = _proportionValues.Last(proportionValue =>
        {
            selector += proportionValue.Proportion;
            return selector >= lotteryLuckyNumber;
        });
        return lotteryResult.Value;
    }
}

public class ProportionValue<T>
{
    public double Proportion { get; set; }
    public T Value { get; set; } = default!;
}
