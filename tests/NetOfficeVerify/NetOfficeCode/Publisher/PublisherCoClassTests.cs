using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode.Publisher
{
    [TestFixture]
    public class PublisherCoClassTests : ProjectTestContext
    {
        public PublisherCoClassTests()
        : base("Publisher")
        {
        }

        [Test]
        [TestCase("Application.cs")]
        [TestCase("Document.cs")]
        public void CoClasses_VerifyDelegates(string classFilename)
        {
            // Arrange & Act & Assert
            this.VerifyCoClassDelegatesInFile(classFilename);
        }
    }
}
