  "DarkSkyApiKey": "c5ab83de14d57843c3d9be08ee907000"
# MultiWeather API

An unofficial C# .NET Standard 2.0 library to unify the access to multiple weather services.[CR]
Compatible with .NET 4.5+, Mono 4.6, Windows 8/8.1, Windows Phone 8/8.1, .NET Core 2.0+, Xamarin Android/iOS, Xamarin Forms and Universal Windows Apps.

The first services integrated are [Dark Sky](https://darksky.net/dev) and [OpenWeather](https://openweathermap.org/api) weather services.

This work has been inspired and originally derived by [Jerome Cheng](https://github.com/jcheng31)'s [DarkSkyApi](https://github.com/jcheng31/DarkSkyApi) project.[CR]
All developer are invited to extend this library with additional services.


## Installation

[NuGet](https://www.nuget.org/packages/DarkSkyApi/): `Install-Package MultiWeatherApi`


## Quick Start

### Current Conditions

```c#
using MultiWeatherApi;
using MultiWeatherApi.Model;

...

var client = new DarkSkyService("YOUR API KEY HERE");
Forecast result = await client.GetCurrentWeather(37.8267, -122.423);

...
```

Note that the Dark Sky service doesn't always return all fields for each region. In these cases, some properties may be null or zero.


## Remarks

This library uses a client-oriented approach: the `[XXX]Service` (OpenWheater, DarkSky, ...) object is intended to be an abstraction from which weather data can be obtained.

Returned results *could not contain* all the fields that can appear in the raw JSON obtained through making a direct request to the web service, but exposes them through more 
.NET convention-friendly properties: for example, `precipIntensityMax` is exposed as `MaxPrecipitationIntensity`. These properties are (as shown here) sometimes more verbose, 
but were intended to match the style commonly used in .NET projects.


## Tests

XUnit is used for some simple integration tests with the actual web service. To run the tests, a valid API key must be added to the `xunit.config.json` file
in the `MultiWeatherApi.Test` folder.
