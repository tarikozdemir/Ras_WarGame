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

        var playerCount = AskPlayerCount();

        var players = new List<Player>();

        for (int i = 0; i < playerCount; i++)
        {
            Console.WriteLine($"What is the name of player {i + 1}: ");
            string playerName = Console.ReadLine()!;
            var player = new Player(playerName);
            var order = AskWeapons(player, weaponMarket);
            player.inventory.weaponList.Add(order);
            player.currentWeapon = order;
            player.gold -= order.cost;
            players.Add(player);
            Console.WriteLine("--------------- LAST MOVE ---------------");
            Console.WriteLine($"{player.name} got a {order.name}.");
            Console.WriteLine($"{player.name} has {player.gold} golds remaining.");
            Console.WriteLine("----------------- NEXT ------------------");
            weaponMarket.Remove(order);
        }

        bool gameRunning = true;

        while (gameRunning)
        {
            foreach (var player in players)
            {
                if (player.status == 0) continue;

                Console.WriteLine($"{player.name}, what do you want to do? Press 1 for Attack, 2 for Move:");
                if (int.TryParse(Console.ReadLine(), out int actionChoice) && (actionChoice == 1 || actionChoice == 2))
                {
                    if (actionChoice == 1)
                    {
                        var targetIndex = (players.IndexOf(player) + 1) % players.Count;
                        player.Attack(players[targetIndex]);
                        if (players[targetIndex].status == 0)
                        {
                            Console.WriteLine($"{players[targetIndex].name} has died. Game Over.");
                            gameRunning = false;
                            break;
                        }
                    }
                    else if (actionChoice == 2)
                    {
                        player.Move();
                        Console.WriteLine("--------------- LAST MOVE ---------------");
                        Console.WriteLine($"{player.name} moved to a new position at {player.position}.");
                        Console.WriteLine("----------------- NEXT ------------------");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 1 for Attack or 2 for Move.");
                }
            }
        }
    }

    public static Weapon AskWeapons(Player player, List<Weapon> weapons)
    {
        Console.WriteLine($"{player.name}, Which weapon do you want to buy? You have {player.gold} golds.");
        for (int i = 0; i < weapons.Count; i++)
        {
            Console.WriteLine($"{i + 1}- {weapons[i].name} costs {weapons[i].cost} golds.");
        }
        var choice = Console.ReadLine();
        if (int.TryParse(choice, out int intChoice) && intChoice > 0 && intChoice <= weapons.Count)
        {
            return weapons[intChoice - 1];
        }
        Console.WriteLine("Invalid selection. Please try again.");
        return AskWeapons(player, weapons);
    }

    public static int AskPlayerCount()
    {
        Console.WriteLine("How many players will play the game?");
        var playerCount = Console.ReadLine();

        if (int.TryParse(playerCount, out int intPlayerCount) && intPlayerCount > 1)
        {
            return intPlayerCount;
        }
        Console.WriteLine("Player count cannot be lower than 2. Please enter a valid number.");
        return AskPlayerCount();
    }
}

public class Weapon
{
    public string name;
    public int damagePower;
    public int range;
    public int cost;
}

public class Inventory
{
    public List<Weapon> weaponList = new List<Weapon>();
}

public class Player
{
    public string name;
    public int health = 100;
    public int gold = 100;
    public int status = 1;
    public Vector2 position;
    public Inventory inventory = new Inventory();
    public Weapon currentWeapon;

    public Player(string name)
    {
        this.name = name;
        position = new Vector2(Random.Shared.Next(0, 6), Random.Shared.Next(0, 6));
    }

    public void Move()
    {
        position = new Vector2(Random.Shared.Next(0, 6), Random.Shared.Next(0, 6));
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

        if (!CanAttack(target))
        {
            Console.WriteLine($"{target.name} is out of range of {this.name}.");
            return;
        }

        target.health -= currentWeapon.damagePower;
        if (target.health <= 0)
        {
            target.status = 0;
            Console.WriteLine($"{target.name} is attacked and killed by {this.name}.");
        }
        else
        {
            Console.WriteLine($"{target.name} is attacked by {this.name}. {target.name}'s remaining health is {target.health}.");
        }
    }

    public bool CanAttack(Player target)
    {
        var distance = Vector2.Distance(this.position, target.position);
        return distance <= this.currentWeapon.range;
    }
}
