namespace ThreadSafeLRU;

class Program
{
    private static readonly int NumThreads = 4;
    private static readonly int NumOperationsPerThread = 5;
    private static readonly int cacheSize = 10;

    static void Main(string[] args)
    {
        var cache = new ThreadSafeLRUCache(cacheSize);

        var threads = new Thread[NumThreads];

        for (int currentThread = 0; currentThread < NumThreads; currentThread++)
        {
            Console.WriteLine($"\nStarting thread: {currentThread}\n");
            threads[currentThread] = new Thread(() => RunOperations(cache, currentThread));
            threads[currentThread].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("\nAll operations completed!");
    }

    private static void RunOperations(ThreadSafeLRUCache cache, int currentThreadNumber)
    {
        Random random = new Random();
        for (int currentThreadOperationCount = 0; currentThreadOperationCount < NumOperationsPerThread; currentThreadOperationCount++)
        {
            int key = random.Next(100);
            int value = random.Next(100);

            // Randomly choose between Get and Put
            if (random.NextDouble() < 0.5)
            {
                Console.WriteLine($"\nThread Operation: {currentThreadOperationCount} for thread: {currentThreadNumber} is getting key: {key}");
                cache.Get(key);
            }
            else
            {
                Console.WriteLine($"\nThread Operation: {currentThreadOperationCount} for thread: {currentThreadNumber} is putting key,value: {key},{value}");
                cache.Put(key, value);
            }
        }
    }
}
