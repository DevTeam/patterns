﻿namespace DevTeam.TestTool.Test.Mocks
{
    using System;

    using Contracts;

    [TestFixture]
    public class TestFixtureFail
    {
        [Test]
        public void TestFail()
        {
            throw new Exception("Error");
        }
    }
}
