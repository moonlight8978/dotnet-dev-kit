// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public abstract class S3Uploader<TConfiguration> : IFileUploader where TConfiguration : UploaderConfiguration
{
    private readonly S3Configuration _s3Configuration;

    protected virtual string Prefix => _s3Configuration.Prefix;

    private string StoragePath => string.IsNullOrWhiteSpace(Prefix)
        ? _s3Configuration.Bucket
        : $"{_s3Configuration.Bucket}/{Prefix}";

    protected S3Uploader(IOptions<TConfiguration> storageConfiguration)
    {
        _s3Configuration = storageConfiguration.Value.S3;
    }

    public async Task<UploadResult> UploadAsync(string key, string tmpfile)
    {
        try
        {
            var client = _s3Configuration.GetClient();

            var utility = new TransferUtility(client);
            var request = new TransferUtilityUploadRequest { BucketName = StoragePath, Key = key, FilePath = tmpfile };

            await utility.UploadAsync(request);
            return new UploadResult { Key = key, Provider = UploadProvider.S3.ToString() };
        }
        catch (Exception ex)
        {
            throw new CannotUploadFileException($"{StoragePath}/{key}", UploadProvider.S3, ex);
        }
    }
}
