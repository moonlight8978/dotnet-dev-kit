// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Net.Mail;
using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using DotnetDevelopmentSdk.Lib.Utils;
using Microsoft.Extensions.Options;

namespace DotnetDevelopmentSdk.Lib.Mailer;

public interface IMailProvider : ITypeDirectedScopeBindedService
{
    Task SendAsync(MailInfo mailInfo);
}

public class MailInfo
{
    public MailAddress AddressTo { get; set; }
    public MailAddress AddressFrom { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}

public class AwsSesMailProvider : IMailProvider
{
    private readonly MailerConfiguration _mailerConfiguration;

    public AwsSesMailProvider(IOptions<MailerConfiguration> options)
    {
        _mailerConfiguration = options.Value;
    }

    public async Task SendAsync(MailInfo mailInfo)
    {
        var awsClient = GetClient();

        var message = new SendEmailRequest
        {
            Source = mailInfo.AddressFrom.ToString(),
            Destination = new Destination { ToAddresses = new List<string> { mailInfo.AddressTo.Address } },
            Message = new Message
            {
                Subject = new Content(mailInfo.Subject),
                Body = new Body { Html = new Content { Charset = "UTF-8", Data = mailInfo.Content } }
            }
        };
        await awsClient.SendEmailAsync(message);
    }

    private AmazonSimpleEmailServiceClient GetClient()
    {
        var awsSesMailerConfiguration = _mailerConfiguration.AwsSes;
        var region = RegionEndpoint.GetBySystemName(awsSesMailerConfiguration.Region);

        if (awsSesMailerConfiguration.IsCloud)
        {
            return new AmazonSimpleEmailServiceClient(region);
        }

        return new AmazonSimpleEmailServiceClient(awsSesMailerConfiguration.AccessKeyId,
            awsSesMailerConfiguration.SecretAccessKey, region);
    }
}
