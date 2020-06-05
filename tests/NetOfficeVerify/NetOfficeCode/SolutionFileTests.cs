using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace NetOfficeVerify.NetOfficeCode
{
    [TestFixture]
    public class SolutionFileTests : GeneratorTestContext
    {
        public static readonly string NetOfficeSolutionName = "NetOffice.sln";

        [Test]
        public void SolutionFile_MatchesGoldFile()
        {
            // Arrange
            var generatedSolutionFile = Path.Combine(this.GeneratedCodeDir, NetOfficeSolutionName);
            var goldSolutionFile = Path.Combine(this.GoldCodeDir, NetOfficeSolutionName);

            // Act
            // nop

            // Assert
            FileAssertEx.AreEqual(goldSolutionFile, generatedSolutionFile, NetOfficeSolutionName);
        }
    }
}
