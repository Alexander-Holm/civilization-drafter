using Civilization_draft.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Civilization_draft.används_inte_för_att_köra
{
    public static class SkapaGrejjs
    {
        public static void SkapaBilder()
        {
            string mappAttLäsaFrån = @"";
            string mappAttSkrivaTill = @"";
            var listOfAllCivilizations = DataAccess.LoadCivilizations().ToArray();
            string[] fileArray = Directory.GetFiles(mappAttLäsaFrån, "*.png");
            int j = 0;
            for (int i = 0; i < listOfAllCivilizations.Length; i++)
            {
                if (!File.Exists(mappAttSkrivaTill + listOfAllCivilizations[i].Name + ".png"))
                {
                    File.Move(fileArray[j], mappAttSkrivaTill + listOfAllCivilizations[i].Name + ".png");
                    j++;
                }
            }
        }

        private class JsonClass
        {
            public List<string> civilizations { get; set; }
            public List<string> leaders { get; set; }
        }
        public static void FormateraJsonFil()
        {
            string filAttLäsa = @"";
            string filAttSkriva = @"";
            var civObjectList = new List<Civilization>();
            using (StreamReader reader = new StreamReader(filAttLäsa))
            {
                var jsonFile = reader.ReadToEnd();
                var jsonObject = new JavaScriptSerializer().Deserialize<JsonClass>(jsonFile);
                var nameArr = jsonObject.civilizations.ToArray();
                var leaderArr = jsonObject.leaders.ToArray();

                for (int i = 0; i < nameArr.Length; i++)
                {
                    Civilization civ = new Civilization()
                    {
                        Name = nameArr[i],
                        Leader = leaderArr[i],
                        Dlc = "dlc",
                        Image = nameArr[i] + ".png",
                    };
                    civObjectList.Add(civ);
                }
            }
            var newJson = new JavaScriptSerializer().Serialize(civObjectList);
            File.WriteAllText(filAttSkriva, newJson);
        }
    }
}
