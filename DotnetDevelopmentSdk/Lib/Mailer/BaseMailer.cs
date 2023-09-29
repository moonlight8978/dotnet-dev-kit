// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace DotnetDevelopmentSdk.Lib.Mailer;

public abstract class BaseMailer<T> : ITypeDirectedScopeBindedService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMailRenderer _mailRenderer;
    private readonly IMailProvider _mailProvider;
    private readonly MailerConfiguration _mailerConfiguration;
    private readonly ILogger _logger;

    protected BaseMailer(IWebHostEnvironment webHostEnvironment, IOptions<MailerConfiguration> options,
        IMailRenderer mailRenderer, IMailProvider mailProvider)
    {
        _webHostEnvironment = webHostEnvironment;
        _mailRenderer = mailRenderer;
        _mailProvider = mailProvider;
        _mailerConfiguration = options.Value;
        _logger = Log.ForContext(GetType());
    }

    public async Task SendAsync(T data)
    {
        try
        {
            var template = GetTemplatePath();
            var mailInfo = ToMailInfo(data);
            mailInfo.Content = await _mailRenderer.RenderAsync(template, data);
            await _mailProvider.SendAsync(mailInfo);
        }
        catch (Exception ex)
        {
            // silent if mail was failed to sent
            _logger.Information("{Message} {Stack}", ex.Message, ex.StackTrace);
        }
    }

    protected virtual string GetTemplatePath()
    {
        var mailerAttribute = (MailerAttribute)Attribute.GetCustomAttribute(GetType(), typeof(MailerAttribute))!;
        var templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, _mailerConfiguration.TemplateDir,
            $"{mailerAttribute.TemplateName}.html");
        return templatePath;
    }

    protected abstract MailInfo ToMailInfo(T data);
}

public class MailerAttribute : Attribute
{
    public string TemplateName { get; }

    public MailerAttribute(string templateName)
    {
        TemplateName = templateName;
    }
}
