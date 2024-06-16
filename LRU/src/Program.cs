namespace LRU;

class Program
{
    static void Main(string[] args)
    {
        /**
        * Your LRUCache object will be instantiated and called as such:
        * LRUCache obj = new LRUCache(capacity);
        * int param_1 = obj.Get(key);
        * obj.Put(key,value);
        */
        int lruCacheCapacity = 2;
        LRUCache lruCache = new LRUCache(lruCacheCapacity);

        Node node1 = new Node(1,2);
        Node node2 = new Node(2,3);
        Node node3 = new Node(3,1);
        
        Node[] nodeArray = new Node[3] {node1, node2, node3} ;

        foreach (Node node in nodeArray)
        {
            lruCache.Put(node.Key, node.Value);
        }

        int cacheFirstValue = lruCache.Get(1);
        int cacheSecondValue = lruCache.Get(2);
        int cacheThirdValue = lruCache.Get(3);
        
        Console.WriteLine($"Retrieved value: {cacheFirstValue} for key: 1");
        Console.WriteLine($"Retrieved value: {cacheSecondValue} for key: 2");
        Console.WriteLine($"Retrieved value: {cacheThirdValue} for key: 3");
    }
}
