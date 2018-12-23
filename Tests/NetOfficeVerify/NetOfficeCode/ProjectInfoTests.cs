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
        [TestCase("MSForms")]
        [TestCase("MSHTML")]
        [TestCase("OWC10")]
        public void ProjectInfo_GeneratedFile_MatchesSourceCode(string projectName)
        {
            // Arrange
            var generatedFile = Path.Combine(this.GeneratedCodeDir, projectName, "Utils", "ProjectInfo.cs");
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, projectName, "Utils", "ProjectInfo.cs");

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile);
        }

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
        [TestCase("MSForms")]
        [TestCase("MSHTML")]
        [TestCase("OWC10")]
        public void AssemblyInfo_GeneratedFile_MatchesSourceCode(string projectName)
        {
            // Arrange
            var generatedFile = Path.Combine(this.GeneratedCodeDir, projectName, "AssemblyInfo.cs");
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, projectName, "AssemblyInfo.cs");

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile);
        }

        [Test]
        public void AssemblyInfo_NetOfficeProject_MatchesSourceCode()
        {
            // Arrange
            var generatedFile = Path.Combine(this.GeneratedCodeDir, "NetOffice", "Properties", "AssemblyInfo.cs");
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, "NetOffice", "Properties", "AssemblyInfo.cs");

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile);
        }
    }
}