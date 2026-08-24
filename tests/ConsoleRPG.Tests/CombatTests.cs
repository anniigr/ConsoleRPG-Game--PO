using ConsoleRPG.Combat;
using ConsoleRPG.Entities;
using ConsoleRPG.Items;

namespace ConsoleRPG.Tests;

public class CombatTests
{
    [Fact]
    public void NormalAttack_WithLightWeapon_UsesDexterity()
    {
        var player = new Player(0, 0) { Dexterity = 10 };
        var attack = new NormalAttack(player);

        new Sword().Accept(attack);

        Assert.Equal(11, attack.ResultDamage);
        Assert.Equal(20, attack.ResultDefense);
    }

    [Fact]
    public void MagicAttack_WithMagicWeapon_UsesWisdom()
    {
        var player = new Player(0, 0) { Wisdom = 5 };
        var attack = new MagicAttack(player);

        new MagicStaff().Accept(attack);

        Assert.Equal(14, attack.ResultDamage);
        Assert.Equal(10, attack.ResultDefense);
    }

    [Fact]
    public void StealthAttack_WithLightWeapon_UsesDexterityAndLuck()
    {
        var player = new Player(0, 0) { Dexterity = 10, Luck = 10 };
        var attack = new StealthAttack(player);

        new Sword().Accept(attack);

        Assert.Equal(52, attack.ResultDamage);
        Assert.Equal(10, attack.ResultDefense);
    }

    [Fact]
    public void EnemyArmor_ReducesIncomingDamage()
    {
        var enemy = new SimpleEnemy("Test enemy", 'e', 0, 0, 20, 5, 2);

        enemy.TakeDamage(7);

        Assert.Equal(15, enemy.Health);
    }

    [Fact]
    public void PlayerTotalDamage_AddsDifferentWeaponsFromBothHands()
    {
        var player = new Player(0, 0)
        {
            LeftHand = new Sword(),
            RightHand = new MagicStaff()
        };

        Assert.Equal(10, player.GetTotalDamage());
    }
}
