namespace TextRPG
{
    internal class Shop
    {
        
        public List<List<Item>> storePageBundle = new List<List<Item>>();

        public Shop()
        {
            LoadItemCSV(storePageBundle);
        }

        //CSV를 읽어서 리스트를 넣음
        static void LoadItemCSV(List<List<Item>> _storePageBundle)
        {
            StreamReader itemCSV = new StreamReader(@"..\..\..\ItemData.csv");

            string strCSV = itemCSV.ReadToEnd();
            string[] CSVLine = strCSV.Split("\n");

            List<Item> StoreItems = new List<Item>();

            for (int i = 0; i < CSVLine.Length; i++)
            {
                string[] itemSetting = CSVLine[i].Split(',');
                string _name = itemSetting[0];
                Item.EType _type = new Item().type;
                if (itemSetting[1] == "Weapon")
                {
                    _type = Item.EType.Weapon;
                }
                else if (itemSetting[1] == "Armor")
                {
                    _type = Item.EType.Armor;
                }
                else if (itemSetting[1] == "Potion")
                {
                    _type = Item.EType.Potion;
                }

                int _hp = int.Parse(itemSetting[2]);
                int _atk = int.Parse(itemSetting[3]);
                int _def = int.Parse(itemSetting[4]);

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

                string _description = itemSetting[5];
                int _price = int.Parse(itemSetting[6]);

                Item item = new Item(_name, _status, _description, _type, _price);

                StoreItems.Add(item);
                
                if (StoreItems.Count > 5 || i == CSVLine.Length - 1)
                {
                    _storePageBundle.Add(StoreItems);
                    StoreItems = new List<Item>();
                }
            }

        }
    
    }
}
