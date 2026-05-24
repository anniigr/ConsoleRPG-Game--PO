namespace ConsoleRPG.Entities.Observers;
public class EventManagerDeath
{
    private string _spiciesName;
    private List<IEventListenerDeath> _listeners = new() ;
    public EventManagerDeath(string spieciesName) => _spiciesName = spieciesName;

    public void Subscribe(IEventListenerDeath listener) => _listeners.Add(listener);
    public void Unsubscribe(IEventListenerDeath listener) => _listeners.Remove(listener);
    public void Notify ()
    {
        foreach (var listener in _listeners.ToList())
        {
            listener.MemberDied(_spiciesName);
        }
    }


}