using Civilization_draft.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Civilization_draft.Models.JsonModels
{
    public class Civilization
    {
        public string Name { get; set; }
        public string Leader { get; set; }
        public string Dlc { get; set; }
        //private string _image;
        //public string Image
        //{
        //    get
        //    {
        //        return "/Civilization draft;component/Images/Civs/" + _image;
        //    }
        //    set { _image = value; }
        //}
        public string Image { get; set; }
    }
}
