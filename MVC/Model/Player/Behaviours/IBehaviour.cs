using ConsoleRPG.World;

namespace ConsoleRPG.Entities;

public interface ISoundBehaviour
{
    void OnSoundHeard(Enemy enemy, int sourceX, int sourceY, Map map);
}

public interface IVisionBehaviour
{
    void OnPlayerSeen(Enemy enemy, List<Entity> visiblePlayers, Map map);
}