# RadioactivityMonitor

## Code
Original code for Alarm and Sensor at:
- ./RadioactivityMonitor/Alarm.cs
- ./RadioactivityMonitor/Sensor.cs

Given constraints
- keep it simple
- no mock framework
- no modification to Sensor class
- consider alarm class already in production


## Original Behavior
Alarm class has the following behaviours:
- AlarmOn is true when threshold is too high/low
- AlarmOn never turns off thereafter
- _alarmCount is never exposed

Issues:
1. Direct coupling to Sensor class - hard to unit test
2. Thresholds are hardcoded - have to update code if any changes
3. Alarm cannot be turned off - application needs to restart to turn off


## Approach
- Dependency Injection
  - Sensors to be initialized outside and injected into Alarm via constructor
    - This allows us to test Alarm without being coupled to the actual Sensor
    - Can provide custom values of sensor measurements during unit testing
  - PressureThreshold values are moved and injected as record
    - This modification makes the class more friendly to configuration changes
    - This also makes the unit tests more maintainable - any changes to actual threshold values won't cause tests to break
- Added Reset() to allow operators to turn off the alarm
- Added AlarmCount -> assuming operators want to know how many times alarm was raised

Given Alarm is already in production, the following assumptions are made:
- preserves the original behaviour for raising alarm
- able to modify how Alarm class is instantiated
  - if not, default constructor can be used for default configuration
- able to extend behaviour with Reset() to turn off alarm
- able to extend behavour with AlarmCount to expose _alarmCount values


## Test cases
- alarm is off when within threshold
- alarm is on when above/below threshold
- alarm stays on when return to within threshold
- alarm turns off only when within threshold and reset is called
- alarm count increments only when going from normal to above/below threshold
