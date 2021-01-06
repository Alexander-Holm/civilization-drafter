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

        public class Constructor
        {
            FakeData fakeData = new FakeData();

            [Fact]
            public void UsingConfig_ConfigValuesInViewModel()
            {
                // Arrange
                Civilization civ = new Civilization
                {
                    Name = "testCiv",
                    Leader = "testLeader"
                };
                var civList = fakeData.GetCivilizations(2);
                civList.Add(civ);
                var dlcList = fakeData.GetDlc();

                Config config = new Config
                {
                    UncheckedCivs = new List<CivSimple>
                    {
                        new CivSimple { Civ = civ.Name, Leader = civ.Leader }
                    },
                    AllowDuplicateCivs = true,
                    AllowDuplicateLeaders = false,
                    SelectedCivsPerPlayer = 5,
                    SelectedNumberOfPlayers = 5,
                };

                // Act
                ViewModel viewModel = new ViewModel(civList, dlcList, config);

                // Assert
                Assert.True(
                    viewModel.CivButtonList
                        .Where(x => x.Civ.Name == civ.Name && x.Civ.Leader == civ.Leader)
                        .All(x => x.IsChecked == false)
                    && viewModel.AllowDuplicateCivs == config.AllowDuplicateCivs
                    && viewModel.AllowDuplicateLeaders == config.AllowDuplicateLeaders
                    && viewModel.CivsPerPlayer.Selected == config.SelectedCivsPerPlayer
                    && viewModel.NumberOfPlayers.Selected == config.SelectedNumberOfPlayers
                );
            }

            [Fact]
            public void NoEmptyStringAsKeyInDlcSortedList_AddDlcCheckBoxWithEmptyAbbreviation()
            {
                // Arrange
                var civList = fakeData.GetCivilizations(10);
                var dlcSortedList = fakeData.GetDlc();
                dlcSortedList.Remove("");

                // Act
                var viewModel = new ViewModel(civList, dlcSortedList, null);

                // Assert
                Assert.Contains(viewModel.DlcCheckboxes, checkBox => checkBox.Dlc.Abbreviation == "");
            }
            [Fact]
            public void CivHasDlcNotInDlcJson_CivButtonIsCreated()
            {
                // Arrange
                Dlc existingDlc = new Dlc { Fullname = "Test", Abbreviation = "T" };
                SortedList<string, Dlc> dlcList = new SortedList<string, Dlc> { { existingDlc.Abbreviation, existingDlc } };

                Civilization civToTest = new Civilization
                {
                    Dlc = "NotInDlcList",
                };
                Civilization civBaseline = new Civilization
                {
                    Dlc = existingDlc.Abbreviation,
                };
                List<Civilization> civList = new List<Civilization> { civToTest, civBaseline };

                // Act
                var viewModel = new ViewModel(civList, dlcList, null);

                // Assert
                Assert.True(viewModel.CivButtonList.Count == civList.Count);
            }
            [Fact]
            public void ImageNotFound_BitmapImageInCivButtonIsNull()
            {
                // Arrange
                List<Civilization> civList = new List<Civilization>
            {
                new Civilization{ Image = "image does not exist"}
            };

                // Act
                var viewModel = new ViewModel(civList, fakeData.GetDlc(), null);

                // Assert
                Assert.True(viewModel.CivButtonList[0].BitmapImage is null);
            }
        }
        
        public class SelectedCivsCount
        {
            FakeData fakeData = new FakeData();

            [Theory]
            [InlineData(true, 3, 3, 9)]
            [InlineData(false, 3, 3, 3)]
            [InlineData(false, 3, 1, 3)]
            public void DuplicateCivsExist_ReturnIsBasedOnAllowDuplicateCivs(bool allowDuplicateCivs, int civsWithDuplicatesCount, int numberOfDuplicateInstances, int expectedResult)
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

                var viewModel = new ViewModel(civList, fakeData.GetDlc(), null);
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
            public void DuplicateLeadersExist_ReturnIsBasedOnAllowDuplicateLeaders(bool allowDuplicateLeaders, int civsWithDuplicatesCount, int numberOfDuplicateInstances, int expectedResult)
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

                var viewModel = new ViewModel(civList, fakeData.GetDlc(), null);
                viewModel.AllowDuplicateLeaders = allowDuplicateLeaders;
                viewModel.AllowDuplicateCivs = true;

                // Act
                int actual = viewModel.SelectedCivsCount;

                // Assert
                Assert.Equal(expectedResult, actual);
            }
        }
    }
}
