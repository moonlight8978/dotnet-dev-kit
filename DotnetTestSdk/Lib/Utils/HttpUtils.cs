// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Text;
using Newtonsoft.Json;

namespace DotnetTestSdk.Lib.Utils;

public class HttpUtils
{
    public static HttpContent CreateJsonContent(object data)
    {
        return new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
    }
}
