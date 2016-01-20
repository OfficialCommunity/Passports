using System;
using NUnit.Framework;
using OCC.Passports.Common.Contracts.Infrastructure;
using OCC.Passports.Common.Contracts.Services;
using OCC.Passports.Common.Domains;
using OCC.Passports.Common.Tests.Models;
using OCC.Passports.Storage.Null;

namespace OCC.Passports.Common.Tests
{
    [TestFixture]
    public class UserPassportAttributeTests
    {
        private IPassportStorageService _storage;
        IPassport _passport;
        UserPassportAttributeClass _target;

        [SetUp]
        public void Setup()
        {
            _storage = new PassportStorageService();
            _passport = new Passport(_storage);
            _target = new UserPassportAttributeClass(_passport);
        }

        [Test]
        public void NoSession()
        {
            var result = _target.NoSession();
            Assert.IsTrue(result.Response);
        }

        [Test]
        public void ValidSession()
        {
            var result = _target.ValidSession("1");
            Assert.IsTrue(result.Response);
        }

        [Test]
        public void InvalidSession()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _target.InvalidSession("1"));
        }
    }
}
