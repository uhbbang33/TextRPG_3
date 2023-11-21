using Newtonsoft.Json.Linq;
using System;


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

        string name = "...";
        public string Name { get { return name; } set { name = Name; } }

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

        int mp = 100;
        public int Mp { get { return mp; } set { mp -= value; } }

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

                //포션 먹을 때, 최대 체력 이상으로 회복하지 못하게 함
                if (hp > maxHp)
                {
                    hp = maxHp;
                }
            }
        }

        int maxMp = 100;
        int maxHp = 100;
        int _deltaHp = 0;
        public int MaxHp { get { return maxHp + _deltaHp; } }
        public int MaxMp { get { return maxMp; } }

        float _evasion = 0.2f;

        int _exp = 0;
        int _maxExp = 10;
        public int MaxExp { get { return _maxExp; } }
        int[] _expByLevel = { 0, 10, 20, 30, 40, 50, 70, 95, 120, 200 };

        public int Exp { get { return _exp; } }

        List<Item> _inventory = new List<Item>();
        List<Skill> _skills = new List<Skill>();
        EquipManager _equipManager;
        int _gold = 0;

        public List<Item> Inventory { get { return _inventory; } }
        public List<Skill> Skills { get { return _skills; } }
        public Item[] Equipment { get { return _equipManager.EquipItems; } }
        public int Gold { get { return _gold; } set { _gold = value; } }

        public Player()
        {
            //저장돼있는 플레이어 정보를 가져와 형변환 후 변수 초기화(이어하기)
            JObject save = Loader.LoadData(@"..\..\..\data\Player.json");
            lv = (int)save["Lv"];
            name = save["Name"].ToString();
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            _exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            for (int i = 0; i < _inventory.Count; ++i)
            {
                if (_inventory[i].bEquip)
                {
                    EquipItem(i);
                }
            }
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
            _exp = exp;

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
        public void SetWizard(String getName)
        {
            JObject save = Loader.LoadData(@"..\..\..\data\wizardData.json");
            lv = (int)save["Lv"];
            name = getName;
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            _exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            foreach (var item in _inventory)
            {
                if (item.bEquip)
                    _equipManager.Wear(item);
            }

        }

        //클래스 선택창에서 전사 직업을 골랐을 때, 기존 데이터 덮어씌우기
        public void SetWarrior(String getName)
        {
            JObject save = Loader.LoadData(@"..\..\..\data\warriorData.json");
            lv = (int)save["Lv"];
            name = getName;
            job = save["Class"].ToString();
            atk = (int)save["Atk"];
            def = (int)save["Def"];
            maxHp = (int)save["MaxHP"];
            hp = maxHp;
            _maxExp = (int)save["MaxExp"];
            _exp = (int)save["Exp"];
            _gold = (int)save["Gold"];
            _crit = (float)save["Critical"];

            //스킬 불러와 리스트에 저장
            _skills = save["Skills"].ToObject<List<Skill>>();
            _inventory = save["Inventory"].ToObject<List<Item>>();

            _equipManager = new EquipManager();

            foreach (var item in _inventory)
            {
                if (item.bEquip)
                    _equipManager.Wear(item);
            }
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
            else if (_inventory.Count == 40 && item.type != Item.EType.Potion)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                //해당 아이템이 무기라면 리스트에 추가
                if (item.type == Item.EType.Weapon || item.type == Item.EType.Weapon)
                {
                    _inventory.Add(item);
                }
                //해당 아이템이 소모품 && 해당 아이템을 이미 보유중 이라면 개수만 추가
                else
                {
                    bool hasPotion = false;
                    int potionIndex = 0;
                    for (int i = 0; i < _inventory.Count; i++)
                    {
                        if (_inventory[i].Name == item.Name)
                        {
                            hasPotion = true;
                            potionIndex = i;
                            break;
                        }
                    }
                    if (hasPotion)
                    {
                        _inventory[potionIndex].HasCount += 1;
                    }
                    else
                    {
                        _inventory.Add(item);
                    }
                }
                _gold -= item.Price;
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
                //해당 아이템이 장비 아이템이라면 바로 제거
                if (_inventory[index].type == Item.EType.Weapon || _inventory[index].type == Item.EType.Armor)
                {
                    _inventory.RemoveAt(index);
                }
                //해당 아이템이 장비 아이템이 아니라면 보유 개수를 확인 후 제거
                else
                {
                    if (_inventory[index].HasCount > 1)
                    {
                        _inventory[index].HasCount -= 1;
                    }
                    else
                    {
                        _inventory.RemoveAt(index);
                    }
                }
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

        //HP 포션 사용
        public void UsedHealthPotion(int index)
        {

            if (_inventory[index].HasCount > 1)

            {
                hp += _inventory[index].Value;
                _inventory[index].HasCount -= 1;
            }
            else
            {
                hp += _inventory[index].Value;
                _inventory.RemoveAt(index);
            }
        }
        //MP 포션 사용
        public void UsedManaPotion(int index, int mp)
        {

            if (_inventory[index].HasCount > 1)
            {
                mp += _inventory[index].Value;
                _inventory[index].HasCount -= 1;
            }
            else
            {
                mp += _inventory[index].Value;
                _inventory.RemoveAt(index);
            }
        }
        //버프 아이템 변수
        int BuffTurn;
        int BuffHP;
        int BuffAtk;
        int BuffDef;

        //버프 아이템 사용
        public void UsedPotion(int index, int turn)
        {
            if (_inventory[index].HasCount > 1)
            {
                BuffTurn = turn;
                switch (_inventory[index].Status)
                {
                    case "체력":
                        BuffHP += _inventory[index].Value;
                        break;

                    case "공격력":
                        BuffAtk += _inventory[index].Value;
                        break;

                    case "방어력":
                        BuffDef += _inventory[index].Value;
                        break;
                }
                _inventory[index].HasCount -= 1;
            }
            else
            {              
                BuffTurn = turn;
                switch (_inventory[index].Status)
                {
                    case "체력":
                        BuffHP += _inventory[index].Value;
                        break;

                    case "공격력":
                        BuffAtk += _inventory[index].Value;
                        break;

                    case "방어력":
                        BuffDef += _inventory[index].Value;
                        break;
                }
                _inventory.RemoveAt(index);
            }
        }
        
        //버프 턴 감소 및 수치 초기화
        public void SubtractorBuffTurn()
        {
            if(BuffTurn > 0)
            {
                BuffTurn -= 1;
            }
            else
            {
                BuffHP = 0;
                BuffAtk = 0;
                BuffDef = 0;
            }

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
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                lv.ToString(),
                name,
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

            if (random.NextDouble() < _crit)
            {
                bCrit = true;
                dmg *= (int)(dmg * 1.6f);
            }

            dmg = (int)(dmg * Skills[SID].damage);

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
                invenPage = (int)Math.Ceiling((double)Inventory.Count / 6) - 1;
            }
            else
            {
                invenPage -= 1;
            }
        }
        public void BackwardPage()
        {
            if (invenPage >= (int)Math.Ceiling((double)Inventory.Count / 6) - 1)
            {
                invenPage = 0;
            }
            else
            {
                invenPage += 1;
            }
        }

        public void GetExp(int exp, out bool levelUp)
        {
            levelUp = false;
            _exp += exp;
            if (_exp >= _maxExp)
            {
                _exp -= _maxExp;
                _maxExp = _expByLevel[++lv];
                atk += 2;
                def += 1;
                levelUp = true;
            }
        }

    }
}
