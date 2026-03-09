namespace ConsoleRPG.World;

public abstract class Terrain {
    public abstract char Symbol {get;}
    public abstract bool IsPassable();
    
}

public class Floor : Terrain
{
    public override char Symbol => ' ';
    public override bool IsPassable() => true;
}

public class Wall : Terrain
{
    public override char Symbol => '█';
    public override bool IsPassable() => false;
}

