// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Services;

public interface ISingleResponsibilityService<in TInputData, out TOutputData>
{
    TOutputData Perform(TInputData data);
}

public interface ISingleResponsibilityService<in TInputData>
{
    void Perform(TInputData data);
}

public interface ISingleResponsibilityServiceAsync<in TInputData, TOutputData>
{
    Task<TOutputData> PerformAsync(TInputData data);
}

public interface ISingleResponsibilityServiceAsync<in TInputData>
{
    Task PerformAsync(TInputData data);
}
