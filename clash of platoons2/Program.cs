using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clash_of_platoons2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Battlefield battlefield = new Battlefield();

            battlefield.Fight();
        }
    }

    class Battlefield
    {
        private Platoon _platoon1;
        private Platoon _platoon2;
        private Random _random;

        public Battlefield()
        {
            _random = new Random();
            _platoon1 = CreatePlatoon(6, "1");
            _platoon2 = CreatePlatoon(6, "2");
        }

        public void Fight()
        {
            bool isFinish = false;
            Platoon winningPlatoon = null;
            Platoon attackPlatoon = _platoon1;
            Platoon defendingPlatoon = _platoon2;
            
            while(isFinish == false)
            {
                Attack(attackPlatoon, defendingPlatoon);
                defendingPlatoon.RemoveDeadWarriors();

                if (defendingPlatoon.IsNoWarriorsLeft())
                {
                    winningPlatoon = attackPlatoon;
                    isFinish = true;
                }
                else
                {
                    Platoon value = attackPlatoon;
                    attackPlatoon = defendingPlatoon;
                    defendingPlatoon = value;
                }
            }

            Console.WriteLine($"\nВзвод {winningPlatoon.Name} одержал победу!");
            Console.ReadKey();
        }

        public void Attack(Platoon attackplatoon, Platoon defendingPlatoon)
        {
            Warrior loyalWarrior = attackplatoon.GetWarrior();
            Warrior enemyWarrior = defendingPlatoon.GetWarrior();

            Console.WriteLine($"\nВзвод {attackplatoon.Name} атакует взвод {defendingPlatoon.Name}:");
            Console.Write($"боец {loyalWarrior.Number}.{loyalWarrior.Name} стреляет по вражескому {enemyWarrior.Number}.{enemyWarrior.Name} ");

            loyalWarrior.Shoot(enemyWarrior);
        }

        private Platoon CreatePlatoon(int warriorsCount, string name)
        {
            List<Warrior> warriors = new List<Warrior>(warriorsCount);
            int maxGunnerHeal = 600;
            int minGunnerHeal = 300;
            int maxSniperHeal = 200;
            int minSniperHeal = 80;
            int maxGunnerDamage = 25;
            int minGunnerDamage = 8;
            int maxSniperDamage = 140;
            int minSniperDamage = 80;
            int gunnerAccuracy = 60;
            int sniperAccuracy = 85;
            string gunnerName = "Пулеметчик";
            string sniperName = "Снайпер";
            int maxGunnerShotsCount = 12;
            int minGunnerShotsCount = 5;
            int sniperCritChanse = 35;
            int sniperCritMultiplier = 2;

            for (int i = 0; i < warriorsCount; i++)
            {
                int gunnerHeal = _random.Next(minGunnerHeal, maxGunnerHeal + 1);
                int gunnerDamage = _random.Next(minGunnerDamage, maxGunnerDamage + 1);
                int gunnerShotsCount = _random.Next(minGunnerShotsCount, maxGunnerShotsCount + 1);
                int sniperHeal = _random.Next(minSniperHeal, maxSniperHeal + 1);
                int sniperDamage = _random.Next(minSniperDamage, maxSniperDamage + 1);
                string number = Convert.ToString(i + 1);
                Warrior[] warriorVariants = new Warrior[2] { new Gunner(gunnerHeal, gunnerDamage, gunnerAccuracy, gunnerName, number, _random, gunnerShotsCount), new Sniper(sniperHeal, sniperDamage, sniperAccuracy, sniperName, number, _random, sniperCritChanse, sniperCritMultiplier) };

                warriors.Add(warriorVariants[_random.Next(warriorVariants.Length)]);
            }

            return new Platoon(warriors, name, _random);
        }
    }

    class Platoon
    {
        private List<Warrior> _warriors;
        private Random _random;
        public string Name { get; private set; }

        public Platoon(List<Warrior> warriors, string name, Random random)
        {
            _warriors = warriors;
            _random = random;
            Name = name;
        }

        public Warrior GetWarrior()
        {
            return _warriors[_random.Next(_warriors.Count)];
        }
        
        public bool IsNoWarriorsLeft()
        {
            return _warriors.Count == 0;
        }

        public void RemoveDeadWarriors()
        {
            int index = 0;

            while(index < _warriors.Count)
            {
                if (_warriors[index].Health <= 0)
                    _warriors.RemoveAt(index);
                else
                    index++;
            }
        }
    }

    abstract class Warrior
    {
        protected int Damage;
        protected int Accuracy;
        protected Random Random;
        public string Name { get; private set; }
        public string Number { get; private set; }
        public int Health { get; private set; }

        public Warrior(int health, int damage, int accuracy, string name, string number, Random random)
        {
            Health = health;
            Damage = damage;
            Accuracy = accuracy;
            Name = name;
            Number = number;
            Random = random;
        }

        public abstract void Shoot(Warrior enemyWarrior);

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;

            if(Health > 0)
            {
                Console.WriteLine($"Вражеский {Name} теряет {damage} жизней, осталось {Health}");
            }
            else
            {
                Console.WriteLine($"Вражеский {Name} погибает");
            }
        }
    }

    class Gunner : Warrior
    {
        private int _shotsCount;

        public Gunner(int health, int damage, int accuracy, string name, string number, Random random, int shotsCount) : base (health, damage, accuracy, name, number, random)
        {
            _shotsCount = shotsCount;
        }

        public override void Shoot(Warrior enemyWarrior)
        {
            int totalDamage = 0;

            for(int i = 0; i < _shotsCount; i++)
            {
                if (Random.Next(101) <= Accuracy)
                    totalDamage += Damage;
            }

            if(totalDamage > 0)
            {
                Console.WriteLine($"и наносит {totalDamage} урона");

                enemyWarrior.TakeDamage(totalDamage);
            }
            else
            {
                Console.WriteLine("и промахивается");
            }
        }
    }

    class Sniper : Warrior
    {
        private int _critChance;
        private int _critMultiplier;

        public Sniper(int health, int damage, int accuracy, string name, string number, Random random, int critChance, int critMultiplier) : base(health, damage, accuracy, name, number, random)
        {
            _critChance = critChance;
            _critMultiplier = critMultiplier;
        }

        public override void Shoot(Warrior enemyWarrior)
        {
            

            if (Random.Next(101) < Accuracy)
            {
                int damage = Damage;

                if (Random.Next(101) < _critChance)
                {
                    damage *= _critMultiplier;
                    Console.WriteLine($"и наносит повышенный урон: {damage}");
                }
                else
                {
                    Console.WriteLine($"и наносит {damage} урона");
                }

                enemyWarrior.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine("и промахивается");
            }
        }
    }
}
