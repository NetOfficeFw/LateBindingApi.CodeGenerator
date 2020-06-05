using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode
{
    [TestFixture]
    public class AssemblyInfoTests : GeneratorTestContext
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
        public void AssemblyInfo_GeneratedFile_MatchesSourceCode(string projectName)
        {
            // Arrange
            var assemblyInfoFilename = Path.Combine(projectName, "AssemblyInfo.cs");
            var generatedFile = Path.Combine(this.GeneratedCodeDir, assemblyInfoFilename);
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, assemblyInfoFilename);

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile, assemblyInfoFilename);
        }

        [Test]
        public void AssemblyInfo_NetOfficeProject_MatchesSourceCode()
        {
            // Arrange
            var assemblyInfoFilename = Path.Combine("NetOffice", "Properties", "AssemblyInfo.cs");
            var generatedFile = Path.Combine(this.GeneratedCodeDir, assemblyInfoFilename);
            var sourceCodeFile = Path.Combine(this.GoldCodeDir, assemblyInfoFilename);

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(sourceCodeFile, generatedFile, assemblyInfoFilename);
        }
    }
}