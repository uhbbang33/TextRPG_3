using Newtonsoft.Json.Linq;
using System;
using System.Numerics;

namespace TextRPG
{

    
 
    class EquipManager
    {
        Item[] _equips;
        public Item[] EquipItems { get { return _equips; } }

        // _equips[(int)Item.EType]
        public EquipManager()
        {
            //JObject save = Loader.LoadEquipment(@"..\..\..\Player.json");
            //_equips = save["Equip"].ToObject<Item[]>();
            _equips = new Item[2];

            for (int i = 0; i < _equips.Length; ++i)
            {
                _equips[i] = Item.NULL;
            }
        }

        public void Wear(Item item)
        {
            int parts = (int)item.type;
            if (_equips[parts] == item) // 동일한 아이템일 경우
            {
                _equips[parts].bEquip = false;
                _equips[parts] = Item.NULL;
            }
            else if (_equips[parts] == Item.NULL) // 비어있는 경우
            {
                _equips[parts] = item;
                item.bEquip = true;
            }
            else // 비어있지 않은 경우
            {
                _equips[parts].bEquip = false;
                _equips[parts] = item;
                item.bEquip = true;
            }
        }
    }

    public class Player
    {
        int lv = 1;
        public int Lv { get { return lv; } }

        string job = "초보자";
        public string Class { get { return job; } }

        int atk = 10;
        public int BaseAtk { get { return atk; } }
        int _deltaAtk = 0;
        public int Atk { get { return atk + _deltaAtk; } }

        int def = 1;
        public int BaseDef { get { return def; } }
        int _deltaDef = 0;
        public int Def { get { return def + _deltaDef; } }

        float _crit = 0.3f;
        public float Crit { get { return _crit; } }

        int hp;
        public int Hp
        {
            get
            {
                return hp;
            }
            set
            {
                hp = value;
                if (hp < 0)
                {
                    hp = 0;

                    // 골드 감소
                    _gold -= (int)(_gold * 0.3f);
                }
            }
        }

        int maxHp = 100;
        int _deltaHp = 0;
        public int MaxHp { get { return maxHp + _deltaHp; } }

        float _evasion = 0.2f;

        int _exp = 0;
        int _maxExp = 10;
        public int MaxExp { get { return _maxExp; } }
        int[] _expByLevel = { 0, 10, 20, 30, 40, 50, 70, 95, 120, 200 };

        public int Exp
        {
            get { return _exp; }
            set
            {
                _exp = value;
                if (_exp >= _maxExp)
                {
                    _exp -= _maxExp;
                    ++lv;
                    _maxExp = _expByLevel[lv];
                    atk += 2;
                    def += 1;
                }
            }
        }

        List<Item> _inventory = new List<Item>();
        List<Skill> _skills = new List<Skill>();
        EquipManager _equipManager;
        int _gold = 0;
        Quest _playerQuest = new Quest("",0,0,false);

        public List<Item> Inventory { get { return _inventory; } }
        public List<Skill> Skills { get { return _skills; } }
        public Item[] Equipment { get { return _equipManager.EquipItems; } }
        public int Gold { get { return _gold; } }
        public Quest PlayerQuest { get { return _playerQuest; } }
        
        public bool isQuestComplte = false;

        public int hasPotion = 0;

        public Player() 
        {
            //저장돼있는 플레이어 정보를 가져와 형변환 후 변수 초기화(이어하기)
            JObject save = Loader.LoadData(@"..\..\..\data\Player.json");
            lv = (int)save["Lv"];
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            Exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];
            hasPotion = (int)save["HasPotion"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            for(int i = 0; i < _inventory.Count; ++i)
            {
                if (_inventory[i].bEquip)
                {
                    EquipItem(i);
                }
            }

            _playerQuest = save["Quest"].ToObject<Quest>();
        }

        public Player(int lv, string job, int atk, int def, int maxHp, int exp, int maxExp, int gold, float critical)
        {
            this.lv = lv;
            this.job = job;
            this.atk = atk;

            this.def = def;

            this.maxHp = maxHp;
            hp = maxHp;

            _maxExp = maxExp;
            Exp = exp;

            _gold = gold;

            _crit = critical;
        }

        public void EquipItem(int index)
        {
            _equipManager.Wear(_inventory[index]); // 장착 교환 탈착 의 형태로 리턴?

            _deltaHp = _deltaDef = _deltaAtk = 0;

            foreach (Item item in _equipManager.EquipItems)
            {
                if (item == null) continue;
                switch (item.Status)
                {
                    case "체력":
                        _deltaHp += item.Value;
                        break;

                    case "공격력":
                        _deltaAtk += item.Value;
                        break;

                    case "방어력":
                        _deltaDef += item.Value;
                        break;
                }
            }
        }

