namespace ConsoleRPG.Entities.Observers;
public interface IEventListenerSound
{
    void SoundProduced(int dist, int sourceX, int sourceY);
}