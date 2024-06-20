namespace LFU;

public class CacheFrequency
{
    private Dictionary<int, LinkedList<CacheNode>> dataFrequency;

    public CacheFrequency()
    {
        dataFrequency = new Dictionary<int,LinkedList<CacheNode>>();
    }

    public void updateFrequencyDictionaryWithNewNodeValue(LinkedListNode<CacheNode> newNode, LinkedListNode<CacheNode> oldNode)
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

    public void incrementCacheNodeFrequency(LinkedListNode<CacheNode> node)
    {
        removeFromFrequencyDictionary(node);

        node.Value.Frequency++;

        addToFrequencyDictionary(node);
    }

    public void removeFromFrequencyDictionary(LinkedListNode<CacheNode> node)
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

    public void addToFrequencyDictionary(LinkedListNode<CacheNode> node)
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

    public LinkedListNode<CacheNode> leastFrequentlyUsedNode()
    {   
        int lowestFrequencyKey = dataFrequency.Keys.Min(); // O(n) where n is size of cache

        LinkedList<CacheNode> nodesAtFrequency;
        dataFrequency.TryGetValue(lowestFrequencyKey, out nodesAtFrequency);

        return nodesAtFrequency.Last;
    }
}