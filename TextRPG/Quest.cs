using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Text.Json.Serialization;

namespace TextRPG
{
    public class Quest
    {
        string _name;
        public string Name { get { return _name; } }

        int _num;
        public int Num { get { return _num; } }

        int _reward;
        public int Reward {  get { return _reward; } }

        bool _isTempleQuest;
        public bool IsTempleQuest { get {  return _isTempleQuest; } }

        public Quest(string name, int num, int reward, bool isTempleQuest)
        {
            _name = name;
            _num = num;
            _reward = reward;
            _isTempleQuest = isTempleQuest;
        }
    }

    public class QuestList
    {
        List<Quest> _shopQuests = new List<Quest>();
        List<Quest> _templeQuests = new List<Quest>();
        public List<Quest> ShopQuests { get { return _shopQuests; } }
        public List<Quest> TempleQuests { get { return _templeQuests; } }

        public QuestList()
        {
            JObject save = Loader.LoadData(@"..\..\..\data\QuestList.json");
            _shopQuests = save["ShopQuestList"].ToObject<List<Quest>>();
            _templeQuests = save["TempleQuestList"].ToObject<List<Quest>>();
        }
    }
}
