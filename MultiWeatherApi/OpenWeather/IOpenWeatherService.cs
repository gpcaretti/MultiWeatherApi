using System.Threading.Tasks;
using MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.OpenWeather {
    public interface IOpenWeatherService {
        Task<WeatherDto> GetCurrentWeather(double latitude, double longitude, Unit unit = Unit.Standard, Language language = Language.En);
        Task<WeatherDto> GetCurrentWeather(string city, Unit unit = Unit.Standard, Language language = Language.En);
        Task<Forecast> GetForecast(double latitude, double longitude, Unit unit = Unit.Standard, Language language = Language.En);
    }
}