using System;
using System.Threading.Tasks;

namespace MultiWeatherApi {

    public interface IWeatherService {
        Task GetCurrentWeather(double alcatrazLatitude, double alcatrazLongitude);
        Task GetForecast(double alcatrazLatitude, double alcatrazLongitude);
        Task GetWeatherByDate(double alcatrazLatitude, double alcatrazLongitude, DateTime dateTime);
    }
}