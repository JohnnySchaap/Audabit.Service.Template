namespace Audabit.Service.Template.App.Weather.Settings;

/// <summary>
/// Configuration settings for weather generation.
/// </summary>
/// <remarks>
/// <para>
/// Controls the parameters for generating random weather forecasts.
/// Temperature range and forecast duration can be customized via configuration.
/// </para>
/// <para>
/// All settings can be configured via appsettings.json or environment variables.
/// Environment variables use double underscore (__) as the hierarchy separator.
/// </para>
/// </remarks>
/// <example>
/// Configuration in appsettings.json:
/// <code>
/// {
///   "WeatherSettings": {
///     "MinTemperature": -20,
///     "MaxTemperature": 55,
///     "ForecastDays": 5
///   }
/// }
/// </code>
/// 
/// Or using environment variables:
/// <code>
/// WeatherSettings__MinTemperature=-20
/// WeatherSettings__MaxTemperature=55
/// WeatherSettings__ForecastDays=5
/// </code>
/// </example>
public sealed record WeatherSettings
{
    /// <summary>
    /// Gets or initializes the minimum temperature in Celsius.
    /// </summary>
    /// <value>
    /// Valid range: typically -20 to 55.
    /// Default is -20°C.
    /// </value>
    /// <remarks>
    /// This value sets the lower bound for randomly generated temperatures.
    /// Must be less than <see cref="MaxTemperature"/>.
    /// </remarks>
    public int MinTemperature { get; init; } = -20;

    /// <summary>
    /// Gets or initializes the maximum temperature in Celsius.
    /// </summary>
    /// <value>
    /// Valid range: typically -20 to 55.
    /// Default is 55°C.
    /// </value>
    /// <remarks>
    /// This value sets the upper bound for randomly generated temperatures.
    /// Must be greater than <see cref="MinTemperature"/>.
    /// </remarks>
    public int MaxTemperature { get; init; } = 55;

    /// <summary>
    /// Gets or initializes the number of days to forecast.
    /// </summary>
    /// <value>
    /// Valid range: 1-30 days.
    /// Default is 5 days.
    /// </value>
    /// <remarks>
    /// Determines how many future days of weather data will be generated.
    /// Larger values generate more forecast entries but don't affect performance significantly.
    /// </remarks>
    public int ForecastDays { get; init; } = 5;
}