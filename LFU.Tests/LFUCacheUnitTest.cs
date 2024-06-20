using System.Text;
using LFU;

namespace LFUTests.Tests;
public class LFUCacheShould
{
    private LFUCache lfuCache;

    [SetUp]
    public void Setup()
    {
        lfuCache = new LFUCache(2);
    }

    [Test]
    public void GetReturnsMinusOneIfCacheEmpty()
    {
        KeyValuePair<int,int> keyValuePair = new KeyValuePair<int,int>(5,-1);

        int returnedValue = lfuCache.Get(keyValuePair.Key);
        
        Assert.That(returnedValue, Is.EqualTo(keyValuePair.Value));
    }

    [Test]
    public void PutAndGetExpectedValue()
    {
        KeyValuePair<int,int> keyValuePair = new KeyValuePair<int,int>(1,1);

        lfuCache.Put(keyValuePair.Key, keyValuePair.Value);

        int returnedValue = lfuCache.Get(keyValuePair.Value);
        Assert.That(returnedValue, Is.EqualTo(keyValuePair.Value));
    }

    [Test]
    public void EvictLeastFrequentlyUsedAfterMaxCacheSizeReached()
    {
        KeyValuePair<int,int>[] keyValuePairs = new KeyValuePair<int,int>[3] {
            new KeyValuePair<int,int>(1,1),
            new KeyValuePair<int,int>(2,3),
            new KeyValuePair<int,int>(3,5)
        };

        foreach(KeyValuePair<int,int> keyValuePair in keyValuePairs)
        {
            lfuCache.Put(keyValuePair.Key, keyValuePair.Value);
        }

        int returnedValue = lfuCache.Get(keyValuePairs[0].Key);
        Assert.That(returnedValue, Is.EqualTo(-1));
    }

    [Test]
    public void UpdateValueIfKeyAlreadyExists()
    {
        KeyValuePair<int,int>[] keyValuePairs = new KeyValuePair<int,int>[2] {
            new KeyValuePair<int,int>(1,1),
            new KeyValuePair<int,int>(1,3)
        };

        foreach(KeyValuePair<int,int> keyValuePair in keyValuePairs)
        {
            lfuCache.Put(keyValuePair.Key, keyValuePair.Value);
        }

        int returnedValue = lfuCache.Get(keyValuePairs[0].Key);
        Assert.That(returnedValue, Is.EqualTo(keyValuePairs[1].Value));
    }

    [Test]
    public void AddNothingIfCacheSizeIsZero()
    {
        lfuCache = new LFUCache(0);
        KeyValuePair<int,int>[] keyValuePairs = new KeyValuePair<int,int>[2] {
            new KeyValuePair<int,int>(6,7),
            new KeyValuePair<int,int>(8,3)
        };

        foreach(KeyValuePair<int,int> keyValuePair in keyValuePairs)
        {
            lfuCache.Put(keyValuePair.Key, keyValuePair.Value);
        }

        int returnedValue = lfuCache.Get(keyValuePairs[1].Key);
        Assert.That(returnedValue, Is.EqualTo(-1));
    }
}