using System;
using System.Collections.Generic;
using System.Linq;
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

    internal class Dungeon
    {
        static Random Random;
        public Record beforeRecord;
        public Record afterRecord;
        
        List<MyMonster> _monsters;
        Queue<MyMonster> _monsterOrder;

        public enum EDungunState { PlayerTurn , MonsterTurn, PlayerDeath, MonsterAllDeath, Clear };
        public EDungunState state;

        enum EDifficulty { Easy , Normal, Hard, Hell };
        EDifficulty _difficulty;

        string _name;
        public string Name { get { return _name; } }

        int _selectedSkillIndex = 0;

        Player _player;
        MyMonster _targetMonster;

        public Dungeon(string name, int difficulty, int def, int exp)
        {
            Random = new Random();
            beforeRecord = new Record();
            afterRecord = new Record();

            state = EDungunState.PlayerTurn;
            _name = name;

            _difficulty = (EDifficulty)difficulty;

            _monsters = new List<MyMonster>();
            _monsterOrder = new Queue<MyMonster>();

            // 랜덤한 몬스터 수 결정
            int monsterCount = Random.Next(1, 5);

            // 랜덤한 몬스터 종류 선택
            //int MID = Random.Next(0, 5);

            _monsters.Add(new MyMonster("꼬부기"));
            _monsters.Add(new MyMonster("파이리"));
            _monsters.Add(new MyMonster("이상해씨"));
        }

        public void Enter(Player player)
        {
            if(_player == null)
                _player = player;            
            beforeRecord.Save(_player);
            // 몬스터 생성
        }

        public EDungunState Progress(out string[] msg)
        {
            int dmg = 0;
            msg = null;

            switch(state)
            {
                case EDungunState.PlayerTurn:
                    dmg = Attack();
                    // check monster is die? 
                    // Reward. Add . Monster's

                    msg = new string[] { $"{_player.Class} 의 공격!!", $"{_targetMonster.Name} 을(를) 맞췄습니다. 데미지 : {dmg} ]" };
                    break;

                case EDungunState.MonsterAllDeath:
                    msg = new string[] { "모든 몬스터를 쓰러트렸습니다.", "보상을 획득합니다." };
                    state = EDungunState.Clear;
                    break;

                case EDungunState.MonsterTurn:                    
                    MyMonster monster = _monsterOrder.Dequeue();
                    dmg = monster.Attack(_player);

                    msg = new string[2] { $"{monster.Name} 의 공격!!", $"{_player.Class} 을(를) 맞췄습니다. [ 데미지 : {dmg} ]" };

                    if (_player.Hp <= 0)
                    {
                        state = EDungunState.PlayerDeath;
                    }
                    else if (_monsterOrder.Count == 0)
                    {
                        state = EDungunState.PlayerTurn;
                    }
                    break;
            }

            return state;
        }

        bool SetMonsterOrder()
        {
            foreach(var monster in _monsters)
            {
                if (monster.HP > 0)
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
            if (_monsters[index].HP <= 0)
            {
                return false;
            }
            _targetMonster = _monsters[index];
            return true;
        }

        int Attack()
        {            
            int dmg = _player.Attack(_targetMonster);
            
            if(SetMonsterOrder())
            {
                state = EDungunState.MonsterTurn;
            }
            else
            {
                state = EDungunState.MonsterAllDeath;
            }
            
            return dmg;
        }

        public MyMonster[] GetMonster()
        {
            return _monsters.ToArray();
        }
    }
}
