using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft.Models.JsonModels
{
    public class Dlc
    {
        public string Abbreviation { get; set; }
        public string Fullname { get; set; }
        public string Color { get; set; }
        public bool HasCheckbox { get; set; }
    }
}
