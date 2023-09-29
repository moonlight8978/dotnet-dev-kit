// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using FluentValidation;

namespace DotnetDevelopmentSdk.Lib.Validators;

public static class UniqueValidatorExtensions
{
    public static IRuleBuilderOptions<T, IEnumerable<TSource>> MustContainsUniqueItems<T, TSource, TKey>(
        this IRuleBuilder<T, IEnumerable<TSource>> ruleBuilder, Func<TSource, TKey> comparer)
    {
        return ruleBuilder.Must(items =>
        {
            var iEnumerable = items as TSource[] ?? items.ToArray();
            return iEnumerable.Length == iEnumerable.DistinctBy(comparer).Count();
        });
    }

    public static IRuleBuilderOptions<T, IEnumerable<TSource>> MustContainsUniqueItems<T, TSource>(
        this IRuleBuilder<T, IEnumerable<TSource>> ruleBuilder)
    {
        return ruleBuilder.MustContainsUniqueItems(item => item);
    }
}
