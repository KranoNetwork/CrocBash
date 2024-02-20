public enum TimerState { Started, Running, Ended }
public class Timer
{
    // P R O P E R T I E S
    public float CurrentTime;
    public float EndTime;

    public TimerState State;

    // C O N S T R U C T O R
    public Timer()
    {

    }


    // M E T H O D S
    public void StartTimer(float _currentTime, float _durationInMiliseconds)
    {
        CurrentTime = _currentTime;
        EndTime = _durationInMiliseconds + _currentTime;

        State = TimerState.Started;
    }

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
    }

    public void ResetTimer()
    {

        State = TimerState.Ended;
    }

    private float SecondsToMilliseconds(float seconds)
    {
        return (seconds * 1000);
    }
}