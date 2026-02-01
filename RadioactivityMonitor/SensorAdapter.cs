namespace RadioactivityMonitor;

/// <summary>
/// Assuming we cannot modify Sensor.cs in anyway
/// This wraps Sensor.cs
/// </summary>
public class SensorAdapter : ISensor
{
    private readonly Sensor _sensor = new Sensor();

    public double NextMeasure() => _sensor.NextMeasure();
}
