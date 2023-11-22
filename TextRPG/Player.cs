﻿using Newtonsoft.Json.Linq;



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
        int _maxLv = 10;
        int lv = 1;
        public int Lv
        {
            get
            {
                return lv;
            }
            set
            {
                lv = value;

                //최대 레벨은 10
                if (lv > 10)
                    lv = 10;

                //최대 경험치를 레벨마다 다르게
                MaxExp = _expByLevel[lv - 1];
            }
        }

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

        int mp = 50;
        public int Mp
        {
            get
            {
                return mp;
            }
            set
            {
                mp = value;

                //최대 마나를 초과하여 회복하지 못하게 함
                if (mp > maxMp)
                {
                    mp = maxMp;
                }
            }
        }

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
                }

                //최대 체력을 초과하여 회복하지 못하게 함
                if (hp > maxHp)
                {
                    hp = maxHp;
                }
            }
        }

        int maxMp = 50;
        int maxHp = 100;
        int _deltaHp = 0;
        public int MaxHp { get { return maxHp + _deltaHp; } }
        public int MaxMp { get { return maxMp; } }

        float _evasion = 0.2f;

        int _exp = 0;
        int _maxExp = 10;
        public int MaxExp { get { return _maxExp; } set { _maxExp = value; } }
        int[] _expByLevel = { 50, 60, 80, 110, 150, 200, 270, 365, 485, 600 };

        public int Exp { get { return _exp; } }

        List<Item> _inventory = new List<Item>();
        List<Skill> _skills = new List<Skill>();
        EquipManager _equipManager;
        int _gold = 0;

        Quest _playerQuest;

        public List<Item> Inventory { get { return _inventory; } }
        public List<Skill> Skills { get { return _skills; } }
        public Item[] Equipment { get { return _equipManager.EquipItems; } }
        public int Gold { get { return _gold; } set { _gold = value; } }
        public Quest PlayerQuest { get { return _playerQuest; } }
        public bool ShouldCreateShopQuest = false;


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
            _playerQuest = save["Quest"].ToObject<Quest>();

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
            else if (_inventory.Count == 40 && item.type != Item.EType.Potion)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                MergeIfConsumable(item);
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

        //아이템이 소모품이라면 아이템을 합침
        public void MergeIfConsumable(Item item)
        {
            if (item.type != Item.EType.Potion)
            {
                Inventory.Add(item);
            }
            else
            {
                int index = 0;
                bool isHave = false;
                for (int i = 0; i < Inventory.Count; i++)
                {
                    if (item.Name == Inventory[i].Name)
                    {
                        isHave = true;
                        index = i;
                        break;
                    }
                }
                if (isHave)
                {
                    Inventory[index].HasCount++;
                }
                else if (!isHave)
                {
                    Inventory.Add(item);
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

        public bool UseItem(int index)
        {
            Item item = _inventory[index];
            switch (item._status)
            {
                case Item.EStatus.ATK:
                    break;

                case Item.EStatus.DEF:
                    break;

                case Item.EStatus.HP:
                    if (Hp == MaxHp) return false;
                    Hp += item.Value;
                    break;
            }

            --item.HasCount;
            if (item.HasCount == 0) _inventory.RemoveAt(index);
            return true;
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
            if (BuffTurn > 0)
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
                dmg = (int)(dmg * 1.6f);
            }

            dmg = (int)(dmg * Skills[SID].damage);

            if (!MpCheck(SID)) dmg = 0;
            dmg = monster.TakeDamage(dmg, Skills[SID].accuracy);
            
            if (dmg == 0) bCrit = false;
            return dmg;
        }

        //소모하려는 스킬의 Mp를 체크하는 함수
        public bool MpCheck(int SID)
        {
            //사용하려는 스킬의 소모MP가 남아있는 MP보다 작을 때
            if (mp < _skills[SID].cost)
            {
                return false;
            }

            mp -= _skills[SID].cost;//스킬 사용가능할 때 mp소모

            return true;
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
        //인벤토리 페이지 번호 변경하는 메소드
        public int invenPage = 0;
        public void PageNum()
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
                Lv += 1;
                atk += 2;
                def += 1;

                hp = MaxHp;
                mp = MaxMp;
                levelUp = true;
            }
        }

        //전투 끝나고 마나 회복을 위한 함수
        public void GetMp(int mp)
        {
            Mp += mp;
        }

        #region Quest 관련 함수

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
            if (_playerQuest.Name == null || _playerQuest.IsMonsterHuntQuest)
                return false;

            int cnt = 0;
            List<int> indexList = new List<int>();
            for (int i = 0; i < _inventory.Count; ++i)
                if (_inventory[i].Name == _playerQuest.Name)
                {
                    ++cnt;

                    // 퀘스트 아이템 인덱스 indexList에 추가
                    indexList.Add(i);

                }

            if (cnt >= _playerQuest.Num)
            {
                // 아이템을 퀘스트num만큼만 삭제
                cnt = _playerQuest.Num;
                for (int i = indexList.Count - 1; i >= 0; --i)
                {
                    int _index = indexList[i];
                    if (cnt > 0)
                    {
                        _inventory.RemoveAt(_index);
                        --cnt;
                    }
                }
                ShouldCreateShopQuest = true;
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
        }

        #endregion
        public void Revival()
        {
            _gold /= 2;

            hp = maxHp;

            _exp -= (int)(MaxExp / 10);
            _exp = _exp < 0 ? 0 : _exp;
        }
    }
}
