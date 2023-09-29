// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Mailer;

[CustomConfiguration("Mailer")]
public class MailerConfiguration : ICustomConfiguration
{
    public MailProvider Provider { get; set; }
    public string TemplateDir { get; set; }
    public AwsSesMailerConfiguration AwsSes { get; set; }
}

public enum MailProvider
{
    AwsSes,
}

public class AwsSesMailerConfiguration
{
    public bool IsCloud { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string Region { get; set; }
}
