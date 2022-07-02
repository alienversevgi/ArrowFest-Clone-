using System;

public struct GameEvent 
{
    public event Action EventListeners;

    public void Raise()
    {
        EventListeners.Invoke();
    }

    public void Register(Action listener)
    {
        EventListeners += listener;
    }

    public void UnregisterListener(Action listener)
    {
        EventListeners -= listener;
    }
}