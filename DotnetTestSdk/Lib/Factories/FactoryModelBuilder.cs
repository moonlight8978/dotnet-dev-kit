// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;

namespace DotnetTestSdk.Lib.Factories;

public interface IModelFactory<TModel, TTrait> where TTrait : Enum
{
    Task<TModel> Build();
    Task Save(TModel model);
    Task SaveMany(List<TModel> models);
    FactoryModelBuilder<TModel, TTrait> Builder { get; }
    Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task> GetTrait(TTrait trait);
    void Detach(List<TModel> models);
}

public class FactoryModelBuilder<TModel, TTrait> where TTrait : Enum
{
    private readonly IModelFactory<TModel, TTrait> _factory;
    private readonly List<Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task>> _appliedTraits = new();
    private readonly List<Func<TModel, Task>> _afterSaveActions = new();

    public FactoryModelBuilder(IModelFactory<TModel, TTrait> factory)
    {
        _factory = factory;
    }

    public FactoryModelBuilder<TModel, TTrait> Apply(TTrait traitName)
    {
        _appliedTraits.Add(_factory.GetTrait(traitName));

        return this;
    }

    public FactoryModelBuilder<TModel, TTrait> Apply(Action<TModel, FactoryModelBuilder<TModel, TTrait>> anonymousTrait)
    {
        _appliedTraits.Add((model, builder) =>
        {
            anonymousTrait.Invoke(model, builder);
            return Task.CompletedTask;
        });

        return this;
    }

    public FactoryModelBuilder<TModel, TTrait> Apply(Action<TModel> anonymousTrait)
    {
        _appliedTraits.Add((model, _) =>
        {
            anonymousTrait.Invoke(model);
            return Task.CompletedTask;
        });

        return this;
    }

    public FactoryModelBuilder<TModel, TTrait> AfterSave(Func<TModel, Task> applyToModelAfterSave)
    {
        _afterSaveActions.Add(applyToModelAfterSave);
        return this;
    }

    public FactoryModelBuilder<TModel, TTrait> Apply(
        Func<TModel, FactoryModelBuilder<TModel, TTrait>, Task> anonymousTrait)
    {
        _appliedTraits.Add(anonymousTrait);

        return this;
    }

    public async Task<TModel> Create()
    {
        var model = await Build();
        await _factory.Save(model);
        foreach (var (afterSave, index) in _afterSaveActions.WithIndex())
        {
            await afterSave.Invoke(model);
        }

        _factory.Detach(new List<TModel> { model });
        return model;
    }

    public async Task<List<TModel>> CreateMany(int count, Action<List<TModel>> modification = null)
    {
        if (count == 0)
        {
            return new List<TModel>();
        }

        var models = await BuildMany(count, modification);
        await _factory.SaveMany(models);
        foreach (var (afterSave, index) in _afterSaveActions.WithIndex())
        {
            await afterSave.Invoke(models[index]);
        }

        _factory.Detach(models);
        return models;
    }

    public async Task<TModel> Build()
    {
        var model = await _factory.Build();
        foreach (var trait in _appliedTraits)
        {
            await trait.Invoke(model, this);
        }

        return model;
    }

    public async Task<List<TModel>> BuildMany(int count, Action<List<TModel>> modification = null)
    {
        if (count == 0)
        {
            return new List<TModel>();
        }

        var models = new List<TModel>();
        foreach (var _ in Enumerable.Repeat(0, count))
        {
            models.Add(await Build());
        }

        modification?.Invoke(models.ToList());
        return models.ToList();
    }
}

public class FactoryModelBuilder<TModel> : FactoryModelBuilder<TModel, NoneFactoryTrait>
{
    public FactoryModelBuilder(IModelFactory<TModel, NoneFactoryTrait> factory) : base(factory)
    {
    }
}
