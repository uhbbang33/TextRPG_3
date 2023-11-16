using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TextRPG
{
    public static class Loader
    {
        //플레이어의 데이터를 가져오는 함수 .json 형식
        public static JObject LoadPlayerData(string path)
        {
            using (StreamReader file = File.OpenText(path))//@"..\..\..\Player.json"
            {
                using(JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject)JToken.ReadFrom(reader);
                }
            }
        }

        //무기와 방어구 장비의 데이터를 가져오는 함수 .json 형식
        public static JObject LoadEquipment(string path)
        {
            using (StreamReader file = File.OpenText(path))//@"..\..\..\Equipment.json"
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

        //플레이어의 데이터를 원본 파일에 저장하는 함수
        public static void Save(Player player)
        {
            string path = @"..\..\..\Player.json";
            JObject configData = new JObject(
                new JProperty("Lv", player.Lv),
                new JProperty("Class", player.Class),
                new JProperty("Atk", player.BaseAtk),
                new JProperty("Def", player.BaseDef),
                new JProperty("MaxHP", player.MaxHp),
                new JProperty("Exp", player.Exp),
                new JProperty("MaxExp", player.MaxExp),
                new JProperty("Gold", player.Gold)
                );
            
            Item[] inventory = player.Inventory.ToArray();
            configData.Add("Inventory", JArray.FromObject(inventory));

            File.WriteAllText(path, configData.ToString());
            
            Save(player.Equipment);
        }


        //장비 아이템의 변경된 데이터를 원본 파일에 저장하는 함수
        public static void Save(Item[] items)
        {
            string path = @"..\..\..\Equipment.json";
            JObject configData = new JObject();
            configData.Add("Equip", JArray.FromObject(items));

            File.WriteAllText(path, configData.ToString());
        }
    }
}
