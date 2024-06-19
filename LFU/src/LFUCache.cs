
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace LFU;

public class LFUCache
{
  private Dictionary<int, LinkedListNode<CacheNode>> cacheData;
  private Dictionary<int, LinkedList<CacheNode>> dataFrequency;
  private int cacheMaxSize;

  public LFUCache(int cacheSize)
  {
    cacheMaxSize = cacheSize;
    cacheData = new Dictionary<int, LinkedListNode<CacheNode>>();
    dataFrequency = new Dictionary<int, LinkedList<CacheNode>>();
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
    incrementCacheNodeFrequency(returnedCacheDataNode);

    return returnedCacheDataNode.Value.Value;
  }

  public void Put(int key, int value)
  {
    if(cacheData.ContainsKey(key))
    {
      LinkedListNode<CacheNode> oldNode = cacheData[key];
      LinkedListNode<CacheNode> newNode  = createLinkedListCacheNode(key, value, oldNode.Value.Frequency);
      
      updateCacheDictionaryWithNewNodeValue(newNode);
      updateFrequencyDictionaryWithNewNodeValue(newNode, oldNode);
    }
    else
    {
      LinkedListNode<CacheNode> newNode  = createLinkedListCacheNode(key, value);

      addToCacheDictionary(newNode);
      addToFrequencyDictionary(newNode);
    }

    if(cacheData.Count > cacheMaxSize)
    {
      Console.WriteLine($"Cache full evicting least frequently used node.");
      evictLeastFrequentlyUsed();
    }
  }


  private void updateCacheDictionaryWithNewNodeValue(LinkedListNode<CacheNode> nodeToAdd)
  {
    cacheData[nodeToAdd.Value.Key] = nodeToAdd;
  }

  private void updateFrequencyDictionaryWithNewNodeValue(LinkedListNode<CacheNode> newNode, LinkedListNode<CacheNode> oldNode)
  {
    LinkedList<CacheNode> nodeListToUpdate;
    if(dataFrequency.TryGetValue(newNode.Value.Frequency, out nodeListToUpdate))
    {
      nodeListToUpdate.Remove(oldNode);
      nodeListToUpdate.AddFirst(newNode);
      dataFrequency[newNode.Value.Key] = nodeListToUpdate;
      Console.WriteLine($"Update Node {oldNode.Value.Key},{oldNode.Value.Value} with new value: {newNode.Value.Value} at frequency: {newNode.Value.Frequency}");
    }
  }

    private void incrementCacheNodeFrequency(LinkedListNode<CacheNode> node)
  {
    removeFromFrequencyDictionary(node);

    node.Value.Frequency++;

    addToFrequencyDictionary(node);
  }

  private void removeFromFrequencyDictionary(LinkedListNode<CacheNode> node)
  {
    LinkedList<CacheNode> nodeListAtFrequency;
    if(!dataFrequency.TryGetValue(node.Value.Frequency, out nodeListAtFrequency))
    {
      Console.WriteLine($"Unable to remove Node {node.Value.Key},{node.Value.Value}. No Frequency List at frequency: {node.Value.Frequency}");
      return;
    }
    
    nodeListAtFrequency.Remove(node);
    Console.WriteLine($"Removed Node {node.Value.Key},{node.Value.Value} at frequency: {node.Value.Frequency}");

    // check if frequencyDictionary is empty
    if(nodeListAtFrequency.First == null)
    {
      dataFrequency.Remove(node.Value.Frequency);
      Console.WriteLine($"List at frequency: {node.Value.Frequency} is empty. Removed List.");
    }
  }

  private void addToFrequencyDictionary(LinkedListNode<CacheNode> node)
  {
    LinkedList<CacheNode> nodeListAtFrequency;
    //get existing Frequency list if exists, otherwise create new
    if(dataFrequency.TryGetValue(node.Value.Frequency, out nodeListAtFrequency))
    {
      nodeListAtFrequency.AddFirst(node);

      Console.WriteLine($"Frequency list exists at frequency: {node.Value.Frequency}. Appended Node {node.Value.Key},{node.Value.Value}.");
    }
    else
    {
      nodeListAtFrequency = new LinkedList<CacheNode>();
      nodeListAtFrequency.AddFirst(node);

      Console.WriteLine($"Frequency list doesn't exist at frequency: {node.Value.Frequency}. Added Node {node.Value.Key},{node.Value.Value} to new list.");
    }

    if(dataFrequency.TryAdd(node.Value.Frequency, nodeListAtFrequency))
    {
      Console.WriteLine($"Added Node {node.Value.Key},{node.Value.Value} to frequency: {node.Value.Frequency}.");
    }
    else
    {
      dataFrequency[node.Value.Frequency] = nodeListAtFrequency;
      Console.WriteLine($"Updated frequency: {node.Value.Frequency} to add Node {node.Value.Key},{node.Value.Value}.");
    }
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

  private void evictLeastFrequentlyUsed()
  {
    int lowestFrequencyKey = dataFrequency.Keys.Min(); // O(n) where n is size of cache

    LinkedList<CacheNode> nodesAtFrequency;
    dataFrequency.TryGetValue(lowestFrequencyKey, out nodesAtFrequency);

    cacheData.Remove(nodesAtFrequency.Last.Value.Key);
    nodesAtFrequency.RemoveLast();
    
    Console.WriteLine($"Removed least recently used node at frequency: {lowestFrequencyKey}.");

    if(nodesAtFrequency.First == null)
    {
      dataFrequency.Remove(lowestFrequencyKey);
      Console.WriteLine($"List at frequency: {lowestFrequencyKey} is empty. Removed List.");
    }
    else
    {
      dataFrequency[lowestFrequencyKey] = nodesAtFrequency;
      Console.WriteLine($"List at frequency: {lowestFrequencyKey} updated with node removed.");
    }
  }       
}