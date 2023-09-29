// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Repositories;

public interface IDynamoRepository
{
    Task CreateTable();
    Task DropTable();
}
