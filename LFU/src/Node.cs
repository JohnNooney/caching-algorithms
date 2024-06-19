using System.Runtime.InteropServices;

namespace LFU;

public class CacheNode
{
  public int Key {get;private set;}
  public int Value {get;set;}
  public int Frequency {get;set;}

  public CacheNode(int key, int value, int frequency = 1)
  {
    Key = key;
    Value = value;
    Frequency = frequency;
  }
}