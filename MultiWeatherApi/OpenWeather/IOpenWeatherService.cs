using System.Threading.Tasks;
using MultiWeatherApi.Model;
using MultiWeatherApi.OpenWeather.Model;

namespace MultiWeatherApi.OpenWeather {
    public interface IOpenWeatherService {
        Task<WeatherDto> GetCurrentWeather(double latitude, double longitude, OWUnit unit = OWUnit.Standard, Language language = Language.English);
        Task<WeatherDto> GetCurrentWeather(string city, OWUnit unit = OWUnit.Standard, Language language = Language.English);
        Task<Forecast> GetForecast(double latitude, double longitude, OWUnit unit = OWUnit.Standard, Language language = Language.English);
    }
}