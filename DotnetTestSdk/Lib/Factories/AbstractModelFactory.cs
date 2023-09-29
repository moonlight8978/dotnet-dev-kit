// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;

namespace DotnetTestSdk.Lib.Factories;

public abstract class AbstractModelFactory<TModel, TTrait> : IModelFactory<TModel, TTrait> where TTrait : Enum
{
    private readonly Dictionary<TTrait, Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task>> _traitNameToAction =
        new();

    protected abstract FakerUtils FakerUtils { get; }

    protected void RegisterTrait(TTrait traitName, Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task> traitAction)
    {
        if (_traitNameToAction.TryAdd(traitName, traitAction))
        {
            return;
        }

        throw new ArgumentException($"{traitName} is already exist");
    }

    protected void RegisterTrait(TTrait traitName, Action<TModel> traitAction)
    {
        Task WrappedRegistration(TModel model, FactoryModelBuilder<TModel, TTrait> builder)
        {
            traitAction.Invoke(model);
            return Task.CompletedTask;
        }

        RegisterTrait(traitName, WrappedRegistration);
    }

    protected void RegisterTrait(TTrait traitName, Func<TModel, Task> traitAction)
    {
        async Task WrappedRegistration(TModel model, FactoryModelBuilder<TModel, TTrait> builder)
        {
            await traitAction.Invoke(model);
        }

        RegisterTrait(traitName, WrappedRegistration);
    }

    protected void RegisterTrait(TTrait traitName, Action<TModel, FactoryModelBuilder<TModel, TTrait>> traitAction)
    {
        Task WrappedRegistration(TModel model, FactoryModelBuilder<TModel, TTrait> builder)
        {
            traitAction.Invoke(model, builder);
            return Task.CompletedTask;
        }

        RegisterTrait(traitName, WrappedRegistration);
    }

    protected async Task ApplyTrait(TModel model, TTrait traitName)
    {
        await _traitNameToAction[traitName].Invoke(model, null);
    }

    protected int RandomizeNumber(int max)
    {
        return RandomizeNumber(0, max);
    }

    protected int RandomizeNumber(int min, int max)
    {
        return Faker.RandomNumber.Next(min, max);
    }

    protected int RandomizeNumber(IEnumerable<int> range)
    {
        var iEnumerable = range.ToList();
        var rng = new Random();
        var randomIndex = rng.Next(0, iEnumerable.Count);
        return iEnumerable.ElementAt(randomIndex);
    }

    protected TEnum RandomizeEnum<TEnum>() where TEnum : Enum
    {
        // TODO: cache enum values for higher performance
        var enumValues = Enum.GetValues(typeof(TEnum));
        var random = new Random();
        return (TEnum)enumValues.GetValue(random.Next(enumValues.Length));
    }

    protected string RandomizeEthAddress()
    {
        return FakerUtils.RandomizeEthAddress();
    }

    protected T Unique<T>(string propertyName, Func<T> valueGenerator)
    {
        return FakerUtils.Unique(propertyName, valueGenerator);
    }

    public abstract Task<TModel> Build();

    public abstract Task Save(TModel model);

    public abstract Task SaveMany(List<TModel> models);

    public Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task> GetTrait(TTrait trait) => _traitNameToAction[trait];

    public FactoryModelBuilder<TModel, TTrait> Builder => new(this);

    public abstract void Detach(List<TModel> models);
}

public abstract class AbstractModelFactory<TModel> : AbstractModelFactory<TModel, NoneFactoryTrait>
{
    public new Task ApplyTrait(TModel _, NoneFactoryTrait __)
    {
        throw new ArgumentException("Please define new trait enum for the factor before applying traits.");
    }
}
