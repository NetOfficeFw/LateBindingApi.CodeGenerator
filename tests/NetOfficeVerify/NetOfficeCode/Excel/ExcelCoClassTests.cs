﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Excel
{
    [TestFixture]
    public class ExcelCoClassTests : ProjectTestContext
    {
        public ExcelCoClassTests()
        : base("Excel")
        {
        }

        [Test]
        [TestCase("Application.cs")]
        [TestCase("Chart.cs")]
        [TestCase("OLEObject.cs")]
        [TestCase("QueryTable.cs")]
        [TestCase("Workbook.cs")]
        [TestCase("Worksheet.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }
    }
}