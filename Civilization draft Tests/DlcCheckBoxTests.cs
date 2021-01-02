using Civilization_draft.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Civilization_draft_Tests
{
    public class DlcCheckBoxTests
    {
        FakeData fakeData = new FakeData();

        [Theory]
        // Change to true
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(null, true)]
        // Change to false
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(null, false)]
        public void AllSelectedChanged_ChangeValue_IsCheckedInConnectedCivButtonsMatchesNewValue(bool? initialValue, bool newValue)
        {
            //Arrange
            var civList = fakeData.GetCivilizations(10);
            var dlcSortedList = fakeData.GetDlc();
            string dlcAbbreviation = fakeData.dlcAbbreviations.Values[0];

            var viewModel = new ViewModel(civList, dlcSortedList);
            var dlcCheckBox = viewModel.DlcCheckboxes.First(dlcCheckBox => dlcCheckBox.Dlc.Abbreviation == dlcAbbreviation);
            dlcCheckBox.AllSelected = initialValue;

            // Act
            dlcCheckBox.AllSelected = newValue;

            // Assert
            Assert.DoesNotContain(
                viewModel.CivButtonList.Where(civButton => civButton.Dlc.Abbreviation == dlcAbbreviation),
                civButton => civButton.IsChecked != newValue
            );
        }
    }
}
