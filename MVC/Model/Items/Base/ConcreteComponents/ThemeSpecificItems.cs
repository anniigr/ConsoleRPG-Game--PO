
using ConsoleRPG.Combat;

namespace ConsoleRPG.Items;

//Library
public class Book : Item { public override char Symbol => 'B'; public override string Name => "Old Book"; }
public class BlackWand : MagicWeapon { 
    public override char Symbol => '*'; public override string Name => "Czarna Różdżka"; 
    public BlackWand() { damage = 15; } 
}

// SciFi
public class MetalScrap : Item { public override char Symbol => '#'; public override string Name => "Metal Scrap"; }
public class Blaster : MagicWeapon { 
    public override char Symbol => '>'; public override string Name => "Blaster"; 
    public Blaster() { damage = 20; } 
}

// Vault
public class LuckyCoinPouch : HeavyWeapon { 
    public override char Symbol => '$'; public override string Name => "Szczęśliwa Sakwa Monet"; 
    public LuckyCoinPouch() { damage = 12; } 
}