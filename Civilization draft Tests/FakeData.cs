using Civilization_draft.Models.JsonModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Civilization_draft_Tests
{
    public enum DlcEnum
    {
        dlc1,
        dlc2,
        dlc3,
    };

    public class FakeData
    {        
        public SortedList<DlcEnum, string> dlcAbbreviations;

        public FakeData()
        {
            dlcAbbreviations = new SortedList<DlcEnum, string>();
            foreach (var item in Enum.GetValues(typeof(DlcEnum)))
            {
                dlcAbbreviations.Add((DlcEnum)item, item.ToString());
            }
        }

        public SortedList<string, Dlc> GetDlc()
        {
            SortedList<string, Dlc> dlcSortedList = new SortedList<string, Dlc>();
            foreach (var abbreviation in dlcAbbreviations.Values)
            {
                Dlc dlc = new Dlc
                {
                    Abbreviation = abbreviation,
                    Fullname = abbreviation,
                    HasCheckbox = true,
                    Color = "#32a852",
                };
                dlcSortedList.Add(abbreviation, dlc);
            }
            return dlcSortedList;
        }
        public List<Civilization> GetCivilizations(int amount)
        {
            List<Civilization> civList = new List<Civilization>();
            for (int i = 0; i < amount; i++)
            {
                // Example: dlcAbbreviations.Count = 2
                // i = 0  -> dlcIndex = 0
                // i = 1  -> dlcIndex = 1
                // then starts over
                // i = 2  -> dlcIndex = 0, 
                // i = 3  -> dlcIndex = 1, 
                int dlcIndex = i % dlcAbbreviations.Count;
                string dlcString = dlcAbbreviations.Values[dlcIndex];

                Civilization civ = new Civilization
                {
                    Name = "civ" + (i + 1),
                    Leader = "leader" + (i + 1),
                    Dlc = dlcString,
                    Image = "fakeImage.png",
                };
                civList.Add(civ);
            }
            return civList;
        }
    }
}
