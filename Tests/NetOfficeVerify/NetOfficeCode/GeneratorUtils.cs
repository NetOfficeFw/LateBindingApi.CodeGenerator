﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LateBindingApi.CodeGenerator.CSharp;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode
{
    [TestFixture]
    public class GeneratorUtils : GeneratorTestContext
    {
        [Test]
        // apps
        [TestCase("Access")]
        [TestCase("Excel")]
        [TestCase("MSProject")]
        [TestCase("Word")]
        [TestCase("PowerPoint")]
        [TestCase("Publisher")]
        //[TestCase("Visio")]
        [TestCase("Outlook")]
        // libs
        [TestCase("Office")]
        [TestCase("VBIDE")]
        // supporting libs
        [TestCase("ADODB")]
        [TestCase("DAO")]
        [TestCase("MSComctlLib")]
        //[TestCase("MSDATASRC")]
        [TestCase("MSForms")]
        [TestCase("MSHTML")]
        [TestCase("OWC10")]
        [Ignore("Utility code.")]
        public void Enums_CopyGeneratedCode_ToTargetPath(string projectName)
        {
            // Arrange
            var sourcePath = Path.Combine(this.GeneratedCodeDir, projectName, "Enums");
            var targetPath = Path.Combine(this.GoldCodeDir, projectName, "Enums");

            // Act
            Directory.Delete(targetPath, true);
            DirectoryExtensions.CopyTo(sourcePath, targetPath);

            // Assert
        }

        [Test]
        // apps
        [TestCase("Access")]
        [TestCase("Excel")]
        [TestCase("MSProject")]
        [TestCase("Word")]
        [TestCase("PowerPoint")]
        [TestCase("Publisher")]
        //[TestCase("Visio")]
        [TestCase("Outlook")]
        // libs
        [TestCase("Office")]
        [TestCase("VBIDE")]
        // supporting libs
        [TestCase("ADODB")]
        [TestCase("DAO")]
        [TestCase("MSComctlLib")]
        //[TestCase("MSDATASRC")]
        [TestCase("MSForms")]
        [TestCase("MSHTML")]
        [TestCase("OWC10")]
        [Ignore("Utility code.")]
        public void Events_CopyGeneratedCode_ToTargetPath(string projectName)
        {
            // Arrange
            var sourcePath = Path.Combine(this.GeneratedCodeDir, projectName, EventApi.FolderName);
            var targetPath = Path.Combine(this.GoldCodeDir, projectName, EventApi.FolderName);

            // Act
            Directory.Delete(targetPath, true);
            DirectoryExtensions.CopyTo(sourcePath, targetPath);

            // Assert
        }
    }
}