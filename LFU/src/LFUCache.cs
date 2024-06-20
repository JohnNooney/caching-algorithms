
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace LFU;

public class LFUCache
{
  private Dictionary<int, LinkedListNode<CacheNode>> cacheData;
  private CacheFrequency cacheFrequency;
  private int cacheMaxSize;

  public LFUCache(int cacheSize)
  {
    cacheMaxSize = cacheSize;
    cacheData = new Dictionary<int, LinkedListNode<CacheNode>>();
    cacheFrequency = new CacheFrequency();
  }

  public int Get(int key)
  {
    Console.WriteLine($"Getting cached data for key: {key}");

    LinkedListNode<CacheNode> returnedCacheDataNode;
    if(!cacheData.TryGetValue(key, out returnedCacheDataNode))
    {
      Console.WriteLine($"No cached data for key: {key}. Returning -1.");
      return -1;
    }
    
    Console.WriteLine($"Cached data for key: {key} found. Incrementing frequency.");
    cacheFrequency.incrementCacheNodeFrequency(returnedCacheDataNode);

    return returnedCacheDataNode.Value.Value;
  }

  public void Put(int key, int value)
  {
    LinkedListNode<CacheNode> newNode  = createLinkedListCacheNode(key, value);

    if(cacheData.ContainsKey(key))
    {
      LinkedListNode<CacheNode> oldNode = cacheData[key];
      newNode.Value.Frequency  = oldNode.Value.Frequency;
      
      updateCacheDictionaryWithNewNodeValue(newNode);
      cacheFrequency.updateFrequencyDictionaryWithNewNodeValue(newNode, oldNode);
    }
    else
    {
      addToCacheDictionary(newNode);
      cacheFrequency.addToFrequencyDictionary(newNode);
    }

    if(cacheData.Count > cacheMaxSize)
    {
      LinkedListNode<CacheNode> lfuNode = cacheFrequency.leastFrequentlyUsedNode();
      
      cacheFrequency.removeFromFrequencyDictionary(lfuNode);
      cacheData.Remove(lfuNode.Value.Key);
      
      Console.WriteLine($"Cache full. Evicted least frequently used node: {lfuNode.Value.Key},{lfuNode.Value.Value}.");
    }
  }

  private void updateCacheDictionaryWithNewNodeValue(LinkedListNode<CacheNode> nodeToAdd)
  {
    cacheData[nodeToAdd.Value.Key] = nodeToAdd;
  }

  private LinkedListNode<CacheNode> createLinkedListCacheNode(int key, int value, int frequency = 1)
  {
    CacheNode cacheNode = new CacheNode(key, value, frequency);
    return new LinkedListNode<CacheNode>(cacheNode);
  }
  
  private void addToCacheDictionary(LinkedListNode<CacheNode> nodeToAdd)
  {
    cacheData.TryAdd(nodeToAdd.Value.Key, nodeToAdd);
    Console.WriteLine($"Added Node {nodeToAdd.Value.Key},{nodeToAdd.Value.Value} to Cache Data.");
  }   
}