﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Web.LibraryManager.Test
{
    [TestClass]
    public class SemanticVersionTest
    {
        [TestMethod]
        public void SemanticVersion_Parse_PrereleaseBeforeMetadata()
        {
            string test0 = "1.2.3-as-df+te+st";
            var semVer0 = SemanticVersion.Parse(test0);

            Assert.AreEqual(1, semVer0.Major);
            Assert.AreEqual(2, semVer0.Minor);
            Assert.AreEqual(3, semVer0.Patch);
            Assert.AreEqual("as-df", semVer0.PrereleaseVersion);
            Assert.AreEqual("te+st", semVer0.BuildMetadata);
        }

        [TestMethod]
        public void SemanticVersion_Parse_MetadataBeforePrerelease()
        {
            string test1 = "1.2.3+te+st-as-df";
            var semVer1 = SemanticVersion.Parse(test1);

            Assert.AreEqual(1, semVer1.Major);
            Assert.AreEqual(2, semVer1.Minor);
            Assert.AreEqual(3, semVer1.Patch);
            Assert.AreEqual("as-df", semVer1.PrereleaseVersion);
            Assert.AreEqual("te+st", semVer1.BuildMetadata);
        }

        [TestMethod]
        public void SemanticVersion_Parse_4PartVersionWithPrereleaseAndMetadata()
        {
            string test2 = "1.2.3.4+te+st-as-df";
            var semVer2 = SemanticVersion.Parse(test2);

            Assert.AreEqual(1, semVer2.Major);
            Assert.AreEqual(2, semVer2.Minor);
            Assert.AreEqual(3, semVer2.Patch);
            Assert.AreEqual("as-df", semVer2.PrereleaseVersion);
            Assert.AreEqual("te+st", semVer2.BuildMetadata);
        }

        [TestMethod]
        public void SemanticVersion_Sort()
        {
            var actual = new List<SemanticVersion>
            {
                SemanticVersion.Parse("2.2.3"),
                SemanticVersion.Parse("1.3.3"),
                SemanticVersion.Parse("1.2.4"),
                SemanticVersion.Parse("1.2.3"),
                SemanticVersion.Parse("1.2.3.4.5"),
                SemanticVersion.Parse("1.2.3+build23"),
                SemanticVersion.Parse("1.2.3+build22"),
                SemanticVersion.Parse("1.2.3-alpha")
            };

            actual.Sort();

            var expected = new List<SemanticVersion>
            {
                SemanticVersion.Parse("1.2.3-alpha"),
                SemanticVersion.Parse("1.2.3"),
                SemanticVersion.Parse("1.2.3+build22"), //Build metadata does not impact precedence
                SemanticVersion.Parse("1.2.3+build23"), //  but is sorted as the furthest fallback
                SemanticVersion.Parse("1.2.3.4.5"),
                SemanticVersion.Parse("1.2.4"),
                SemanticVersion.Parse("1.3.3"),
                SemanticVersion.Parse("2.2.3")
            };

            for (int i = 0; i < expected.Count; ++i)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
