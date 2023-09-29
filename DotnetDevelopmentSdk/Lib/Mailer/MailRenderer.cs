// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using RazorEngine;
using RazorEngine.Templating;

namespace DotnetDevelopmentSdk.Lib.Mailer;

public interface IMailRenderer : ITypeDirectedScopeBindedService
{
    Task<string> RenderAsync<T>(string templatePath, T data);
}

public class RazorMailRenderer : IMailRenderer
{
    public async Task<string> RenderAsync<T>(string templatePath, T data)
    {
        // var config = new TemplateServiceConfiguration
        // {
        //     EncodedStringFactory = new HtmlEncodedStringFactory(), Debug = true
        // };
        var template = await File.ReadAllTextAsync(templatePath);
        return Engine.Razor.RunCompile(template, templatePath, typeof(T), data);
    }
}
