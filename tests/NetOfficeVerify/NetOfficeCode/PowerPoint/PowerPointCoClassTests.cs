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
    }
}
