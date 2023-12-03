using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the Text RPG!");

        // Character creation
        int playerHealth = 100;
        int playerDamage = 10;
        int playerCoins = 2;
        int playerStagger = 0; // Initialize to 0

        int enemyHealth = 80;
        int enemyDamage = 8;
        int enemyCoins = 2;
        int enemyStagger = 0; // Initialize to 0

        // Game loop
        while (playerHealth > 0 && enemyHealth > 0)
        {
            // Calculate stagger resistance
            int playerStaggerResistance = playerHealth / 2 + 20;
            int enemyStaggerResistance = enemyHealth / 2 + 20;

            // Player's turn
            Console.WriteLine("\nPlayer's turn:");

            // Check if player is staggered
            if (playerStagger <= playerStaggerResistance && playerStagger > 0)
            {
                Console.WriteLine("[Staggered] You are staggered! Your turn is skipped.");
                playerStagger = 0; // Reset stagger
                continue; // Skip the rest of the player's turn
            }
            else
            {

                Console.WriteLine("1. Attack");
                Console.WriteLine("2. Defend");

                int choice = GetPlayerChoice();
                if (choice == 1)
                {
                    // Player attacks
                    int playerCoinValue = coinFlip();
                    Console.WriteLine($"[Player has: {coinFlipResult(playerCoinValue)}]");

                    int enemyCoinValue = coinFlip();
                    Console.WriteLine($"[Enemy has: {coinFlipResult(enemyCoinValue)}]");

                    int playerPower = AttackPowerValue(playerCoinValue, playerDamage);
                    int enemyPower = AttackPowerValue(enemyCoinValue, enemyDamage);

                    Clash result = Winner(playerPower, enemyPower);

                    EndClash(result, ref playerCoins, ref enemyCoins, ref playerHealth, ref enemyHealth, ref playerStagger, ref enemyStagger);
                    PrintHealth("Enemy", enemyHealth, enemyStagger);

                }
                else if (choice == 2)
                {
                    int playerCoinValue = coinFlip();
                    Console.WriteLine($"[Player has: {coinFlipResult(playerCoinValue)}]");

                    int enemyCoinValue = coinFlip();
                    Console.WriteLine($"[Enemy has: {coinFlipResult(enemyCoinValue)}]");

                    int enemyPower = AttackPowerValue(enemyCoinValue, enemyDamage);

                    Clash result = Winner(playerCoins, enemyCoins);

                    EndClash(result, ref playerCoins, ref enemyCoins, ref playerHealth, ref enemyHealth, ref playerStagger, ref enemyStagger);
                    PrintHealth("Enemy", enemyHealth, enemyStagger);

                    continue;
                }
            }

            // Enemy's turn
            if (enemyHealth > 0)
            {
                Console.WriteLine("\nEnemy's turn:");

                // Check if enemy is staggered
                if (enemyStagger <= enemyStaggerResistance && enemyStagger > 0)
                {
                    Console.WriteLine("[Staggered] Enemy is staggered! Their turn is skipped.");
                    enemyStagger = 0; // Reset stagger
                    continue; // Skip the rest of the enemy's turn
                }
                else
                {
                    // Enemy attacks
                    int enemyCoinValue = coinFlip();
                    Console.WriteLine($"[Enemy has: {coinFlipResult(enemyCoinValue)}]");

                    int playerCoinValue = coinFlip();
                    Console.WriteLine($"[Player has: {coinFlipResult(playerCoinValue)}]");

                    int enemyPower = AttackPowerValue(enemyCoinValue, enemyDamage);
                    int playerPower = AttackPowerValue(playerCoinValue, playerDamage);

                    Clash result = Winner(enemyPower, playerPower);

                    EndClash(result, ref enemyCoins, ref playerCoins, ref enemyHealth, ref playerHealth, ref enemyStagger, ref playerStagger);
                    PrintHealth("Player", playerHealth, playerStagger);
                }
            }
        }

        // Determine the winner
        if (playerHealth > 0)
        {
            Console.WriteLine("\nCongratulations! You defeated the enemy!");
        }
        else
        {
            Console.WriteLine("\nGame over. Better luck next time!");
        }

        // Function to get the player's choice (1 for Attack, 2 for Defend)
        static int GetPlayerChoice()
        {
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || (choice != 1 && choice != 2))
            {
                Console.WriteLine("Invalid choice. Enter 1 for Attack or 2 for Defend:");
            }
            return choice;
        }

        static int coinFlip()
        {
            return new Random().Next(0, 2);
        }

        static string coinFlipResult(int result)
        {
            return result == 0 ? "Heads" : "Tails";
        }

        static int AttackPowerValue(int coinOutput, int basePower)
        {
            return coinOutput == 0 ? basePower + 2 : basePower;
        } }

        enum Clash
    {
        PlayerWins,
        EnemyWins,
        Tie
    }

    static Clash Winner(int playerPower, int enemyPower)
    {
        if (playerPower > enemyPower)
        {
            return Clash.PlayerWins;
        }
        else if (playerPower < enemyPower)
        {
            return Clash.EnemyWins;
        }
        else
        {
            return Clash.Tie;
        }
    }

    static void EndClash(Clash result, ref int attackerCoins, ref int defenderCoins, ref int attackerHealth, ref int defenderHealth, ref int attackerStagger, ref int defenderStagger)
    {
        if (result == Clash.PlayerWins)
        {
            // Player wins
            defenderCoins--;
            if (defenderCoins < 0) defenderCoins = 0;
            int damage = attackerCoins + 1; // Damage is based on the number of coins of the attacker
            AttackEnemy(damage, ref defenderHealth, ref defenderStagger);
        }
        else if (result == Clash.EnemyWins)
        {
            // Enemy wins
            attackerCoins--;
            if (attackerCoins < 0) attackerCoins = 0;
            int damage = defenderCoins + 1; // Damage is based on the number of coins of the defender
            AttackPlayer(damage, ref attackerHealth, ref attackerStagger);
        }
        else
        {
            // Tie
            Console.WriteLine("Tie! Reflipping coins...");
            int newResult = coinFlip();
            Console.WriteLine($"[Tiebreaker: {coinFlipResult(newResult)}]");
            Clash newClashResult = Winner(attackerCoins, defenderCoins);
            EndClash(newClashResult, ref attackerCoins, ref defenderCoins, ref attackerHealth, ref defenderHealth, ref attackerStagger, ref defenderStagger);
        }
    }

    private static object coinFlipResult(int newResult)
    {
        throw new NotImplementedException();
    }

    private static int coinFlip()
    {
        throw new NotImplementedException();
    }

    static void AttackEnemy(int damage, ref int enemyHealth, ref int enemyStagger)
    {
        if (enemyStagger > 0)
        {
            // If enemy is staggered, deal double damage
            enemyHealth -= 2 * damage;
            Console.WriteLine($"You dealt {2 * damage} damage to the enemy!");

            if (enemyHealth <= enemyStagger)
            {
                Console.WriteLine("[Staggered] Enemy is staggered!");
            }
        }
        else
        {
            // Split damage between stagger and health
            int staggerDamage = damage * 3 / 4;
            int healthDamage = damage - staggerDamage;

            enemyHealth -= staggerDamage;
            enemyStagger -= staggerDamage;

            Console.WriteLine($"You dealt {staggerDamage} stagger damage and {healthDamage} health damage to the enemy!");
        }
    }

    static void AttackPlayer(int damage, ref int playerHealth, ref int playerStagger)
    {
        if (playerStagger > 0)
        {
            // If player is staggered, take double damage
            playerHealth -= 2 * damage;
            Console.WriteLine($"Enemy dealt {2 * damage} damage to you!");

            if (playerHealth <= playerStagger)
            {
                Console.WriteLine("[Staggered] You are staggered!");
            }
        }
        else
        {
            // Split damage between stagger and health
            int staggerDamage = damage * 3 / 4;
            int healthDamage = damage - staggerDamage;

            playerHealth -= staggerDamage;
            playerStagger -= staggerDamage;

            Console.WriteLine($"Enemy dealt {staggerDamage} stagger damage and {healthDamage} health damage to you!");
        }
    }

    static void PrintHealth(string name, int health, int stagger)
    {
        if (stagger > 0)
        {
            Console.WriteLine($"{name}'s health: {health}, Stagger: {stagger} [Staggered]");
        }
        else
        {
            Console.WriteLine($"{name}'s health: {health}");
        }
    }
}

