using System;
using System.Collections.Generic;
using System.Numerics;

internal class Program
{
    private static void Main(string[] args)
    {
        Game game = new Game();
        game.InitializeGame();
        game.RunGameLoop();
    }
}

public class Game
{
    private List<Weapon> weaponMarket = new List<Weapon>();
    private List<Player> players = new List<Player>();
    private bool gameRunning = true;

    public Game()
    {
        LoadWeapons();
    }

    private void LoadWeapons()
    {
        weaponMarket.Add(new Weapon("Shotgun", 70, 2, 30));
        weaponMarket.Add(new Weapon("Rifle", 30, 5, 60));
        weaponMarket.Add(new Weapon("Machine gun", 50, 3, 45));
    }

    public void InitializeGame()
    {
        int playerCount = AskPlayerCount();
        for (int i = 0; i < playerCount; i++)
        {
            Console.WriteLine($"What is the name of player {i + 1}?");
            string playerName = Console.ReadLine()!;
            var player = new Player(playerName, 100, 100);
            var weapon = ChooseWeaponForPlayer(player);
            player.PurchaseWeapon(weapon);
            players.Add(player);
            DisplayPlayerStatus(player);
            weaponMarket.Remove(weapon);
        }
    }

    private int AskPlayerCount()
    {
        while (true)
        {
            Console.WriteLine("How many players will play the game?");
            if (int.TryParse(Console.ReadLine(), out int playerCount) && playerCount >= 2)
            {
                return playerCount;
            }
            Console.WriteLine("Invalid input. Player count cannot be less than 2.");
        }
    }

    private Weapon ChooseWeaponForPlayer(Player player)
    {
        while (true)
        {
            Console.WriteLine($"{player.Name}, which weapon do you want to buy? You have {player.Gold} gold.");
            for (int i = 0; i < weaponMarket.Count; i++)
            {
                Console.WriteLine($"{i + 1}- {weaponMarket[i].Name} costs {weaponMarket[i].Cost} gold.");
            }
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= weaponMarket.Count)
            {
                return weaponMarket[choice - 1];
            }
            Console.WriteLine("Invalid selection. Please try again.");
        }
    }

    private void DisplayPlayerStatus(Player player)
    {
        Console.WriteLine($"--------------- LAST MOVE ---------------");
        Console.WriteLine($"{player.Name} now has a {player.CurrentWeapon.Name}.");
        Console.WriteLine($"{player.Name} has {player.Gold} gold remaining.");
        Console.WriteLine("----------------- NEXT ------------------");
    }

    public void RunGameLoop()
    {
        while (gameRunning)
        {
            foreach (var player in players)
            {
                if (!player.IsAlive) continue;

                Console.WriteLine($"{player.Name}, what do you want to do? Press 1 for Attack, 2 for Move:");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int actionChoice) && (actionChoice == 1 || actionChoice == 2))
                {
                    if (actionChoice == 1)
                    {
                        Player target = SelectTargetPlayer(player);
                        if (target != null)
                        {
                            Attack(player, target);
                        }
                    }
                    else
                    {
                        Move(player);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter 1 for Attack or 2 for Move.");
                }
            }
            var livePlayers = players.Where(player=>player.IsAlive).ToList();
            if (livePlayers.Count == 1)
            {
                Console.WriteLine($"{livePlayers[0].Name} is the last survivor and wins the game!");
                gameRunning = false;
            }
        }
    }

    private Player SelectTargetPlayer(Player attackingPlayer)
    {
        Console.WriteLine("Select a target to attack:");
        int index = 1;
        Dictionary<int, Player> targetOptions = new Dictionary<int, Player>();
        foreach (var player in players)
        {
            if (player != attackingPlayer && player.IsAlive)
            {
                Console.WriteLine($"{index} - {player.Name}");
                targetOptions[index] = player;
                index++;
            }
        }
        int choice;
        if (int.TryParse(Console.ReadLine(), out choice) && targetOptions.ContainsKey(choice))
        {
            return targetOptions[choice];
        }
        Console.WriteLine("Invalid selection. Please try again.");
        return null;
    }

    private void Attack(Player player, Player target)
    {
        player.Attack(target);
        if (!target.IsAlive)
        {
            Console.WriteLine($"{target.Name} has been killed by {player.Name}.");
            // players.Remove(target);
        }
    }

    private void Move(Player player)
    {
        player.Move();
        Console.WriteLine("--------------- LAST MOVE ---------------");
        Console.WriteLine($"{player.Name} moved to a new position at ({player.Position.X}, {player.Position.Y}).");
        Console.WriteLine("----------------- NEXT ------------------");
    }
}


public class Player
{
    public string Name { get; }
    public int Health { get; private set; }
    public int Gold { get; private set; }
    public Vector2 Position { get; private set; }
    public Weapon? CurrentWeapon { get; private set; }
    public bool IsAlive => Health > 0;

    public Player(string name, int health, int gold)
    {
        Name = name;
        Health = health;
        Gold = gold;
        Move();
    }

    public void PurchaseWeapon(Weapon weapon)
    {
        CurrentWeapon = weapon;
        Gold -= weapon.Cost;
    }

    public void Move()
    {
        Position = new Vector2(Random.Shared.Next(0, 6), Random.Shared.Next(0, 6));
    }

    public void Attack(Player target)
    {
        if (!target.IsAlive)
        {
            Console.WriteLine("Target is already dead.");
            return;
        }
        if (CurrentWeapon == null || Vector2.Distance(Position, target.Position) > CurrentWeapon.Range)
        {
            Console.WriteLine($"{target.Name} is out of range.");
            return;
        }
        target.Health -= CurrentWeapon.DamagePower;
        if (target.Health <= 0)
        {
            target.Health = 0;
            target.CurrentWeapon = null; // Disarm the defeated player
            // Console.WriteLine($"{target.Name} has been killed.");
        }
        else
        {
            Console.WriteLine($"{target.Name} is attacked. Remaining health: {target.Health}.");
        }
    }
}

public class Weapon
{
    public string Name { get; }
    public int DamagePower { get; }
    public int Range { get; }
    public int Cost { get; }

    public Weapon(string name, int damagePower, int range, int cost)
    {
        Name = name;
        DamagePower = damagePower;
        Range = range;
        Cost = cost;
    }
}
