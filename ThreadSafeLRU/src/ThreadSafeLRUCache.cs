namespace ThreadSafeLRU;

using System.Collections.Concurrent;

public class ThreadSafeLRUCache {
    private int cacheCapacity;
    private ConcurrentDictionary<int, LinkedListNode<Node>> keyNodeConcurrentDictionary;
    private LinkedList<Node> nodeList;
    private Mutex mutex;

    public ThreadSafeLRUCache(int capacity) {
        cacheCapacity = capacity;
        keyNodeConcurrentDictionary = new ConcurrentDictionary<int,LinkedListNode<Node>>();
        nodeList = new LinkedList<Node>();
        mutex = new Mutex();
    }
    
    public int Get(int key) {
        LinkedListNode<Node> returnedLinkedListNode;

        if(!keyNodeConcurrentDictionary.TryGetValue(key, out returnedLinkedListNode))
        {
            Console.WriteLine($"Node not found for key: {key}");
            return -1;
        }

        moveNodeToFront(returnedLinkedListNode);

        Console.WriteLine($"Node with value: {returnedLinkedListNode.Value.Value} found and pushed to front of list.");
        
        return returnedLinkedListNode.Value.Value;
    }
    
    public void Put(int key, int value) {
        LinkedListNode<Node> returnedLinkedListNode;

        if(keyNodeConcurrentDictionary.TryGetValue(key, out returnedLinkedListNode)) 
        {
            Console.WriteLine($"Key already exists in pod with value {returnedLinkedListNode.Value.Value}. Updating value to {value}");

            returnedLinkedListNode.Value.Value = value;
            moveNodeToFront(returnedLinkedListNode);

            return;
        }
        
        LinkedListNode<Node> nodeToAdd = createNewNode(key,value);
        addNodeToCache(nodeToAdd);
        addNodeToList(nodeToAdd);

        Console.WriteLine($"Added new Node with key: {key} and value: {value}.");
        Console.WriteLine($"Current cache count:{keyNodeConcurrentDictionary.Count}");
        if(keyNodeConcurrentDictionary.Count > cacheCapacity)
        {
            evictLeastRecentlyUsed();
        }
    }

    private LinkedListNode<Node> createNewNode(int key, int value)
    {
        Node nodeToAdd = new Node(key, value);
        return new LinkedListNode<Node>(nodeToAdd);
    }

    private void moveNodeToFront(LinkedListNode<Node> node)
    {
        mutex.WaitOne();
        try
        {
            nodeList.Remove(node); // Can this be improved? Currently O(n)
            nodeList.AddFirst(node);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
        
    }

    private void addNodeToCache(LinkedListNode<Node> node)
    {
        keyNodeConcurrentDictionary.TryAdd(node.Value.Key, node);
    }

    private void addNodeToList(LinkedListNode<Node> node)
    {
        // Mutex lock this
        mutex.WaitOne();
        try
        {
            nodeList.AddFirst(node);
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private void evictLeastRecentlyUsed()
    {
        LinkedListNode<Node> leastRecentlyUsedNode;

        mutex.WaitOne();
        try 
        {
            leastRecentlyUsedNode = nodeList.Last;
            nodeList.RemoveLast();
        }
        finally
        {
            mutex.ReleaseMutex();
        }

        KeyValuePair<int, LinkedListNode<Node>> dictionaryKeyValueToRemove = new KeyValuePair<int, LinkedListNode<Node>>(leastRecentlyUsedNode.Value.Key, leastRecentlyUsedNode);

        keyNodeConcurrentDictionary.TryRemove(dictionaryKeyValueToRemove);
        Console.WriteLine($"Cache full. Evicted Node with key:{leastRecentlyUsedNode.Value.Key} and value{leastRecentlyUsedNode.Value.Value}");
    }
}