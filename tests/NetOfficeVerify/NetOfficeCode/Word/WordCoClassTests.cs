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
    }
}
