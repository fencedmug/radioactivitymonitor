namespace RadioactivityMonitor.Tests;

public class AlarmModifiedTests
{
    private const double LowPressure = 17;
    private const double HighPressure = 21;
    private const double TooLow = 16;
    private const double TooHigh = 22;
    private const double JustNice = 19;

    /// <summary>
    /// Provides SUT objects to perform act/assert operations
    /// </summary>
    private static (AlarmModified, SensorMock) GetSystemUnderTest(double measurement = JustNice)
    {
        var sensor = new SensorMock
        {
            Measurement = measurement
        };

        var alarm = new AlarmModified(
            sensor,
            new AlarmThreshold(LowPressure, HighPressure));

        return (alarm, sensor);
    }

    [Theory]
    [InlineData(JustNice, false)]
    [InlineData(TooLow, true)]
    [InlineData(TooHigh, true)]
    public void Alarm_Triggers_BasedOn_Threshold(double measurement, bool isAlarmOn)
    {
        var (alarm, _) = GetSystemUnderTest(measurement);

        alarm.Check();

        Assert.Equal(isAlarmOn, alarm.AlarmOn);
    }

    [Fact]
    public void Alarm_StayedOn_When_WithinThreshold_And_ResetNotCalled()
    {
        var (alarm, sensor) = GetSystemUnderTest();

        // Check alarm is on
        sensor.Measurement = TooLow;
        alarm.Check();
        Assert.True(alarm.AlarmOn);

        // Check alarm stayed on
        sensor.Measurement = JustNice;
        alarm.Check();
        Assert.True(alarm.AlarmOn);
    }

    [Fact]
    public void Alarm_TurnedOff_When_WithinThreshold_And_ResetCalled()
    {
        var (alarm, sensor) = GetSystemUnderTest();

        // Check alarm is on
        sensor.Measurement = TooHigh;
        alarm.Check();
        Assert.True(alarm.AlarmOn);

        // Check alarm is off after reset
        sensor.Measurement = JustNice;
        alarm.Check();
        alarm.Reset();
        Assert.False(alarm.AlarmOn);
    }

    [Fact]
    public void Alarm_StayedOn_When_OutsideThreshold_And_ResetCalled()
    {
        var (alarm, sensor) = GetSystemUnderTest();

        // Check alarm is on
        sensor.Measurement = TooLow;
        alarm.Check();
        Assert.True(alarm.AlarmOn);

        // Check alarm stayed on
        alarm.Reset();
        Assert.True(alarm.AlarmOn);
    }

    [Fact]
    public void AlarmCount_Only_Increment_When_Threshold_Rebreach()
    {
        var (alarm, sensor) = GetSystemUnderTest();
        Assert.Equal(0, alarm.AlarmCount);

        // Alarm count stay at 1 when measurement is still at high
        sensor.Measurement = TooHigh;
        alarm.Check();
        Assert.Equal(1, alarm.AlarmCount);
        Assert.True(alarm.AlarmOn);
        alarm.Check();
        Assert.Equal(1, alarm.AlarmCount);
        Assert.True(alarm.AlarmOn);

        // Alarm count stay at 1 even when reset/normalized
        sensor.Measurement = JustNice;
        alarm.Check();
        alarm.Reset();
        Assert.Equal(1, alarm.AlarmCount);
        Assert.False(alarm.AlarmOn);

        // Alarm count increments when threshold is breached again
        sensor.Measurement = TooLow;
        alarm.Check();
        Assert.Equal(2, alarm.AlarmCount);
        Assert.True(alarm.AlarmOn);
    }
}
