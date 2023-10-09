// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace DotnetDevelopmentSdk.Lib.Workflow;

public interface IWorkflowContext
{
    public int ResultCode { get; set; }
    public List<string> ResultMessages { get; set; }
    public bool IsSuccess();
    public bool IsFailure();
    public void Failure(int errorCode, string errorMessage);
    public void Success(string message);
}

public class WorkflowContext : IWorkflowContext
{
    public int ResultCode { get; set; }

    public List<string> ResultMessages { get; set; } = new();

    public bool IsSuccess() => ResultCode == 0;

    public bool IsFailure() => !IsSuccess();

    public void Failure(int errorCode, string errorMessage)
    {
        ResultCode = errorCode;
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ResultMessages.Add(errorMessage);
        }
    }

    public void Success(string message)
    {
        ResultCode = 0;
        ResultMessages = new List<string> { message };
    }
}
