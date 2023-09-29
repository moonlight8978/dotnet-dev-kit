// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public interface IFileUploader
{
    Task<UploadResult> UploadAsync(string key, string tmpfile);
}
