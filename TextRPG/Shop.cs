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
                Item.EType type = (Item.EType)(int.Parse(itemSetting[3]));
                string status = itemSetting[1];

                string _description = itemSetting[2];
                int _price = int.Parse(itemSetting[4]);

                Item item = new Item(_name, status, _description, type, _price);

                _storeItems.Add(item);                
            }
        }
    }
}
