using Newtonsoft.Json.Linq;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        bool _isMonsterHuntQuest;
        public bool IsMonsterHuntQuest { get { return _isMonsterHuntQuest; } }

        bool _isCompleted;
        bool IsAcceptQuest;

        public Quest(string name, int num, int reward, bool isMonsterHuntQuest)
        {
            _name = name;
            _num = num;
            _reward = reward;
            _isMonsterHuntQuest = isMonsterHuntQuest;
            _isCompleted = false;
        }
    }

    public class QuestList
    {
        List<Quest> _quests = new List<Quest>();
        public List<Quest> Quests { get { return _quests; } }

        public QuestList()
        {
            JObject save = Loader.LoadData(@"..\..\..\data\QuestList.json");
            _quests = save["ShopQuestList"].ToObject<List<Quest>>();
        }
    }
}
