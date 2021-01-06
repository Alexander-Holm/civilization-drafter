using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft.Models
{
    public class PlayerResult
    {
        public int PlayerNumber { get; set; }
        public List<CivButton> Civs { get; set; } = new List<CivButton>();
    }
    class ClipboardPlayerResult
    {
        public int Player { get; set; }
        public List<CivSimple> Civilizations { get; set; }
    }
}
