using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Word
{
    [TestFixture]
    public class WordCoClassTests : ProjectTestContext
    {
        public WordCoClassTests()
        : base("Word")
        {
        }

        [Test]
        [TestCase("Application.cs")]
        [TestCase("Document.cs")]
        [TestCase("Font.cs")]
        [TestCase("LetterContent.cs")]
        [TestCase("OLEControl.cs")]
        [TestCase("ParagraphFormat.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }

        [TestCaseSource(nameof(ProjectCoClassFiles), new object[] { "Word" })]
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
