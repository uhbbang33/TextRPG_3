namespace TextRPG
{
    public class Monster
    {
        string _name;
        public string Name { get { return _name; } set { _name = value; } }

        protected int _hp;
        public int Hp { get { return _hp; } }

        protected int _maxHp;
        public int MaxHp { get { return _maxHp; } }

        protected int _atk;
        public int Atk { get { return _atk; } }

        protected int _def;
        public int Def { get { return _def; } }

        protected int _goldReward;
        public int GoldReward { get { return _goldReward; } }

        protected Item _itemReward;
        public Item? ItemReward { get { return _itemReward; } }

        protected bool _isDead = false;
        public bool IsDead { get { return _isDead; } }

        protected string[] _display;
        public string[] Display { get { return _display; } }

        protected float _evasion;

        protected int _lv;
        public int Lv { get { return _lv; } }

        protected static Random random;

        public Monster()
        {
            random = new Random();
        }

        public int TakeDamage(int damage, float accuracy)
        {
            float totalAccuracy = accuracy * (1 - _evasion);
            int dmg = damage - Def;
            if (dmg < 0) dmg = 1;

            Random random = new Random();
            if (random.NextDouble() < totalAccuracy)
            {
                _hp -= dmg;
                if (_hp < 0)
                {
                    _hp = 0;
                    _isDead = true;
                }
            }
            else
            {
                dmg = 0;
            }
            return dmg;
        }

        // Attack
        public int Attack(Player player)
        {
            int dmg = player.TakeDamage(Atk);
            return dmg;
        }
    }

    class Spider : Monster
    {
        public Spider()
        {
            Name = "거미";
            _lv = random.Next(1, 6);
            _maxHp = 5 + _lv;
            _hp = MaxHp;
            _atk = 5 + _lv;
            _def = 5 + _lv;
            _goldReward = 5 + _lv;
            _itemReward = new Item("거미줄", "0:0", "아무 능력도 없는 거미줄", Item.ClassRestricted.NoRestricted, Item.EType.Potion, 5);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\spider.txt");
        }
    }

    class Bat : Monster
    {
        public Bat()
        {
            Name = "박쥐";
            _lv = random.Next(5, 11);
            _maxHp = 10 + (int)(_lv * 1.2f);
            _hp = MaxHp;
            _atk = 7 + (int)(_lv * 1.2f);
            _def = 1 + (int)(_lv * 1.2f);
            _goldReward = 10 + _lv;
            _itemReward = new Item("박쥐날개", "0:1", "팔면 쏠쏠하다", Item.ClassRestricted.NoRestricted, Item.EType.Potion, 15);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\bat.txt");
        }
    }

    class Frog : Monster
    {
        public Frog()
        {
            Name = "독개구리";
            _lv = random.Next(10, 16);
            _maxHp = 20 + (int)(_lv * 1.5f);
            _hp = MaxHp;
            _atk = 15 + (int)(_lv * 1.5f);
            _def = 20 + (int)(_lv * 1.5f);
            _goldReward = 20 + _lv;
            _itemReward = new Item("독개구리의 독", "0:-10", "먹지 말고 팔자", Item.ClassRestricted.NoRestricted,  Item.EType.Potion, 25);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\frog.txt");
        }
    }

    class Gryphon : Monster
    {
        public Gryphon()
        {
            Name = "그리폰";
            _lv = random.Next(20, 26);
            _maxHp = 70 + (int)(_lv * 1.2f);
            _hp = MaxHp;
            _atk = 30 + (int)(_lv * 1.2f);
            _def = 30 + (int)(_lv * 1.2f);
            _goldReward = 50 + (int)(_lv * 1.2f);
            _itemReward = new Item("그리폰 부리", "1:10", "쪼이면 아프다", Item.ClassRestricted.NoRestricted, Item.EType.Armor, 50);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\gryphon.txt");
        }
    }

    class Unicorn : Monster
    {
        public Unicorn()
        {
            Name = "유니콘";
            _lv = random.Next(30, 36);
            _maxHp = 60 + (int)(_lv * 1.4f);
            _hp = MaxHp;
            _atk = 35 + (int)(_lv * 1.4f);
            _def = 40 + (int)(_lv * 1.4f);
            _goldReward = 60 + (int)(_lv * 1.4f);
            _itemReward = new Item("유니콘 뿔", "2:13", "관상용으로도 추천", Item.ClassRestricted.Wizard, Item.EType.Weapon, 60);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\unicorn.txt");
        }
    }

    class Dragon : Monster
    {
        public Dragon()
        {
            Name = "드래곤";
            _lv = random.Next(40, 51);
            _maxHp = 100 + (int)(_lv * 1.5f);
            _hp = MaxHp;
            _atk = 60 + (int)(_lv * 1.5f);
            _def = 70 + (int)(_lv * 1.5f);
            _goldReward = 100 + (int)(_lv * 1.5f);
            _itemReward = new Item("드래곤 비늘", "0:100", "비싸다", Item.ClassRestricted.NoRestricted, Item.EType.Potion, 100);
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\dragon.txt");
        }
    }
}
