namespace LRU;
public class LRUCache {
    private int cacheCapacity;
    private Dictionary<int, LinkedListNode<Node>> keyNodeDictionary;
    private LinkedList<Node> nodeList;

    public LRUCache(int capacity) {
        cacheCapacity = capacity;
        keyNodeDictionary = new Dictionary<int,LinkedListNode<Node>>();
        nodeList = new LinkedList<Node>();
    }
    
    public int Get(int key) {
        LinkedListNode<Node> returnedLinkedListNode;

        if(!keyNodeDictionary.TryGetValue(key, out returnedLinkedListNode))
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
        
        if(keyNodeDictionary.TryGetValue(key, out returnedLinkedListNode)) 
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
        Console.WriteLine($"Current cache count:{keyNodeDictionary.Count}");
        if(keyNodeDictionary.Count > cacheCapacity)
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
        nodeList.Remove(node);
        nodeList.AddFirst(node);
    }

    private void addNodeToCache(LinkedListNode<Node> node)
    {
        keyNodeDictionary.TryAdd(node.Value.Key, node);
    }

    private void addNodeToList(LinkedListNode<Node> node)
    {
        nodeList.AddFirst(node);
    }

    private void evictLeastRecentlyUsed()
    {
        LinkedListNode<Node> leastRecentlyUsedNode = nodeList.Last;
        nodeList.RemoveLast();
        keyNodeDictionary.Remove(leastRecentlyUsedNode.Value.Key);

        Console.WriteLine($"Cache full. Evicted Node with key:{leastRecentlyUsedNode.Value.Key} and value{leastRecentlyUsedNode.Value.Value}");
    }
}