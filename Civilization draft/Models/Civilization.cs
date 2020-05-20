using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Civilization_draft
{
    public class Civilization
    {
        public string Name { get; set; }
        public string Leader { get; set; }
        public string Dlc { get; set; }
        private string _image;
        public string Image
        {
            get
            {
                return "/Civilization draft;component/Images/Civs/" + _image;
            }
            set { _image = value; }
        }

        public static List<Civilization> GetCivilizationsList()
        {
            string filepath = "Resources/Civilizations.json";
            using (StreamReader r = new StreamReader(filepath))
            {
                string jsonCivilizations = r.ReadToEnd();
                List<Civilization> civilizationList = new JavaScriptSerializer().Deserialize<List<Civilization>>(jsonCivilizations);
                return civilizationList;
            }
        }
    }
}
