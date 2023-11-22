﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    public class Item
    {
        public enum EStatus { HP, ATK, DEF, MP };
        public enum EType { Weapon, Armor, Potion  }
        string[] _statusWord = { " 체력 ", "공격력", "방어력", "마나" };

        string _name;
        public string Name { get { return _name; } set { _name = value; } }

        public enum ClassRestricted { NoRestricted, Warrior, Wizard};

        public bool bEquip;
        public EStatus _status;
        public EType type;
        public ClassRestricted restricted;
        [JsonIgnore()]
        public string Status { get { return _statusWord[(int)_status]; } }

        int _val;
        public int Value { get { return _val; } set { _val = value; } }

        string _description;
        public string Description { get { return _description; } set { _description = value; } }

        int _price;
        public int Price { get { return _price; } set { _price = value; } }

        int _hasCount;
        public int HasCount { get { return _hasCount; } set { _hasCount = value; } }

        static Item _null = new Item();
        public static Item NULL { get { return _null; } }

        public Item()
        {
            _name = "";
            _val = 0;
            _price = 0;
        }

        public Item(string name, string status, string description, ClassRestricted restricted, EType type, Int32 price, int hasCount)
        {
            string[] msg = status.Split(',');
            for (int i = 0; i < msg.Length; ++i)
            {
                string[] data = msg[i].Split(':');
                _status = (EStatus)(int.Parse(data[0]));
                _val = int.Parse(data[1]);
            }

            _name = name;
            _description = description;
            this.type = type;
            this.restricted = restricted;
            _price = price;
            _hasCount = hasCount;
        }
        public Item(string name, string status, string description, ClassRestricted restricted, EType type, Int32 price)
        {
            string[] msg = status.Split(',');
            for (int i = 0; i < msg.Length; ++i)
            {
                string[] data = msg[i].Split(':');
                _status = (EStatus)(int.Parse(data[0]));
                _val = int.Parse(data[1]);
            }

            _name = name;
            _description = description;
            this.type = type;
            this.restricted = restricted;
            _price = price;
            _hasCount = 1;
        }
    }

    //json파일에서 불러온 스킬들을 담을 클래스
    public class Skill
    {
        public string name;
        public string desc;
        public float damage;
        public float accuracy;
        public int cost;

        public Skill(string name, string desc, float damage, float accuracy, int cost)
        {
            this.name = name;
            this.desc = desc;
            this.damage = damage;
            this.accuracy = accuracy;
            this.cost = cost;
        }
    }
}
