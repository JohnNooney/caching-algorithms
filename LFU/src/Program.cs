namespace LFU;

class Program
{
    private const int CACHE_SIZE = 2;
    static void Main(string[] args)
    {
        LFUCache lFUCache = new LFUCache(2);
    }
}
