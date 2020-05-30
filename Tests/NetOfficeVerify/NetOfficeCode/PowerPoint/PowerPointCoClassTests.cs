using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.PowerPoint
{
    [TestFixture]
    public class PowerPointCoClassTests : ProjectTestContext
    {
        public PowerPointCoClassTests()
        : base("PowerPoint")
        {
        }

        [Test]
        [TestCase("Application.cs")]
        [TestCase("Master.cs")]
        [TestCase("OLEControl.cs")]
        [TestCase("PowerRex.cs")]
        [TestCase("Presentation.cs")]
        [TestCase("Slide.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }

        [TestCaseSource(nameof(ProjectCoClassFiles), new object[] { "PowerPoint" })]
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
