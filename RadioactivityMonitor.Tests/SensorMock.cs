namespace RadioactivityMonitor.Tests;

internal class SensorMock : ISensor
{
    public double Measurement { get; set; }

    public double NextMeasure() => Measurement;
}
