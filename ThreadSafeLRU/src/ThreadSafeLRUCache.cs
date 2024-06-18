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
        mutex.WaitOne();
        try {
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
        finally
        {
            mutex.ReleaseMutex();
        }
    }
    
    public void Put(int key, int value) {
        mutex.WaitOne();
        try {
            LinkedListNode<Node> returnedLinkedListNode;
            if(keyNodeConcurrentDictionary.TryGetValue(key, out returnedLinkedListNode)) 
            {
                Console.WriteLine($"Key already exists in pod with value {returnedLinkedListNode.Value.Value}. Updating value to {value}");

                returnedLinkedListNode.Value.Value = value;
                moveNodeToFront(returnedLinkedListNode);

                return;
            }
            
            LinkedListNode<Node> nodeToAdd = createNewNode(key,value);
            
            addNodeToList(nodeToAdd); // this should be interlocked
            addNodeToCache(nodeToAdd);

            Console.WriteLine($"Added new Node with key: {key} and value: {value}.");
            Console.WriteLine($"Current cache count:{keyNodeConcurrentDictionary.Count}");
            if(keyNodeConcurrentDictionary.Count > cacheCapacity)
            {
                evictLeastRecentlyUsed();
            }
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }

    private LinkedListNode<Node> createNewNode(int key, int value)
    {
        Node nodeToAdd = new Node(key, value);
        return new LinkedListNode<Node>(nodeToAdd);
    }

    private void moveNodeToFront(LinkedListNode<Node> node)
    {
        nodeList.Remove(node); // Can this be improved? Currently O(n)
        nodeList.AddFirst(node);  
    }

    private void addNodeToCache(LinkedListNode<Node> node)
    {
        keyNodeConcurrentDictionary.TryAdd(node.Value.Key, node);
    }

    private void addNodeToList(LinkedListNode<Node> node)
    {
        nodeList.AddFirst(node);
    }

    private void evictLeastRecentlyUsed()
    {
        LinkedListNode<Node> leastRecentlyUsedNode;

        leastRecentlyUsedNode = nodeList.Last;
        nodeList.RemoveLast();

        KeyValuePair<int, LinkedListNode<Node>> dictionaryKeyValueToRemove = new KeyValuePair<int, LinkedListNode<Node>>(leastRecentlyUsedNode.Value.Key, leastRecentlyUsedNode);
        keyNodeConcurrentDictionary.TryRemove(dictionaryKeyValueToRemove);

        Console.WriteLine($"Cache full. Evicted Node with key:{leastRecentlyUsedNode.Value.Key} and value{leastRecentlyUsedNode.Value.Value}");
    }
}