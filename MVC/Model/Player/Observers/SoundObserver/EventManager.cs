using ConsoleRPG.World;

namespace ConsoleRPG.Entities.Observers;
public class EventManagerSound
{
    private List<IEventListenerSound> _listeners = new() ;

    public void Subscribe(IEventListenerSound listener) =>  _listeners.Add(listener);
    public void Unsubscribe(IEventListenerSound listener) => _listeners.Remove(listener);
    public void Notify (int x, int y, int range, Map map)
    {
        List<(IEventListenerSound, int)> listenersANDdist = FindListenersInRange(x,y,range,map);
        foreach (var listenerANDdist in listenersANDdist)
        {
            listenerANDdist.Item1.SoundProduced(listenerANDdist.Item2,x,y);
        }
    }

    private List<(IEventListenerSound, int)> FindListenersInRange(int startX, int startY, int range, Map map)
    {
        List<(IEventListenerSound,int)> listenersInRange = new List<(IEventListenerSound,int)>();
        bool[,] visited = new bool[map.Width,map.Height];
        Queue<(int,int,int)> queue = new Queue<(int x,int y,int dist)>();

        visited[startX,startY] = true;
        queue.Enqueue((startX,startY,0));

        int dist = 0;

        while(queue.Count > 0)
        {
            (int x,int y, dist) = queue.Dequeue();
            if (dist > range) continue;

            foreach (var listener in _listeners)
            {
                if (listener is Entity entity && entity.X == x && entity.Y == y)
                {
                    listenersInRange.Add((listener, dist));
                }
            }


            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                (int nx, int ny) = (x + dx[i],y + dy[i]);
                if (nx >= 0 && nx < map.Width && ny >= 0 && ny < map.Height)
                {
                    Cell cell = map.GetCell(nx,ny);
                    if (!visited[nx,ny] && cell.Terrain.IsPassable())
                    {
                        visited[nx,ny] = true;
                        queue.Enqueue((nx,ny,dist+1));
                    }
                }
            }
        }
        return listenersInRange;
    }
}