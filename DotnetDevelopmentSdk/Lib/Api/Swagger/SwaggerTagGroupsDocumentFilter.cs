// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DotnetDevelopmentSdk.Lib.Api.Swagger;

public abstract class SwaggerTagGroupsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var tagGroups = new OpenApiArray();

        foreach (var tagGroup in CreateTagGroups())
        {
            var (groupName, tagNames) = tagGroup;
            tagGroups.Add(CreateTagGroup(groupName, tagNames));
        }

        swaggerDoc.Extensions["x-tagGroups"] = tagGroups;
    }

    protected abstract Dictionary<string, string[]> CreateTagGroups();

    private static OpenApiObject CreateTagGroup(string groupName, params string[] tagNames)
    {
        var tagGroup = new OpenApiObject { { "name", new OpenApiString(groupName) } };
        var tags = new OpenApiArray();
        tags.AddRange(tagNames.Select(tagName => new OpenApiString(tagName)));
        tagGroup.Add("tags", tags);

        return tagGroup;
    }
}
