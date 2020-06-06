using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode
{
    [TestFixture]
    public class ProjectInfoTests : GeneratorTestContext
    {
        [Test]
        // apps
        [TestCase("Access")]
        [TestCase("Excel")]
        [TestCase("MSProject")]
        [TestCase("Word")]
        [TestCase("PowerPoint")]
        [TestCase("Publisher")]
        [TestCase("Visio")]
        [TestCase("Outlook")]
        // libs
        [TestCase("Office")]
        [TestCase("VBIDE")]
        // supporting libs
        [TestCase("ADODB")]
        [TestCase("DAO")]
        [TestCase("MSComctlLib")]
        [TestCase("MSDATASRC")]
        [TestCase("MSForms", Ignore = "Code generation not supported")]
        [TestCase("MSHTML")]
        [TestCase("OWC10")]
        public void ProjectInfo_GeneratedFile_MatchesSourceCode(string projectName)
        {
            // Arrange
            string projectInfoFilename = Path.Combine(projectName, "Utils", "ProjectInfo.cs");
            var generatedFile = Path.Combine(this.GeneratedCodeDir, projectInfoFilename);
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, projectInfoFilename);

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile, projectInfoFilename);
        }
    }
}