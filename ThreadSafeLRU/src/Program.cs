namespace ThreadSafeLRU;

class Program
{
    private static readonly int TaskCount = 4;
    private static readonly int NumOperationsPerThread = 5;
    private static readonly int cacheSize = 5;

    static void Main(string[] args)
    {
        var cache = new ThreadSafeLRUCache(cacheSize);

        var tasks = new Task[TaskCount];

        for (int currentTask = 0; currentTask < TaskCount; currentTask++)
        {
            Console.WriteLine($"\nStarting task: {currentTask}\n");

            tasks[currentTask] = Task.Run(
                () => RunOperations(cache, currentTask));
        }

        Task.WaitAll(tasks);

        Console.WriteLine("\nAll operations completed!");
    }

    private static void RunOperations(ThreadSafeLRUCache cache, int currentTaskNumber)
    {
        Random random = new Random();
        for (int currentTaskOperationCount = 0; currentTaskOperationCount < NumOperationsPerThread; currentTaskOperationCount++)
        {
            int key = random.Next(100);
            int value = random.Next(100);

            // Randomly choose between Get and Put
            if (random.NextDouble() < 0.5)
            {
                Console.WriteLine($"\nTask Operation: {currentTaskOperationCount} for task: {currentTaskNumber} is getting key: {key}\n");
                cache.Get(key);
            }
            else
            {
                Console.WriteLine($"\nTask Operation: {currentTaskOperationCount} for task: {currentTaskNumber} is putting key,value: {key},{value}\n");
                cache.Put(key, value);
            }
        }
    }
}
