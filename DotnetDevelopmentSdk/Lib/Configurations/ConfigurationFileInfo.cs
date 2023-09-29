// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Configurations;

public class ConfigurationFileInfo
{
    private readonly string _name;
    private readonly string _path;

    public ConfigurationFileInfo(string name, string path = null)
    {
        _name = name;
        _path = path;
    }

    public string Location(string suffix = null)
    {
        var filename = ToFilename(_name, suffix);
        var pathOnDevelopment = Path.Combine(_path, filename);
        return File.Exists(pathOnDevelopment) ? pathOnDevelopment : filename;
    }

    private static string ToFilename(string name, string suffix)
    {
        var parts = new List<string> { $"{name}settings", suffix, "json" }.Where(e => !string.IsNullOrWhiteSpace(e));
        return string.Join(".", parts);
    }
}
