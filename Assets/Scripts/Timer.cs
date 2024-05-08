/// <summary>
/// The states of the timer.
/// </summary>
public enum TimerState { Off, Started, Running, Ended }
/// <summary>
/// A stateful timer. It calculates the end time and counts up to that time.
/// </summary>
public class Timer
{
    // P R O P E R T I E S
    /// <summary>
    /// The current time.
    /// </summary>
    public float CurrentTime;
    /// <summary>
    /// The time the timer will end at.
    /// </summary>
    public float EndTime;

    /// <summary>
    /// The state of the timer.
    /// </summary>
    public TimerState State;

    // C O N S T R U C T O R
    public Timer()
    {

    }


    // M E T H O D S
    /// <summary>
    /// Starts the timer.
    /// </summary>
    /// <param name="_currentTime">The current time.</param>
    /// <param name="_durationInSeconds">The duration of the timer.</param>
    public void StartTimer(float _currentTime, float _durationInSeconds)
    {
        CurrentTime = _currentTime;
        EndTime = _durationInSeconds + _currentTime;

        State = TimerState.Started;
    }

    /// <summary>
    /// Ticks/updates the timer.
    /// </summary>
    /// <param name="_currentTime">The current time.</param>
    public void UpdateTimer(float _currentTime)
    {
        CurrentTime = _currentTime;
        if (CurrentTime <= EndTime)
        {
            State = TimerState.Running;
        }
        else if (CurrentTime > EndTime)
        {

            State = TimerState.Ended;
        }
        else if (State == TimerState.Off)
        {
            State = TimerState.Off;
        }
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTimer()
    {

        State = TimerState.Off;
    }

    /// <summary>
    /// Calucates the amount of milliseconds from the given seconds.
    /// </summary>
    /// <param name="seconds">The second to convert.</param>
    /// <returns>Given seconds in milliseconds.</returns>
    private float SecondsToMilliseconds(float seconds)
    {
        return (seconds * 1000);
    }
}