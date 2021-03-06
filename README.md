# MultiWeather API

An unofficial C# .NET Standard 2.0 library to unify the access to multiple weather services.  
Compatible with .NET 4.5+, Mono 4.6, Windows 8/8.1, Windows Phone 8/8.1, .NET Core 2.0+, .NET 5.0,
Xamarin Android/iOS, Xamarin Forms and Universal Windows Apps.

All implemented services are available either as-is, i.e. you can access their API directly,
or with a generic API that returns a unified data model.
This approach leaves the developer a greater degree of freedom in deciding whether to use
the generic API (to possibly change the underlying service transparently) or whether to directly
use the API of a specific service.

The first services integrated are the [Dark Sky](https://darksky.net/dev) and
the [OpenWeather](https://openweathermap.org/api) weather services.
All developers are invited to extend this library with additional services (see below).


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

// get current weather with detailed hour by hour conditions using
// Imperial units and the default language (English)
Weather weather = await client.GetCurrentWeather(
                    LondonLatitude, 
                    LondonLongitude, 
                    Unit.Imperial);

// get current weather and the forecast for the next n days
// (usually 7) using Italian lang.
WeatherGroup forecast = await client.GetForecast(
                    BolognaLatitude,
                    BolognaLongitude,
                    Unit.SI, 
                    Language.Italian);

// get forecast for a specific day using default units (SI) 
// and language (English)
Weather weather = await client.GetWeatherByDate(
                    LondonLatitude, 
                    LondonLongitude, 
                    DateTime.UtcNow.AddDays(+2));

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
IDarkSkyService client = new DarkSkyService("YOUR API KEY HERE");
Forecast current = await client.GetCurrentWeather(latitude, longitude);
Forecast forecast = await client.GetForecast(latitude, longitude);

...

// OpenWeather direct API
IOpenWeatherService client = new OpenWeatherService("YOUR API KEY HERE");
WeatherConditions weather = await client.GetCurrentWeather(latitude, longitude);
ForecastDSL forecast = await client.GetForecastDSL(latitude, longitude);

...
```

## Remarks

This library uses a client-oriented approach: the `[XXX]Service` (OpenWeather, DarkSky, ...)
 object is intended to be an abstraction from which weather data can be obtained.

Returned results *could not contain* all the fields that can appear in the raw JSON obtained
through making a direct request to the web service, but exposes them through more 
.NET convention-friendly properties: **for example**, `precipIntensityMax` is exposed as
`MaxPrecipitationIntensity`.  
These properties are (as shown here) sometimes more verbose, but were intended to match
the style commonly used in .NET projects.

## Add other weather services
Please, follow the remarks above for the general approach and the naming convention to
implement them.

Once implemented a new service, to include as new service of the *generic* API you can
modify the `WeatherFactory` or extend it like this:

```c#
using MultiWeatherApi;
using MultiWeatherApi.Model;

...

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
If you change or add new code, please integrate and check the test cases.


## Samples
View the test code to see some sample of use.


## Acknowledgements
This work has been inspired and originally derived by [Jerome Cheng](https://github.com/jcheng31)'s
[DarkSkyApi](https://github.com/jcheng31/DarkSkyApi) project.  
