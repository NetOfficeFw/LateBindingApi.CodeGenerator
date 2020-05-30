using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Access
{
    [TestFixture]
    public class AccessCoClassTests : ProjectTestContext
    {
        public AccessCoClassTests()
        : base("Access")
        {
        }

        [Test]
        [TestCase("_ControlInReportEvents.cs")]
        [TestCase("_CustomControlInReport.cs")]
        [TestCase("_CheckBoxInOption.cs")]
        [TestCase("_ChildLabel.cs")]
        [TestCase("_OptionButtonInOption.cs")]
        [TestCase("_PageHdrFtrInReport.cs")]
        [TestCase("_SectionInReport.cs")]
        [TestCase("_ToggleButtonInOption.cs")]
        [TestCase("AccessField.cs")]
        [TestCase("AdditionalData.cs")]
        [TestCase("AllDataAccessPages.cs")]
        [TestCase("AllDatabaseDiagrams.cs")]
        [TestCase("AllForms.cs")]
        [TestCase("AllFunctions.cs")]
        [TestCase("AllMacros.cs")]
        [TestCase("AllModules.cs")]
        [TestCase("AllQueries.cs")]
        [TestCase("AllReports.cs")]
        [TestCase("AllStoredProcedures.cs")]
        [TestCase("AllTables.cs")]
        [TestCase("AllViews.cs")]
        [TestCase("Application.cs")]
        [TestCase("Attachment.cs")]
        [TestCase("AutoCorrect.cs")]
        [TestCase("BoundObjectFrame.cs")]
        [TestCase("Class.cs")]
        [TestCase("CodeData.cs")]
        [TestCase("CodeProject.cs")]
        [TestCase("ComboBox.cs")]
        [TestCase("CommandButton.cs")]
        [TestCase("Control.cs")]
        [TestCase("CurrentData.cs")]
        [TestCase("CurrentProject.cs")]
        [TestCase("CustomControl.cs")]
        [TestCase("DataAccessPage.cs")]
        [TestCase("DefaultWebOptions.cs")]
        [TestCase("DependencyInfo.cs")]
        [TestCase("DependencyObjects.cs")]
        [TestCase("EmptyCell.cs")]
        [TestCase("Form.cs")]
        [TestCase("FormatCondition.cs")]
        [TestCase("FormOld.cs")]
        [TestCase("FormOldV10.cs")]
        [TestCase("GroupLevel.cs")]
        [TestCase("Hyperlink.cs")]
        [TestCase("CheckBox.cs")]
        [TestCase("Image.cs")]
        [TestCase("Label.cs")]
        [TestCase("Line.cs")]
        [TestCase("ListBox.cs")]
        [TestCase("LocalVar.cs")]
        [TestCase("LocalVars.cs")]
        [TestCase("MacroError.cs")]
        [TestCase("NavigationButton.cs")]
        [TestCase("NavigationControl.cs")]
        [TestCase("ObjectFrame.cs")]
        [TestCase("OptionButton.cs")]
        [TestCase("OptionGroup.cs")]
        [TestCase("Page.cs")]
        [TestCase("PageBreak.cs")]
        [TestCase("PaletteButton.cs")]
        [TestCase("Printer.cs")]
        [TestCase("Rectangle.cs")]
        [TestCase("References.cs")]
        [TestCase("Report.cs")]
        [TestCase("ReportOld.cs")]
        [TestCase("ReportOldV10.cs")]
        [TestCase("ReturnVar.cs")]
        [TestCase("ReturnVars.cs")]
        [TestCase("Section.cs")]
        [TestCase("SmartTag.cs")]
        [TestCase("SmartTagAction.cs")]
        [TestCase("SmartTagActions.cs")]
        [TestCase("SmartTagProperties.cs")]
        [TestCase("SmartTagProperty.cs")]
        [TestCase("SmartTags.cs")]
        [TestCase("SubForm.cs")]
        [TestCase("SubReport.cs")]
        [TestCase("TabControl.cs")]
        [TestCase("TempVar.cs")]
        [TestCase("TempVars.cs")]
        [TestCase("TextBox.cs")]
        [TestCase("ToggleButton.cs")]
        [TestCase("WebBrowserControl.cs")]
        [TestCase("WebOptions.cs")]
        [TestCase("WizHook.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }

        [TestCaseSource(nameof(ProjectCoClassFiles), new object [] { "Access" })]
        public void ClassesWithEventBindingInterface_EventBindingRegion_VerifyTheImplementationMatchesGoldFiles(string classFilename)
        {
            // Arrange
            var goldFile = Path.Combine(this.GoldProjectDir, "Classes", classFilename);
            var generatedFile = Path.Combine(this.GeneratedProjectDir, "Classes", classFilename);

            // Act
            var expected = GetEventBindingRegionFromFile(goldFile);
            var actual = GetEventBindingRegionFromFile(generatedFile);

            // Assert
            AssertDiff(expected, actual, goldFile, generatedFile, $"IEventBinding region in file {this.ProjectName}\\Classes\\{classFilename} does not match the gold file definition.");
        }
    }
}
