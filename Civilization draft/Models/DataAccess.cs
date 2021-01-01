using Civilization_draft.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Civilization_draft.Models
{
    static class DataAccess
    {
        private static readonly string path = "Resources/";

        public static SortedList<string, Dlc> LoadDlc()
        {
            List<Dlc> listFromJson;
            using (StreamReader r = new StreamReader(path + "Dlc.json"))
            {
                string dlc = r.ReadToEnd();
                listFromJson = new JavaScriptSerializer().Deserialize<List<Dlc>>(dlc);
            }

            SortedList<string, Dlc> outputList = new SortedList<string, Dlc>();
            foreach (var dlc in listFromJson)
            {
                outputList.Add(dlc.Abbreviation, dlc);
            }

            return outputList;
        }

        public static List<Civilization> LoadCivilizations()
        {
            List<Civilization> civList;
            using (StreamReader r = new StreamReader(path + "Civilizations.json"))
            {
                string jsonCivilizations = r.ReadToEnd();
                civList = new JavaScriptSerializer().Deserialize<List<Civilization>>(jsonCivilizations);
            }
            
            return civList.OrderBy(c => c.Name).ToList();
        }
    }
}
