# MultiWeather API

An unofficial C# .NET Standard 2.0 library to unify the access to multiple weather services.  
Compatible with .NET 4.5+, Mono 4.6, Windows 8/8.1, Windows Phone 8/8.1, .NET Core 2.0+, Xamarin Android/iOS, Xamarin Forms and Universal Windows Apps.

All available services are available either as is, i.e. you can access their API directly, or with a generic API that returns a unified data model.
This approach leaves the developer a greater degree of freedom in deciding whether to use the generic API to possibly change the underlying service transparently,
or whether to directly use the API of a specific service.

The first services integrated are [Dark Sky](https://darksky.net/dev) and [OpenWeather](https://openweathermap.org/api) weather services.
All developer are invited to extend this library with additional services.


## Installation

* [NuGet](https://www.nuget.org/packages/MultiWeatherApi/): `Install-Package MultiWeatherApi`
* dotNET: `dotnet add package MultiWeatherApi`


## Quick Start

#### Use the generic API

```c#
using MultiWeatherApi;
using MultiWeatherApi.Model;

...

var factory = new WeatherFactory()
IWeatherService client = factory.Create(WeatherFactory.DarkSkyServiceId, "YOUR API KEY HERE");
// or IWeatherService client = factory.Create(WeatherFactory.OpenWeatherServiceId, "YOUR API KEY HERE");

// get current weather with details hour by hour
Weather weather = await client.GetCurrentWeather(LondonLatitude, LondonLongitude, Unit.Imperial);

// get current weather and the forecast for the next n days (usually 7)
WeatherGroup forecast = await client.GetForecast(BolognaLatitude, BolognaLongitude, Unit.SI, Language.Italian);

// get forecast for a specific day (after tomorrow) using default unit and language
Weather weather = await client.GetWeatherByDate(LondonLatitude, LondonLongitude, DateTime.UtcNow.AddDays(+2));

...
```
    
Note: the various underlying services may not leverage all of the fields provided in the unified model.
In these cases, the unrevalued properties will be `null` or `0`.


#### Use the DarkSky and/or the OpenWeather API

```c#
// DarkSky direct API
using MultiWeatherApi.DarkSky;
using MultiWeatherApi.DarkSky.Model;
// OpenWeather direct API
using MultiWeatherApi.OpenWeather;
using MultiWeatherApi.OpenWeather.Model;

...

// DarkSky direct API
IDarkSkyService client = new DarkSkyService(null);
Forecast forecast = await client.GetCurrentWeather(latitude, longitude);
...
Forecast forecast = await client.GetForecast(latitude, longitude);

...

// OpenWeather direct API
IOpenWeatherService client = new OpenWeatherService(null);
WeatherConditions weather = await client.GetCurrentWeather(latitude, longitude);
// ...
ForecastDSL forecast = await client.GetForecastDSL(latitude, longitude);

...
```

## Remarks

This library uses a client-oriented approach: the `[XXX]Service` (OpenWeather, DarkSky, ...) object is intended
to be an abstraction from which weather data can be obtained.

Returned results *could not contain* all the fields that can appear in the raw JSON obtained through making a direct request to the web service, but exposes them through more 
.NET convention-friendly properties: for example, `precipIntensityMax` is exposed as `MaxPrecipitationIntensity`. These properties are (as shown here) sometimes more verbose, 
but were intended to match the style commonly used in .NET projects.

## Add other weather services
Please, follow the remarks above for general approach and the naming convention.

Once implemented a new service, you can modify the `WeatherFactory` or extend it like this>

```
public class WeatherFactoryExt : WeatherFactory {

    public static readonly Guid OtherServiceId = new Guid("UNIQUE-GUIDE-HERE");

    public override IWeatherService Create(Guid serviceId, string apiKey) {
        if (WeatherFactoryExt.OtherServiceId.Equals(serviceId)) {
            return new YourOtherServiceWrapper(apiKey);
        } else {
            base.Create(serviceId, apiKey);
        }
    }
}
```


## Tests

XUnit is used for some simple integration tests with the actual web service. To run the tests, a valid API key must be added to the `xunit.config.json` file
in the `MultiWeatherApi.Test` folder.


## Acknowledgements
This work has been inspired and originally derived by [Jerome Cheng](https://github.com/jcheng31)'s [DarkSkyApi](https://github.com/jcheng31/DarkSkyApi) project.  
