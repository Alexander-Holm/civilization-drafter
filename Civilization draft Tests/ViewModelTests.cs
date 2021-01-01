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
        FakeData fakeData;
        List<Civilization> civList;
        SortedList<string, Dlc> dlcSortedList;

        public ViewModelTests()
        {
            fakeData = new FakeData();
            civList = fakeData.GetCivilizations();
            dlcSortedList = fakeData.GetDlc();
        }

        [Fact]
        public void Constructor_NoEmptyStringAsKeyInDlcSortedList_AddDlcCheckBoxWithEmptyAbbreviation()
        {
            // Arrange
            dlcSortedList.Remove("");

            // Act
            viewModel = new ViewModel(civList, dlcSortedList);

            // Assert
            Assert.Contains(viewModel.DlcCheckboxes, checkBox => checkBox.Dlc.Abbreviation == "");
        }
    }
}
