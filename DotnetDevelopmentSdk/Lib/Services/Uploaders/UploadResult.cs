// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public enum UploadProvider
{
    S3,
    Disk
}

public class UploadResult
{
    public string Key { get; set; }
    public string Provider { get; set; }
}
