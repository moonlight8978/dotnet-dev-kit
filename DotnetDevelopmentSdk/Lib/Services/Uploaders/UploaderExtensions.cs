// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public static class UploaderExtensions
{
    public static void AddUploader<TUploader>(this IServiceCollection services) where TUploader : class, IFileUploader
    {
        services.AddSingleton<TUploader>();
    }
}
