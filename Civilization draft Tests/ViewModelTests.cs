using System;
using Xunit;
using Civilization_draft.ViewModels;
using System.Collections.Generic;
using Civilization_draft.Models;
using System.Linq;
using Civilization_draft.Models.JsonModels;

namespace Civilization_draft_Tests
{
    public class ViewModelTests
    {
        // https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
        //--- The name of your test should consist of three parts ---
        //* The name of the method being tested.
        //* The scenario under which it's being tested.
        //* The expected behavior when the scenario is invoked.

        ViewModel viewModel;

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Test1(bool boolToTest)
        {
            // Arrange            
            string dlcToTest = "GS";
            string dlcOther = "OtherDlc";
            SortedList<string, Dlc> dlcSortedList = new SortedList<string, Dlc>
            {
                { dlcToTest, new Dlc{ Abbreviation = dlcToTest, HasCheckbox = true, } },
                { dlcOther, new Dlc{ Abbreviation = dlcOther, HasCheckbox = true, } },

            };
            List<Civilization> civList = new List<Civilization>
            {
                new Civilization
                {
                    Name = "Civ",
                    Dlc = dlcToTest,
                    Leader = "Leader"
                },
                new Civilization
                {
                    Name = "Civ2",
                    Dlc = dlcToTest,
                    Leader = "Leader2"
                },
                new Civilization
                {
                    Name = "Civ3",
                    Dlc = dlcOther,
                    Leader = "Leader3"
                },
            };

            viewModel = new ViewModel(civList, dlcSortedList);
            var dlcCheckBox = viewModel.DlcCheckboxes.First(dlcCheckBox => dlcCheckBox.Dlc.Abbreviation == dlcToTest);
            dlcCheckBox.AllSelected = !boolToTest;

            // Act
            dlcCheckBox.AllSelected = boolToTest;
            bool testFail = false;
            foreach (var civButton in viewModel.CivButtonList)
            {
                if (civButton.Dlc.Abbreviation == dlcCheckBox.Dlc.Abbreviation
                    &&
                    civButton.IsChecked != boolToTest)
                {
                    testFail = true;
                }
            }

            // Assert
            Assert.True(!testFail);
        }
    }
}
