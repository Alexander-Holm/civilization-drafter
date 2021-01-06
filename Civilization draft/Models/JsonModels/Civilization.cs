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
        public string Image { get; set; }
    }
}
