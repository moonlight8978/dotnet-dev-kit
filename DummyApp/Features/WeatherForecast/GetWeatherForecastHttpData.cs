// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using DotnetDevelopmentSdk.Lib.Utils;
using DotnetDevelopmentSdk.Lib.Validators;
using DummyApp.Models;
using FluentValidation;

namespace DummyApp.Features.WeatherForecast;

public class GetWeatherForecastHttpRequestData
{
    public const string Endpoint = "weathers/get";

    public int Offset { get; set; } = 0;
}

public class GetWeatherForecastHttpResponseData
{
    public List<WeatherForecastModel> WeatherForecasts { get; set; } = new();
}

public enum GetWeatherForecastErrorCode
{
}

[BindingType(typeof(IValidatorV2<GetWeatherForecastHttpRequestData>))]
public class GetWeatherForecastRequestValidator : BaseValidatorV2<GetWeatherForecastHttpRequestData,
    ValidatorDataProvider<GetWeatherForecastHttpRequestData>>, ITypeDirectedScopeBindedService
{
    protected override AbstractValidator<GetWeatherForecastHttpRequestData>? DataFormatValidator { get; } =
        new GetWeatherForecastHttpRequestDataValidator();

    protected override void OnValidate()
    {
        RuleFor(dataProvider => dataProvider.Data.Offset).GreaterThanOrEqualTo(2);
    }

    protected override void OnNormalizeData(GetWeatherForecastHttpRequestData data)
    {
        data.Offset += 1;
    }
}

public class GetWeatherForecastHttpRequestDataValidator : AbstractValidator<GetWeatherForecastHttpRequestData>
{
    public GetWeatherForecastHttpRequestDataValidator()
    {
        RuleFor(data => data.Offset).GreaterThanOrEqualTo(0).LessThanOrEqualTo(7);
    }
}
