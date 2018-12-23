using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DiffMatchPatch;
using NUnit.Framework;

namespace NetOfficeVerify
{
    public class FileAssertEx
    {
        public static void AreEqual(string expected, string actual)
        {
            var expectedFile = File.ReadAllText(expected);
            var actualFile = File.ReadAllText(actual);

            var dmp = new diff_match_patch();
            var diff = dmp.diff_main(expectedFile, actualFile);
            dmp.diff_cleanupSemantic(diff);

            if (diff.Count == 1)
            {
                if (diff[0].operation == Operation.EQUAL)
                {
                    Assert.Pass();
                }
            }

            if (diff.Count > 0)
            {
                var delta = dmp.diff_prettyText(diff);
                Assert.Fail(delta);
            }
        }
    }
}
