// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Exceptions;

namespace DotnetDevelopmentSdk.Lib.Services.Uploaders;

public class CannotUploadFileException : BaseException
{
    public CannotUploadFileException(string key, UploadProvider provider, Exception? innerException = null) : base(
        $"Cannot upload {key} to {provider}", innerException)
    {
        PublicMessage = "Cannot upload file";
    }
}
