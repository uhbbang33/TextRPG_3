using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TextRPG
{
    public static class Loader
    {
        //파일 경로에 있는 .json 형식의 파일을 로드하는 함수
        public static JObject LoadData(string path)
        {
            using (StreamReader file = File.OpenText(path))//@"..\..\..\Player.json"
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        //파일이 있는지 없는지 체크하는 함수
        public static bool LoadCheck(string path)
        {
            //해당 경로에 파일이 없으면 false 전달
            if (!File.Exists(path))
            {
                return false;
            }

            return true;
        }

        public static void Save(Player player)
        {
            string path = @"..\..\..\data\Player.json";
            JObject configData = new JObject(
                new JProperty("Lv", player.Lv),
                new JProperty("Name", player.Name),
                new JProperty("Class", player.Class),
                new JProperty("Atk", player.BaseAtk),
                new JProperty("Def", player.BaseDef),
                new JProperty("MaxHP", player.MaxHp),
                new JProperty("Exp", player.Exp),
                new JProperty("MaxExp", player.MaxExp),
                new JProperty("Gold", player.Gold),
                new JProperty("Critical", player.Crit),
                new JProperty("HasPotion", player.hasPotion)
                );

            Item[] inventory = player.Inventory.ToArray();
            configData.Add("Inventory", JArray.FromObject(inventory));

            //사용중인 플레이어 데이터를 가져와 json에 배열로 기록
            Skill[] skills = player.Skills.ToArray();
            configData.Add("Skills", JArray.FromObject(skills));

            try
            {
                File.WriteAllText(path, configData.ToString());
            }
            catch(Exception e)
            {
                
            }

            Save(player.Equipment);
        }


        //장비 아이템의 변경된 데이터를 원본 파일에 저장하는 함수
        public static void Save(Item[] items)
        {
            string path = @"..\..\..\data\Equipment.json";
            JObject configData = new JObject();
            configData.Add("Equip", JArray.FromObject(items));

            File.WriteAllText(path, configData.ToString());
        }
    }
}
