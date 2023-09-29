// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Utils;

public class ReflectionUtils
{
    public static IEnumerable<Type> TypesImplements<T>() where T : class
    {
        return TypesImplements(typeof(T));
    }

    public static IEnumerable<Type> TypesImplements(Type type)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes()).Where(t =>
                type.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        if (types == null)
        {
            throw new ArgumentException("interface is null");
        }

        return types.ToList();
    }
}
