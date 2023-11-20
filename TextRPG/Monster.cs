using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public class Monster
    {
        string _name;
        public string Name { get { return _name; }  set { _name = value; } }

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

        public Monster()
        {
        }

        public int TakeDamage(int damage, float accuracy)
        {
            float totalAccuracy = accuracy * (1 - _evasion);
            int dmg = damage - Def;
            if (dmg < 0) dmg = 1;

            Random random = new Random();
            if(random.NextDouble() < totalAccuracy)
            {
                _hp -= dmg;
                if(_hp < 0)
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

    class Bat : Monster
    {
        public Bat()
        {
            Name = "박쥐";
            _maxHp = 10;
            _hp = MaxHp;
            _atk = 10;
            _def = 1;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\bat.txt");
        }
    }

    class Unicorn : Monster
    {
        public Unicorn()
        {
            Name = "유니콘";
            _maxHp = 30;
            _hp = MaxHp;
            _atk = 10;
            _def = 30;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\unicorn.txt");
        }
    }

    class Aardvark : Monster
    {
        public Aardvark()
        {
            Name = "땅돼지";
            _maxHp = 25;
            _hp = MaxHp;
            _atk = 10;
            _def = 30;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\aardvark.txt");
        }
    }

    class Dragon : Monster
    {
        public Dragon()
        {
            Name = "드래곤";
            _maxHp = 100;
            _hp = MaxHp;
            _atk = 10;
            _def = 30;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\dragon.txt");
        }
    }

    class Centaurs : Monster
    {
        public Centaurs()
        {
            Name = "켄타우로스";
            _maxHp = 50;
            _hp = MaxHp;
            _atk = 10;
            _def = 30;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\centaurs.txt");
        }
    }

    class Gryphon : Monster
    {
        public Gryphon()
        {
            Name = "그리폰";
            _maxHp = 75;
            _hp = MaxHp;
            _atk = 10;
            _def = 30;
            _goldReward = 50;
            _display = File.ReadAllLines(@"..\..\..\MonstersArt\gryphon.txt");
        }
    }

    //class Monsters
    //{
    //    List<Monster> _monsterList = new List<Monster>();
    //    public List<Monster> MonsterList { get { return _monsterList; } }

    //    public Monsters()
    //    {
    //        JObject monsterObj = Loader.LoadMonsterData();
    //        _monsterList = monsterObj["Monster"].ToObject<List<Monster>>();
    //    }
    //}

}
