// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Amazon;
using Amazon.S3;
using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public class UploaderConfiguration : ICustomConfiguration
{
    [Required] public UploadProvider Provider { get; set; }
    public S3Configuration S3 { get; set; }
}

public class S3Configuration : ICustomConfiguration
{
    public string Bucket { get; set; }
    public string Prefix { get; set; }
    public string Region { get; set; }
    public bool LocalMode { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }

    public IAmazonS3 GetClient()
    {
        return LocalMode
            ? new AmazonS3Client(AccessKeyId, SecretAccessKey, RegionEndpoint.GetBySystemName(Region))
            : new AmazonS3Client();
    }
}
