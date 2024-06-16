namespace ThreadSafeLRU;
public class Node
{
    public int Key{get;set;}
    public int Value{get;set;}

    public Node(int key, int value)
    {
        this.Key = key;
        this.Value = value;
    }
}