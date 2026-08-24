using ConsoleRPG.Entities;
using ConsoleRPG.Items;

namespace ConsoleRPG.Tests;

public class ItemAndEquipmentTests
{
    [Fact]
    public void StrongModifier_AddsDamageAndChangesName()
    {
        Item weapon = new StrongModifier(new Sword());

        Assert.Equal(11, weapon.GetDamage());
        Assert.Equal("Sword(Strong)", weapon.Name);
        Assert.Equal('†', weapon.Symbol);
    }

    [Fact]
    public void StackedModifiers_PreserveAllEffects()
    {
        Item weapon = new StrongModifier(new UnluckyModifier(new Sword()));

        Assert.Equal(11, weapon.GetDamage());
        Assert.Equal("Sword(Unlucky)(Strong)", weapon.Name);
    }

    [Fact]
    public void EquippedModifiers_ChangePlayerStatistics()
    {
        var player = new Player(0, 0);
        Item weapon = new StrongModifier(new UnluckyModifier(new Sword()));
        player.Inventory.Add(weapon);

        weapon.EquipRight(player);

        Assert.Same(weapon, player.RightHand);
        Assert.DoesNotContain(weapon, player.Inventory);
        Assert.Equal(15, player.Strength);
        Assert.Equal(5, player.Luck);
    }

    [Fact]
    public void EquippingNewWeapon_ReturnsPreviousWeaponToInventory()
    {
        var player = new Player(0, 0);
        var sword = new Sword();
        var staff = new MagicStaff();
        player.Inventory.AddRange([sword, staff]);

        sword.EquipLeft(player);
        staff.EquipLeft(player);

        Assert.Same(staff, player.LeftHand);
        Assert.Contains(sword, player.Inventory);
        Assert.DoesNotContain(staff, player.Inventory);
    }

    [Fact]
    public void TwoHandedWeapon_OccupiesBothHandsAndCountsDamageOnce()
    {
        var player = new Player(0, 0);
        var axe = new Axe();
        player.Inventory.Add(axe);

        axe.EquipLeft(player);

        Assert.Same(axe, player.LeftHand);
        Assert.Same(axe, player.RightHand);
        Assert.Equal(10, player.GetTotalDamage());
    }
}
