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

        FakeData fakeData = new FakeData();

        [Fact]
        public void Constructor_NoEmptyStringAsKeyInDlcSortedList_AddDlcCheckBoxWithEmptyAbbreviation()
        {
            // Arrange
            var civList = fakeData.GetCivilizations(10);
            var dlcSortedList = fakeData.GetDlc();
            dlcSortedList.Remove("");

            // Act
            var viewModel = new ViewModel(civList, dlcSortedList);

            // Assert
            Assert.Contains(viewModel.DlcCheckboxes, checkBox => checkBox.Dlc.Abbreviation == "");
        }

        [Theory]
        [InlineData(true, 3, 3, 9)]
        [InlineData(false, 3, 3, 3)]
        [InlineData(false, 3, 1, 3)]
        public void SelectedCivsCount_DuplicateCivsExist_ReturnIsBasedOnAllowDuplicateCivs(bool allowDuplicateCivs, int civsWithDuplicatesCount, int numberOfDuplicateInstances, int expectedResult)
        {
            // Arrange
            string testName = "test";
            var civList = new List<Civilization>();

            for (int i = 0; i < civsWithDuplicatesCount; i++)
            {
                for (int j = 0; j < numberOfDuplicateInstances; j++)
                {
                    var civ = new Civilization() { Name = testName + i, Leader = "", Dlc = "" };
                    civList.Add(civ);
                }
            }

            var viewModel = new ViewModel(civList, fakeData.GetDlc());
            viewModel.AllowDuplicateLeaders = true;
            viewModel.AllowDuplicateCivs = allowDuplicateCivs;

            // Act
            int actual = viewModel.SelectedCivsCount;

            // Assert
            Assert.Equal(expectedResult, actual);
        }

        [Theory]
        [InlineData(true, 3, 3, 9)]
        [InlineData(false, 3, 3, 3)]
        [InlineData(false, 3, 1, 3)]
        public void SelectedCivsCount_DuplicateLeadersExist_ReturnIsBasedOnAllowDuplicateLeaders(bool allowDuplicateLeaders, int civsWithDuplicatesCount, int numberOfDuplicateInstances, int expectedResult)
        {
            // Arrange
            string testName = "test";
            var civList = new List<Civilization>();

            for (int i = 0; i < civsWithDuplicatesCount; i++)
            {
                for (int j = 0; j < numberOfDuplicateInstances; j++)
                {
                    var civ = new Civilization() { Name = "", Leader = testName + i, Dlc = "" };
                    civList.Add(civ);
                }
            }

            var viewModel = new ViewModel(civList, fakeData.GetDlc());
            viewModel.AllowDuplicateLeaders = allowDuplicateLeaders;
            viewModel.AllowDuplicateCivs = true;

            // Act
            int actual = viewModel.SelectedCivsCount;

            // Assert
            Assert.Equal(expectedResult, actual);
        }
    }
}
