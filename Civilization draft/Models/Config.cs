using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft.Models
{
    public class Config
    {
        public List<CivSimple> UncheckedCivs { get; set; }
        public int SelectedCivsPerPlayer { get; set; }
        public int SelectedNumberOfPlayers { get; set; }
        public bool AllowDuplicateCivs { get; set; }
        public bool AllowDuplicateLeaders { get; set; }

        public static Config GetDefaultConfig()
        {
            Config config = new Config
            {
                UncheckedCivs = new List<CivSimple>(),
                SelectedCivsPerPlayer = 3,
                SelectedNumberOfPlayers = 1,
                AllowDuplicateCivs = false,
                AllowDuplicateLeaders = false,
            };
            return config;
        }
    }
}
