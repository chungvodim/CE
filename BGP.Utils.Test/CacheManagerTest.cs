using BGP.Utils.CacheManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Test
{
    [TestClass]
    public class CacheManagerTest
    {
        private ICacheManager _cacheManager;
        private Random _random;

        [TestInitialize]
        public void Initialize()
        {
            _cacheManager = new CacheManager.MemoryCache.CacheManager("TEST");
            _random = new Random();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _cacheManager = null;
        }

        [TestMethod]
        public void BasicSetAndGetTest()
        {
            var key = "BasicSetAndGetTest";
            var value = _random.Next();

            _cacheManager.Set(key, value);

            var cacheValue = _cacheManager.Get(key);

            Assert.AreEqual(value, cacheValue);
        }

        [TestMethod]
        public void GenericTypeTest()
        {
            var key = "GenericTypeTest";
            var value = _random.Next();

            _cacheManager.Set(key, value);

            var cacheValue = _cacheManager.Get<int>(key);

            Assert.AreEqual(value, cacheValue);
        }

        [TestMethod]
        public void ValueFactoryTest()
        {
            var key = "ValueFactoryTest";
            var value = _random.Next();

            _cacheManager.GetOrUpdate(key, () => value);

            var cacheValue = _cacheManager.Get<int>(key);

            Assert.AreEqual(value, cacheValue);
        }

        [TestMethod]
        public async Task ValueFactoryAsyncTest()
        {
            var key = "ValueFactoryAsyncTest";
            var value = _random.Next();

            await _cacheManager.GetOrUpdateAsync(key, async () => await Task.Run(() => value));

            var cacheValue = _cacheManager.Get<int>(key);

            Assert.AreEqual(value, cacheValue);
        }

        [TestMethod]
        public void DurationTest()
        {
            var key = "DurationTest";
            var value = _random.Next();
            var duration = new TimeSpan(0, 0, 10);

            _cacheManager.Set(key, value, duration);

            System.Threading.Thread.Sleep(duration);

            var cacheValue = _cacheManager.Get(key);

            Assert.IsNull(cacheValue);
        }

        [TestMethod]
        public void SetAndUpdateTest()
        {
            var key = "SetAndUpdateTest";
            var value = _random.Next();

            _cacheManager.Set(key, value);

            var cacheValue = _cacheManager.Get(key);

            Assert.AreEqual(value, cacheValue);

            var newValue = _random.Next();

            _cacheManager.Set(key, newValue);

            var updatedValue = _cacheManager.Get(key);

            Assert.AreNotEqual(value, newValue);
            Assert.AreEqual(newValue, updatedValue);
        }

        [TestMethod]
        public void MultiThreadTest()
        {
            var key = "MultiThreadTest";
            var count = 0;
            int value = 0;

            for (int i = 0; i < 10; i++)
            {
                _cacheManager.GetOrUpdate(key, () => { count++; value = _random.Next(); return value; });
            }
            var cacheValue = _cacheManager.Get(key);

            Assert.IsTrue(count == 1);
            Assert.AreEqual(value, cacheValue);
        }
    }
}
