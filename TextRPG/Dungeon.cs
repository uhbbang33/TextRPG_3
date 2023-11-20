using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    class Record
    {
        public int gold = 0;
        public int hp = 0;
        public int exp = 0;
        public int maxExp = 0;
        public int lv = 0;

        public void Save(Player player)
        {
            gold = player.Gold;
            maxExp = player.MaxExp;
            exp = player.Exp;
            hp = player.Hp;
            lv = player.Lv;
        }
    }

    class Reward
    {
        int _gold = 0;
        public int Gold { get { return _gold; } }

        int _exp = 0;
        public int Exp { get; }
        
        List<Item> _Items;
        public List<Item> Items { get { return _Items; } }


        public Reward()
        {
            _Items = new List<Item>();
        }

        public void AddReward(int gold, int exp, List<Item> items)
        {
            _gold += gold;
            _exp += exp;
            foreach(var item in items)
            {
                _Items.Add(item);
            }
        }
        public void AddReward(int gold, int exp, Item item)
        {
            _gold += gold;
            _exp += exp;
            _Items.Add(item);
        }
    }

    internal class Dungeon
    {
        static Random Random;
        public Record beforeRecord;
        public Record afterRecord;
        Reward _reward;
        public Reward Reward { get { return _reward; } }

        List<Monster> _monsters;
        Queue<Monster> _monsterOrder;

        public enum EDungeoState { PlayerTurn , MonsterTurn, PlayerDeath, MonsterAllDeath, Clear };
        public EDungeoState state;

        enum EDifficulty { Easy , Normal, Hard, Hell };
        EDifficulty _difficulty;

        string _name;
        public string Name { get { return _name; } }

        int _selectedSkillIndex = 0;

        Player _player;
        Monster _targetMonster;

        public Dungeon(string name, int difficulty, int def, int exp)
        {
            Random = new Random();
            beforeRecord = new Record();
            afterRecord = new Record();

            state = EDungeoState.PlayerTurn;
            _name = name;

            _difficulty = (EDifficulty)difficulty;

            _monsters = new List<Monster>();
            _monsterOrder = new Queue<Monster>();
            _reward = new Reward();

            // 랜덤한 몬스터 수 결정
            //int monsterCount = Random.Next(1, 5);
            int monsterCount;
            if (difficulty == 2) monsterCount = 1;
            else monsterCount = Random.Next(1, 5);

            for (int i = 0; i < monsterCount; ++i)
            {
                CreateMonster(difficulty);
                _reward.AddReward(_monsters[i].GoldReward, _monsters[i].Lv, _monsters[i].ItemReward);
            }
        }

        void CreateMonster(int difficulty)
        {
            int MID;
            if (difficulty == 0)
            {
                MID = Random.Next(0, 2);
                switch (MID)
                {
                    case 0:
                        _monsters.Add(new Spider());
                        break;
                    case 1:
                        _monsters.Add(new Bat());
                        break;
                }
            }
            else if(difficulty == 1)
            {
                MID = Random.Next(0, 2);
                switch (MID)
                {
                    case 0:
                        _monsters.Add(new Frog());
                        break;
                    case 1:
                        _monsters.Add(new Bat());
                        break;
                }
            }
            else if(difficulty == 2)
            {
                MID = Random.Next(0, 3);
                switch (MID)
                {
                    case 0:
                        _monsters.Add(new Dragon());
                        break;
                    case 1:
                        _monsters.Add(new Gryphon());
                        break;
                    case 2:
                        _monsters.Add(new Unicorn());
                        break;
                }
            }
        }

        public void Enter(Player player)
        {
            if(_player == null)
                _player = player;            
            beforeRecord.Save(_player);
        }

        public EDungeoState Progress(out string[] msg)
        {
            int dmg = 0;
            msg = null;

            switch (state)
            {
                case EDungeoState.PlayerTurn:
                    bool bCrit = false;
                    dmg = Attack(out bCrit);
                    if(CheckTargetMonsterIsAlive() == false)
                    {
                        /* Reward. Add . Monster's */
                        // Reward - Gold. Exp, Items
                        //_reward.AddReward(10, 2, new Item("더미", "0:0", "des", Item.EType.Weapon, 10));
                        // Reward.Add(_targetMonster.DropReward());
                    }
                    msg = MakeMessage(_player, _targetMonster, dmg, bCrit);            
                    break;

                case EDungeoState.MonsterAllDeath:
                    msg = new string[] { "모든 몬스터를 쓰러트렸습니다.", "", "보상을 획득합니다." };
                    state = EDungeoState.Clear;
                    break;

                case EDungeoState.MonsterTurn:
                    Monster monster = _monsterOrder.Dequeue();
                    dmg = monster.Attack(_player);
                    msg = MakeMessage(monster, _player, dmg);

                    if (CheckPlayerIsAlive())
                    {
                        if (_monsterOrder.Count == 0)
                        {
                            state = EDungeoState.PlayerTurn;
                        }
                    }
                    break;
            }

            return state;
        }

        string[] MakeMessage(Player attacker, Monster deffender, int dmg, bool bCrit)
        {
            string[] msg = new string[3];
            string skillName = attacker.Skills[_selectedSkillIndex].name;
            msg[0] = $"{attacker.Class} 의 {skillName}!!";

            if (dmg == 0) msg[2] = "빗나갔습니다.";
            else msg[2] = $"{deffender.Name} 을(를) 맞췄습니다. [ 데미지 : {dmg} ]";

            if (bCrit) msg[1] = "치명적인 일격!!";

            return msg;
        }

        bool CheckPlayerIsAlive()
        {
            if (_player.Hp <= 0)
            {
                state = EDungeoState.PlayerDeath;
                return false;
            }
            return true;
        }

        bool CheckTargetMonsterIsAlive()
        {
            return _targetMonster.Hp > 0 ? true : false;
        }

        string[] MakeMessage(Monster attacker, Player deffender, int dmg)
        {
            string[] msg = new string[3];
            msg[0] = $"{attacker.Name} 의 공격!!";

            if (dmg == 0) msg[2] = "빗나갔습니다.";
            else msg[2] = $"{deffender.Class} 을(를) 맞췄습니다. [ 데미지 : {dmg} ]";

            return msg;
        }

        bool SetMonsterOrder()
        {
            foreach(var monster in _monsters)
            {
                if (monster.Hp > 0)
                    _monsterOrder.Enqueue(monster);
            }
            return _monsterOrder.Count > 0 ? true : false;
        }

        public void SelectSkill(int index)
        {
            _selectedSkillIndex = index;
        }

        public bool SelectMonster(int index)
        {
            if (_monsters[index].Hp <= 0)
            {
                return false;
            }
            _targetMonster = _monsters[index];
            return true;
        }

        int Attack(out bool bCrit)
        {
            int dmg = _player.Attack(_selectedSkillIndex, _targetMonster, out bCrit);
            
            if(SetMonsterOrder())
            {
                state = EDungeoState.MonsterTurn;
            }
            else
            {
                state = EDungeoState.MonsterAllDeath;
            }
            
            return dmg;
        }

        public Monster[] GetMonster()
        {
            return _monsters.ToArray();
        }
    }
}
