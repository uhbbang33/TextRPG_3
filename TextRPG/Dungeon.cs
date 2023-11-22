using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TextRPG
{
    class Reward
    {
        int _gold = 0;
        public int Gold { get { return _gold; } }

        int _exp = 0;
        public int Exp { get { return _exp; } }
        
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
        Reward _reward;
        int _lvAvg = 0;

        public Reward Reward { get { return _reward; } }

        List<Monster> _monsters;
        Queue<Monster> _monsterOrder;

        public enum EDungeoState { PlayerTurn , MonsterTurn, PlayerDeath, MonsterAllDeath, Clear, GoTown };
        public EDungeoState state;

        enum EDifficulty { Easy , Normal, Hard, Hell };
        EDifficulty _difficulty;

        string _name;
        public string Name { get { return _name; } }

        int _selectedSkillIndex = 0;

        Player _player;
        Monster _targetMonster;

        List<string> _monsterNames;
        public List<string> MonsterNames {  get { return _monsterNames; } }

        public Dungeon(string name, int difficulty, int def, int exp)
        {
            Random = new Random();

            state = EDungeoState.PlayerTurn;
            _name = name;

            _difficulty = (EDifficulty)difficulty;

            _monsters = new List<Monster>();
            _monsterOrder = new Queue<Monster>();
            _reward = new Reward();
            _monsterNames = new List<string>();

            int monsterCount;
            if (difficulty == 2) monsterCount = 1;
            else monsterCount = Random.Next(1, 5);

            for (int i = 0; i < monsterCount; ++i)
            {
                CreateMonster(difficulty);
                _reward.AddReward(_monsters[i].GoldReward, _monsters[i].Lv, _monsters[i].ItemReward);
                _monsterNames.Add(_monsters[i].Name);
            }

            float sum = 0;
            foreach(var monster in _monsters)
            {
                sum += monster.Lv;
            }
            _lvAvg = (int)(sum / _monsters.Count);
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
        }

        public EDungeoState Progress(out string[] msg)
        {
            int dmg = 0;
            msg = new string[] { $"{state.ToString()}", "What", "Problem" };

            switch (state)
            {
                case EDungeoState.PlayerTurn:
                    bool bCrit = false;
                    dmg = Attack(out bCrit);
                    if(CheckTargetMonsterIsAlive() == false)
                    {
                        // is monster die ? so, Add Reward from monster
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
                    else
                    {
                        state = EDungeoState.PlayerDeath;
                    }
                    break;

                case EDungeoState.PlayerDeath:
                    msg = new string[] { "플레이어가 사망했습니다.", "", "마을로 돌아갑니다." };
                    state = EDungeoState.GoTown;
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

        public bool SelectSkill(int index)
        {
            if(_player.MpCheck(index))
            {
                _selectedSkillIndex = index;
                return true;
            }
            else
            {
                return false;
            }
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
            if (dmg == -1) return -1;

            if (SetMonsterOrder())
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

        public bool RunAway()
        {
            int lvDif = _player.Lv - _lvAvg;
            float percent = 0.5f + (float)(lvDif / 10);
            if (Random.NextDouble() < percent) return true;
            else
            {
                state = EDungeoState.MonsterTurn;
                SetMonsterOrder();
                return false;
            }            
        }

        public void PassPlayerTurn()
        {
            state = Dungeon.EDungeoState.MonsterTurn;
            SetMonsterOrder();
        }
    }
}
