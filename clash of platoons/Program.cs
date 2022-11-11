using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clash_of_platoons
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Warrior> warriors = new List<Warrior>();
            int warriorsCount = 5;

            for(int i = 0; i < warriorsCount; i++)
            {
                warriors.Add(new Warrior(300, 20, 80, 80, 5, 20, i + 1));
            }

            Battlefield battlefield = new Battlefield(new Platoon(warriors, "1"), new Platoon(warriors, "2"));

            battlefield.Fight();
        }
    }

    class Battlefield
    {
        private Platoon _platoon1;
        private Platoon _platoon2;

        public Battlefield(Platoon platoon1, Platoon platoon2)
        {
            _platoon1 = platoon1;
            _platoon2 = platoon2;
        }

        public void Fight()
        {
            Platoon winningPlatoon = null;
            Random random = new Random();
            int probabilityPercentage = 50;
            bool isOnePlatoonLeft = false;

            while(isOnePlatoonLeft == false)
            {
                if (random.Next(0, 101) <= probabilityPercentage)
                {
                    _platoon1.Attack(_platoon2, random);

                    if(_platoon2.CheckWarriorsAvailability() == false)
                    {
                        winningPlatoon = _platoon1;
                        isOnePlatoonLeft = true;
                    }
                }
                else
                {
                    _platoon2.Attack(_platoon1, random);

                    if (_platoon1.CheckWarriorsAvailability() == false)
                    {
                        winningPlatoon = _platoon2;
                        isOnePlatoonLeft = true;
                    }
                }
            }

            Console.Write($"\nВзвод {winningPlatoon.Number} одержал победу!");
            Console.ReadKey();
        }
    }

    class Platoon
    {
        private List<Warrior> _warriors;
        public string Number { get; private set; }

        public Platoon(List<Warrior> warriors, string number)
        {
            _warriors = warriors;
            Number = number;
        }

        public void Attack(Platoon enemyPlatoon, Random random)
        {
            int index = random.Next(_warriors.Count - 1);

            Console.Write($"{Number} взвод:\nсолдат {_warriors[index].Number} стреляет");

            _warriors[index].Shoot(enemyPlatoon, random);
        }

        public void GetDamage(int damage, Random random)
        {
            int index = random.Next(_warriors.Count - 1);

            Console.Write($"{Number} взвод:\nсолдат {_warriors[index].Number}");

            if (_warriors[index].GetDamage(damage, random))
                _warriors.Remove(_warriors[index]);
        }

        public bool CheckWarriorsAvailability()
        {
            return _warriors.Count > 0;
        }
    }

    class Warrior
    {
        private int _health;
        private int _maxDamage;
        private int _minDamage;
        private int _shootingAccuracy;
        private int _minShieldProtection;
        private int _maxShieldProtection;
        public int Number { get; private set; }

        public Warrior(int health, int minDamage, int maxDamage, int shootingAccuracy, int minShieldProtection, int maxShieldProtection, int number)
        {
            _health = health;
            _maxDamage = maxDamage;
            _minDamage = minDamage;
            _shootingAccuracy = shootingAccuracy;
            _minShieldProtection = minShieldProtection;
            _maxShieldProtection = maxShieldProtection;
            Number = number;
        }

        public void Shoot(Platoon enemyPlatoon, Random random)
        {
            int damage = random.Next(_minDamage, _maxDamage + 1);

            if (random.Next(0, 101) <= _shootingAccuracy)
            {
                Console.WriteLine($" и наносит {damage} урона");

                enemyPlatoon.GetDamage(damage, random);
            }
            else
            {
                Console.WriteLine(" и промахивается");
            }
        }

        public bool GetDamage(int damage, Random random)
        {
            int value = (int)((float)damage * (1 - ((float)random.Next(_minShieldProtection, _maxShieldProtection + 1) / 100)));
            bool isDead = false;

            if (value > _health)
                value = _health;

            Console.Write($" теряет {value} здоровья (щит поглотил {damage - value} урона)");

            _health -= value;

            if(_health > 0)
            {
                Console.WriteLine($", осталось {_health}");
            }
            else
            {
                Console.WriteLine(" и умирает");

                isDead = true;
            }

            return isDead;
        }
    }
}
