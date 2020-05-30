using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Visio
{
    [TestFixture]
    public class VisioCoClassTests : ProjectTestContext
    {
        public VisioCoClassTests()
        : base("Visio")
        {
        }

        [Test]
        [TestCase("AccelItem.cs")]
        [TestCase("AccelItems.cs")]
        [TestCase("AccelTable.cs")]
        [TestCase("AccelTables.cs")]
        [TestCase("Addon.cs")]
        [TestCase("Addons.cs")]
        [TestCase("Application.cs")]
        [TestCase("ApplicationSettings.cs")]
        [TestCase("Cell.cs")]
        [TestCase("CoauthMergeEvent.cs")]
        [TestCase("Color.cs")]
        [TestCase("Colors.cs")]
        [TestCase("Comment.cs")]
        [TestCase("Comments.cs")]
        [TestCase("Connect.cs")]
        [TestCase("Connects.cs")]
        [TestCase("ContainerProperties.cs")]
        [TestCase("Curve.cs")]
        [TestCase("DataColumn.cs")]
        [TestCase("DataColumns.cs")]
        [TestCase("DataConnection.cs")]
        [TestCase("DataRecordset.cs")]
        [TestCase("DataRecordsetChangedEvent.cs")]
        [TestCase("DataRecordsets.cs")]
        [TestCase("Document.cs")]
        [TestCase("Documents.cs")]
        [TestCase("Event.cs")]
        [TestCase("EventList.cs")]
        [TestCase("Extender.cs")]
        [TestCase("Font.cs")]
        [TestCase("Fonts.cs")]
        [TestCase("GraphicItem.cs")]
        [TestCase("GraphicItems.cs")]
        [TestCase("Hyperlink.cs")]
        [TestCase("Hyperlinks.cs")]
        [TestCase("Characters.cs")]
        [TestCase("InvisibleApp.cs")]
        [TestCase("KeyboardEvent.cs")]
        [TestCase("Layer.cs")]
        [TestCase("Layers.cs")]
        [TestCase("Master.cs")]
        [TestCase("Masters.cs")]
        [TestCase("MasterShortcut.cs")]
        [TestCase("MasterShortcuts.cs")]
        [TestCase("Menu.cs")]
        [TestCase("MenuItem.cs")]
        [TestCase("MenuItems.cs")]
        [TestCase("Menus.cs")]
        [TestCase("MenuSet.cs")]
        [TestCase("MenuSets.cs")]
        [TestCase("MouseEvent.cs")]
        [TestCase("MovedSelectionEvent.cs")]
        [TestCase("MSGWrap.cs")]
        [TestCase("OLEObject.cs")]
        [TestCase("OLEObjects.cs")]
        [TestCase("Page.cs")]
        [TestCase("Pages.cs")]
        [TestCase("Path.cs")]
        [TestCase("Paths.cs")]
        [TestCase("RelatedShapePairEvent.cs")]
        [TestCase("ReplaceShapesEvent.cs")]
        [TestCase("Row.cs")]
        [TestCase("Section.cs")]
        [TestCase("Selection.cs")]
        [TestCase("ServerPublishOptions.cs")]
        [TestCase("Shape.cs")]
        [TestCase("Shapes.cs")]
        [TestCase("StatusBar.cs")]
        [TestCase("StatusBarItem.cs")]
        [TestCase("StatusBarItems.cs")]
        [TestCase("StatusBars.cs")]
        [TestCase("Style.cs")]
        [TestCase("Styles.cs")]
        [TestCase("Toolbar.cs")]
        [TestCase("ToolbarItem.cs")]
        [TestCase("ToolbarItems.cs")]
        [TestCase("Toolbars.cs")]
        [TestCase("ToolbarSet.cs")]
        [TestCase("ToolbarSets.cs")]
        [TestCase("UIObject.cs")]
        [TestCase("Validation.cs")]
        [TestCase("ValidationIssue.cs")]
        [TestCase("ValidationIssues.cs")]
        [TestCase("ValidationRule.cs")]
        [TestCase("ValidationRules.cs")]
        [TestCase("ValidationRuleSet.cs")]
        [TestCase("ValidationRuleSets.cs")]
        [TestCase("Window.cs")]
        [TestCase("Windows.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }

        [TestCaseSource(nameof(ProjectCoClassFiles), new object[] { "Publisher" })]
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
