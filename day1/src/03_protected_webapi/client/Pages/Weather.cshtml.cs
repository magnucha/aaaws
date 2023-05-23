using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;

namespace Client.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "WeatherApi:Scopes")]
    public class WeatherModel : PageModel
    {
        private readonly IDownstreamApi _downstreamApi;
        public WeatherForecast[]? Forecasts { get; set; } = Array.Empty<WeatherForecast>();

        public WeatherModel(
            IDownstreamApi downstreamApi)
        {
            _downstreamApi = downstreamApi;
        }
        public async Task OnGet()
        {
            using var response = await _downstreamApi.CallApiForUserAsync("WeatherApi");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
                Forecasts = content;
            }
            else
            {
                ViewData["Weather"] = $"Error calling API: {response.StatusCode}";
            }
        }
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}
