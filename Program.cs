using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Xml.XPath;

internal class Program
{
    private static void Main(string[] args)
    {
        var weaponMarket = new List<Weapon>
        {
            new Weapon
            {
                name = "Shotgun",
                damagePower = 70,
                range = 2,
                cost = 30
            },
            new Weapon
            {
                name = "Rifle",
                damagePower = 30,
                range = 5,
                cost = 60
            },
            new Weapon
            {
                name = "Machine gun",
                damagePower = 50,
                range = 3,
                cost = 45
            }
        };

        var playerOne = new Player("Player1");
        var playerTwo = new Player("Player2");
        Console.WriteLine($"What is the name of {playerOne.name}: ");
        playerOne.name = Console.ReadLine()!;
        Console.WriteLine($"What is the name of {playerTwo.name}: ");
        playerTwo.name = Console.ReadLine()!;

        var choice = AskWeapons(playerOne, weaponMarket);
        playerOne.inventory.weaponList.Add(choice);
        playerOne.currentWeapon = choice;
        playerOne.gold -= choice.cost;
        Console.WriteLine("---------------LAST MOVE------------------");
        Console.WriteLine($"{playerOne.name} got a {choice.name}.");
        Console.WriteLine($"{playerOne.name}'s has {playerOne.gold} golds remaining.");
        Console.WriteLine("-----------------------------------------");
        weaponMarket.Remove(choice);

        choice = AskWeapons(playerTwo, weaponMarket);
        playerTwo.inventory.weaponList.Add(choice);
        playerTwo.currentWeapon = choice;
        playerTwo.gold -= choice.cost;
        Console.WriteLine("---------------LAST MOVE------------------");
        Console.WriteLine($"{playerTwo.name} got a {choice.name}.");
        Console.WriteLine($"{playerTwo.name}'s has {playerTwo.gold} gold remaining.");
        Console.WriteLine("------------------------------------------");
        weaponMarket.Remove(choice);

        playerTwo.Attack(playerOne);
        playerOne.Attack(playerTwo);
        playerOne.Attack(playerTwo);
        Console.ReadLine();
    }

    public static Weapon AskWeapons(Player player, List<Weapon> weapons)
    {
        Console.WriteLine($"{player.name}, Which weapon do you want to take? You have {player.gold} golds.");
        for (int i = 0; i < weapons.Count; i++)
        {
            Console.WriteLine($"{i + 1}- {weapons[i].name} costs ({weapons[i].cost} gold)");
        }
        var choice = Console.ReadLine();
        if (int.TryParse(choice, out var intchoice))
        {
            if (intchoice <= weapons.Count && intchoice > 0)
            {
                return weapons[intchoice - 1];
            }

        }
        return AskWeapons(player, weapons);
    }
}

public class Weapon
{
    public string? name;
    public int damagePower;
    public int range;
    public int cost;
}

public class Inventory
{
    public List<Weapon> weaponList = new();
}

public class Player
{
    public string name;
    public int health;
    public int gold;
    public int status;

    public Vector2 position = new();

    public Inventory inventory = new();
    public Weapon? currentWeapon;

    public Player(string name)
    {
        health = 100;
        gold = 100;
        status = 1;
        position = new Vector2(Random.Shared.Next(0, 6), Random.Shared.Next(0, 6));
        this.name = name;
    }

    public void Attack(Player target)
    {
        if (target.status == 0)
        {
            Console.WriteLine("Enemy is already dead.");
            return;
        }

        if (currentWeapon == null)
        {
            Console.WriteLine("You do not have any weapon in your hand.");
            return;
        }

        if (CanAttack(target) == false)
        {
            Console.WriteLine($"{target.name} is out of range of {this.name}");
            return;
        }

        target.health -= currentWeapon.damagePower;
        if (target.health <= 0)
        {
            target.status = 0;
            Console.WriteLine($"{target.name} is attacked and killed by {this.name}");
        }
        else
            Console.WriteLine($"{target.name} is attacked by {this.name}. Remaining health of {target.name} is {target.health}, still alive.");
    }

    public bool CanAttack(Player target)
    {
        var distance = Vector2.Distance(this.position, target.position);
        if (distance <= this.currentWeapon!.range)
        {
            return true;
        }
        else
            return false;
    }
}

