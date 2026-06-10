using ConsoleRPG.World;

namespace ConsoleRPG.Entities;

public class ChaseSoundBehaviour : ISoundBehaviour
{
    public void OnSoundHeard(Enemy enemy, int sourceX, int sourceY, Map map)
    {
        enemy.SoundTargetX = sourceX;
        enemy.SoundTargetY = sourceY;
        enemy.HasSoundTarget = true;
    }
}

public class FleeSoundBehaviour : ISoundBehaviour
{
    public void OnSoundHeard(Enemy enemy, int sourceX, int sourceY, Map map)
    {
        enemy.SoundTargetX = sourceX;
        enemy.SoundTargetY = sourceY;
        enemy.HasSoundTarget = true;
    }
}

public class IgnoreSoundBehaviour : ISoundBehaviour
{
    public void OnSoundHeard(Enemy enemy, int sourceX, int sourceY, Map map)
    {
    }
}