        //클래스 선택창에서 마법사 직업을 골랐을 때, 기존 데이터 덮어씌우기
        public void SetWizard()
        {
            JObject save = Loader.LoadData(@"..\..\..\data\wizardData.json");
            lv = (int)save["Lv"];
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            Exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];
            hasPotion = (int)save["HasPotion"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            foreach (var item in _inventory)
            {
                if (item.bEquip)
                    _equipManager.Wear(item);
            }
            _playerQuest = save["Quest"].ToObject<Quest>();

        }

        //클래스 선택창에서 전사 직업을 골랐을 때, 기존 데이터 덮어씌우기
        public void SetWarrior()
        {
            JObject save = Loader.LoadData(@"..\..\..\data\warriorData.json");
            lv = (int)save["Lv"];
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            Exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];
            hasPotion = (int)save["HasPotion"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            foreach (var item in _inventory)
            {
                if (item.bEquip)
                    _equipManager.Wear(item);
            }
            _playerQuest = save["Quest"].ToObject<Quest>();

        }

        public void SortInventory()
        {
            _inventory.Sort((x, y) =>
            {
                if (x.Name.Length > y.Name.Length) return -1;
                else if (x.Name.Length == y.Name.Length) return 0;
                else return 1;
            });
        }

        public void Buy(Item item)
        {
            if (_gold < item.Price)
            {
                throw new GoldShortageException();
            }
            else if (_inventory.Count == 40 && item.type != Item.EType.Potion)// _inventory.Max)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                _gold -= item.Price;
               if (item.type != Item.EType.Potion)
                {
                    _inventory.Add(item);
                }
                else
                {
                    hasPotion += 1;
                }
            }
        }

        public void Sell(int index)
        {
            if (_inventory[index].bEquip)
            {
                throw new EquippedItemException();
            }
            else
            {
                _gold += _inventory[index].Price;
                _inventory.RemoveAt(index);
            }
        }

        public void Damaged(int dmg)
        {
            Hp -= dmg;
        }

        public void ReceiveGold(int gold)
        {
            _gold += gold;
        }

        //포션먹는 매서드
        public bool DrinkPotion()
        {
            if(this.hasPotion<=0)
            {
                return false;
            }

            this.hasPotion--;

            return true;
        }

        public bool Rest()
        {
            if (_gold >= 300)
            {
                hp = MaxHp;
                _gold -= 300;
                return true;
            }
            return false;

        }

        public string GetData()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}",
                lv.ToString(),
                job,
                atk.ToString(),
                def.ToString(),
                maxHp.ToString(),
                Exp.ToString(),
                _maxExp.ToString(),
                Gold.ToString(),
                _crit.ToString()
                );
        }

        // Attack
        public int Attack(int SID, Monster monster, out bool bCrit) // SID : Skill ID
        {
            bCrit = false;
            int atkOffset = (int)Math.Ceiling(0.1f * Atk);

            Random random = new Random();
            int dmg = random.Next(Atk - atkOffset, Atk + atkOffset + 1);

            if(random.NextDouble() < _crit)
            {
                bCrit = true;
                dmg *= 2;
            }

            dmg =  (int)(dmg * Skills[SID].damage);

            dmg = monster.TakeDamage(dmg, Skills[SID].accuracy);

            return dmg;
        }

        public int TakeDamage(int damage)
        {
            int dmg = damage - Def;
            if (dmg < 0) dmg = 1;

            Random random = new Random();
            if (random.NextDouble() > _evasion)
            {
                Hp -= dmg;
            }
            else
            {
                dmg = 0;
            }
            return dmg;
        }

        public int invenPage = 0;
        public void ResetPage()
        {
            invenPage = 0;
        }
        public void ForwardPage()
        {
            if (invenPage == 0)
            {
                invenPage = (int)Math.Ceiling((double)Inventory.Count / 6)-1;
            }
            else
            {
                invenPage -= 1;
            }
        }
        public void BackwardPage()
        {
            if (invenPage >=  (int)Math.Ceiling((double)Inventory.Count / 6)- 1)
            {
                invenPage = 0;
            }
            else
            {
                invenPage += 1;
            }
        }

        public void SetQuest(Quest quest)
        {
            _playerQuest = quest;
        }

        public void SetQuestNull()
        {
            _playerQuest = new Quest(null, 0, 0, false);
        }

        public bool IsShopQuestComplete()
        {
            if (_playerQuest.Name == null)
                return false;

            int cnt = 0;
            foreach (var item in _inventory)
                if (item.Name == _playerQuest.Name)
                {
                    ++cnt;
                    // 아이템 index 기억
                    
                }

            if (cnt >= _playerQuest.Num)
            {
                // 아이템 삭제

                return true;
            }

            return false;
        }

        public bool IsTempleQuestComplete()
        {
            // 던전이 끝날 때 cnt ++?
            // 머지 이후 구현

            return false;
        }

        public void GetQuestReward()
        {
            _gold += _playerQuest.Reward;
            //_exp += _playerQuest.ExpReward;
        }
    }
}
