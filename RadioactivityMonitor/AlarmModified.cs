namespace RadioactivityMonitor;


public class AlarmModified
{
    public AlarmModified()
    {
        _sensor = GetDefaultSensor();
        _threshold = GetDefaultThreshold();
    }

    public AlarmModified(ISensor sensor, AlarmThreshold threshold)
    {
        _sensor = sensor;
        _threshold = threshold;
    }

    private readonly ISensor _sensor;
    private readonly AlarmThreshold _threshold;
    private bool _alarmOn = false;
    private long _alarmCount = 0;
    private double _lastValue = 0.0;

    private static SensorAdapter GetDefaultSensor() => new();
    private static AlarmThreshold GetDefaultThreshold() => new(LowPressure: 17, HighPressure: 21);

    public void Check()
    {
        _lastValue = _sensor.NextMeasure();

        if (!_alarmOn && OutsideThreshold(_lastValue))
        {
            _alarmCount += 1;
            _alarmOn = true;
        }
    }

    private bool OutsideThreshold(double measurement)
    {
        return (measurement < _threshold.LowPressure || _threshold.HighPressure < measurement);
    }

    public void Reset()
    {
        if (!OutsideThreshold(_lastValue))
        {
            _alarmOn = false;
        }
    }

    public bool AlarmOn
    {
        get { return _alarmOn; }
    }

    public long AlarmCount
    {
        get { return _alarmCount; }
    }
}
