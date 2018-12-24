using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Office
{
    [TestFixture]
    public class OfficeCoClassTests : ProjectTestContext
    {
        public OfficeCoClassTests()
        : base("Office")
        {
        }

        [Test]
        [TestCase("CommandBarButton.cs")]
        [TestCase("CommandBarComboBox.cs")]
        [TestCase("CommandBars.cs")]
        [TestCase("CustomTaskPane.cs")]
        [TestCase("CustomXMLPart.cs")]
        [TestCase("CustomXMLParts.cs")]
        [TestCase("CustomXMLSchemaCollection.cs")]
        [TestCase("MsoEnvelope.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }
    }
}
