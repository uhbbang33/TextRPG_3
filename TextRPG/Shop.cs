namespace TextRPG
{
    internal class Shop
    {
        
        public List<Item> storeItems = new List<Item>();

        public Shop()
        {
            LoadItemCSV(storeItems);
        }

        //CSV를 읽어서 리스트를 넣음
        static void LoadItemCSV(List<Item> _storeItems)
        {
            StreamReader itemCSV = new StreamReader(@"..\..\..\data\ItemData.csv");

            string strCSV = itemCSV.ReadToEnd();
            string[] CSVLine = strCSV.Split("\n");

            for (int i = 0; i < CSVLine.Length; i++)
            {
                string[] itemSetting = CSVLine[i].Split(',');
                string _name = itemSetting[0];
                Item.EType _type = new Item().type;
                if (itemSetting[2] == "Weapon")
                {
                    _type = Item.EType.Weapon;
                }
                else if (itemSetting[2] == "Armor")
                {
                    _type = Item.EType.Armor;
                }
                else if (itemSetting[2] == "Potion")
                {
                    _type = Item.EType.Potion;
                }

                Item.ClassRestricted _restricted = new Item().restricted;
                if (itemSetting[1] == "NoRestricted")
                {
                    _restricted = Item.ClassRestricted.NoRestricted;
                }
                else if (itemSetting[1] == "Warrior")
                {
                    _restricted = Item.ClassRestricted.Warrior;
                }
                else if (itemSetting[1] == "Wizard")
                {
                    _restricted = Item.ClassRestricted.Wizard;
                }

                int _hp = int.Parse(itemSetting[3]);
                int _atk = int.Parse(itemSetting[4]);
                int _def = int.Parse(itemSetting[5]);

                string _status = "";
                if (_hp != 0)
                {
                    _status = $"0:{_hp}";
                }
                else if (_atk != 0)
                {
                    _status = $"1:{_atk}";
                }
                else if (_def != 0)
                {
                    _status = $"2:{_def}";
                }

                string _description = itemSetting[6];
                int _price = int.Parse(itemSetting[7]);

                Item item = new Item(_name,  _status, _description, _restricted ,_type, _price);

                _storeItems.Add(item);
                
            }

        }
    
    }
}
