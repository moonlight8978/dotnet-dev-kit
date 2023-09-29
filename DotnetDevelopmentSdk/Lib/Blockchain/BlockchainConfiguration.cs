// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Configurations;

namespace DotnetDevelopmentSdk.Lib.Blockchain;

[CustomConfiguration("Blockchain")]
public class BlockchainConfiguration : ICustomConfiguration
{
    public string Network { get; set; }
}

[CustomConfiguration("BlockchainNetworks", new string[] { })]
public abstract class BlockchainNetworkConfiguration<TContractName> : ICustomConfiguration where TContractName : Enum
{
    public string RpcUrl { get; set; }
    public int ChainId { get; set; }
    public Dictionary<string, BlockchainContractInfo> NameToContractInfo { get; set; }

    public BlockchainContractInfo GetContractInfo(TContractName contractName)
    {
        return NameToContractInfo[contractName.ToString()];
    }
}

public class BlockchainContractInfo
{
    public string Address { get; set; }
    public string SignerPrivateKey { get; set; }
}
