using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Homework1_6_11
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            Room room = new Room();
            room.Work();
        }
    }

    abstract class HavingFishes
    {
        protected readonly bool IsInWater;
        protected List<Fish> Fishes;
        protected string Name;

        public HavingFishes(bool isInWater, string name)
        {
            IsInWater = isInWater;
            Name = name;
        }

        public void DoIteration()
        {
            foreach (var fish in Fishes)
            {
                fish.DoLiveIteration();
            }
        }

        public void ShowFishesInfo(int cursorLeft)
        {
            Console.CursorLeft = cursorLeft;
            Console.WriteLine(Name + ":");

            foreach (var fish in Fishes)
            {
                fish.ShowInfo(cursorLeft);
            }
        }

        public void ClearDiedFishes()
        {
            for (int i = 0; i < Fishes.Count; i++)
            {
                if (Fishes[i].IsAlive() == false)
                {
                    Fishes.Remove(Fishes[i]);
                    i--;
                }
            }
        }

        public bool HaveFish()
        {
            return Fishes.Count > 0;
        }

        public virtual void AddFish(Fish fish)
        {
            fish.Move(IsInWater);
            Fishes.Add(fish);
        }

        public virtual bool TryGetFish(out Fish addedFish) 
        {
            Console.Write("Введите номер рыбки, которую хотите вытащить: ");

            if (int.TryParse(Console.ReadLine(), out int userInput))
            {
                if(TryGetFishByNumber(userInput, out addedFish))
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ввода");
            }

            addedFish = null;
            return false;
        }

        private bool TryGetFishByNumber(int number, out Fish addedFish)
        {
            for (int i=0; i<Fishes.Count; i++)
            {
                if (Fishes[i].Number == number)
                {
                    addedFish = Fishes[i];
                    Fishes.Remove(Fishes[i]);
                    return true;
                }
            }

            Console.WriteLine("Рыбки с таким номером нет");
            addedFish = null;
            return false;
        }
    }

    class Room: HavingFishes
    {
        private const bool IsFishInWater = true;
        private const int NecessaryFishVolume = 6400;

        private Box _box;
        private Aquarium _aquarium;
        private int _lastFishNumber;
        private readonly Random _random;
        private readonly int _aquariumLength;
        private readonly int _aquariumWidth;
        private readonly int _aquariumHeight;

        public Room(): base(true, "Комната")
        {
            _random = new Random();
            _aquariumLength = 100;
            _aquariumWidth = 40;
            _aquariumHeight = 80;
            CreateFishes();
            _box = new Box();
            _aquarium = new Aquarium(_aquariumLength, _aquariumWidth, _aquariumHeight, GiveFishesToAquarium());
        }

        public void Work()
        {
            bool isWork = true;

            while (isWork)
            {
                Console.Clear();
                DoFishLive();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("1. Достать рыбу из аквариума и положить в коробку");
                Console.WriteLine("2. Добавить новую рыбу в аквариум");
                Console.WriteLine("3. Купить рыбку");
                Console.WriteLine("4. Достать рыбу из коробки и поместить в аквариум");
                Console.WriteLine("5. Закончить манипуляции с рыбками");
                Console.Write("Введите номер команды: ");

                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        ExchangeFish(_aquarium, _box);
                        break;

                    case "2":
                        ExchangeFish(this, _aquarium);
                        break;

                    case "3":
                        AddNewFish();
                        break;

                    case "4":
                        ExchangeFish(_box, _aquarium);
                        break;

                    case "5":
                        isWork = false;
                        break;

                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }

                Console.ReadKey();
            }
        }

        public override bool TryGetFish(out Fish addedFish)
        {
            int addedFishIndex = _random.Next(0, Fishes.Count);

            if (Fishes.Count > 0)
            {
                addedFish = Fishes[addedFishIndex];
                Fishes.Remove(Fishes[addedFishIndex]);
                return true;
            }

            Console.WriteLine("Рыбок больше нет");
            addedFish = null;
            return false;
        }

        public void DoFishLive() 
        {
            int cursorLeft = 70;

            _box.DoIteration();
            _aquarium.DoIteration();
            _box.ShowFishesInfo(cursorLeft);
            _aquarium.ShowFishesInfo(cursorLeft);
            _box.ClearDiedFishes();
            _aquarium.ClearDiedFishes();
        }

        private void CreateFishes() 
        {
            int minStartCountFishes = 10;
            int maxStartCountFishes = 200;

            int countFishes = _random.Next(minStartCountFishes, maxStartCountFishes + 1);
            Fishes = new List<Fish>();

            for (int i = 0; i < countFishes; i++)
            {
                AddNewFish();
            }
        }

        private void AddNewFish()
        {
            _lastFishNumber++;
            Fishes.Add(new Fish(_lastFishNumber, IsFishInWater));
        }

        private List<Fish> GiveFishesToAquarium() 
        {
            List<Fish> fishes = new List<Fish>();

            int minStartCountFishes = 1;
            int maxStartCountFishes = (_aquariumLength * _aquariumWidth * _aquariumHeight) / NecessaryFishVolume;

            if (maxStartCountFishes > Fishes.Count)
            {
                maxStartCountFishes = Fishes.Count;
            }

            int countFishes = _random.Next(minStartCountFishes, maxStartCountFishes + 1);

            for (int i = 0; i < countFishes; i++)
            {
                if (TryGetFish(out Fish addedFish))
                {
                    fishes.Add(addedFish);
                }
            }

            return fishes;
        }

        private void ExchangeFish(HavingFishes givingFish, HavingFishes gettingFish)
        {
            if (givingFish.TryGetFish(out Fish addedFish))
            {
                gettingFish.AddFish(addedFish);
            }
            else
            {
                Console.WriteLine("Нечего передавать");
            }
        }
    }

    class Box: HavingFishes
    {
        public Box(): base(false, "Коробка")
        {
            Fishes = new List<Fish>();
        }
    }

    class Aquarium: HavingFishes
    {
        private const int NecessaryFishVolume = 6400;

        private readonly int _length;
        private readonly int _width;
        private readonly int _height;
        private readonly int _maxCountFishes;

        public Aquarium (int length, int width, int height, List<Fish> fishes): base(true, "Аквариум")
        {
            _length = length;
            _width = width;
            _height = height;
            Fishes = fishes;
            _maxCountFishes = (_length * _width * _height) / NecessaryFishVolume;
        }

        public override void AddFish(Fish fish)
        {
            if (HaveSpace())
            {
                base.AddFish(fish);
            }
            else
            {
                Console.WriteLine("Места нет");
            }
        }

        private bool HaveSpace()
        {
            return _maxCountFishes > Fishes.Count;
        }
    }

    class Fish
    {
        private const int ConsenescenceRateInWater = 10;
        private const int ConsenescenceRateOutsideWater = 25;

        private int _health;
        private readonly Random _random;
        private int _countIterationInWater;
        private int _countIterationOutsideWater;
        private bool _isInWater;

        public Fish(int number, bool isInWater)
        {
            int minStartHealth = 100;
            int maxStartHealth = 200;

            Number = number;
            _isInWater = isInWater;
            _countIterationInWater = 0;
            _countIterationOutsideWater = 0;
            _random = new Random();
            _health = _random.Next(minStartHealth, maxStartHealth + 1);
        }

        public int Number { get; private set; }

        public bool IsAlive()
        {
            return _health > 0;
        }

        public void ShowInfo(int cursorLeft)
        {
            WriteLineWithLeftIndent("Рыба " + Number + ": прожила " + _countIterationInWater + " итераций в воде и " + _countIterationOutsideWater + " итераций вне воды", cursorLeft);

            if (IsAlive())
            {
                if (_health <= 30)
                {
                    WriteLineWithLeftIndent("Выглядит плохо", cursorLeft);
                }
            }
            else
            {
                WriteLineWithLeftIndent("Умерла", cursorLeft);
            }
        }

        public void Move(bool isInWater)
        {
            _isInWater = isInWater;
        }

        public void DoLiveIteration()
        {
            if (_isInWater)
            {
                DoLiveIterationBasedWater(ConsenescenceRateInWater, ref _countIterationInWater);
            }
            else
            {
                DoLiveIterationBasedWater(ConsenescenceRateOutsideWater, ref _countIterationOutsideWater);
            }
        }

        private void DoLiveIterationBasedWater(int consenescenceRate, ref int countIteration)
        {
            _health -= consenescenceRate;
            countIteration++;
        }

        private void WriteLineWithLeftIndent(string text, int indentLeft)
        {
            Console.SetCursorPosition(indentLeft, Console.CursorTop);
            Console.WriteLine(text);
        }
    }
}