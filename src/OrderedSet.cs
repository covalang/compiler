using System;
using System.Collections;
using System.Collections.Generic;

public class OrderedSet<T> : ICollection<T> where T : notnull
{
    private readonly IDictionary<T, LinkedListNode<T>> dictionary;
    private readonly LinkedList<T> linkedList;
 
    public OrderedSet() : this(EqualityComparer<T>.Default) { }
 
    public OrderedSet(IEqualityComparer<T> comparer)
    {
        dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
        linkedList = new LinkedList<T>();
    }
 
    public Int32 Count => dictionary.Count;

    public virtual Boolean IsReadOnly => dictionary.IsReadOnly;

    void ICollection<T>.Add(T item) => Add(item);

    public void Clear()
    {
        linkedList.Clear();
        dictionary.Clear();
    }
 
    public Boolean Remove(T item)
    {
        if (!dictionary.TryGetValue(item, out var node))
            return false;
        dictionary.Remove(item);
        linkedList.Remove(node);
        return true;
    }
 
    public IEnumerator<T> GetEnumerator() => linkedList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Boolean Contains(T item) => dictionary.ContainsKey(item);

    public void CopyTo(T[] array, Int32 arrayIndex) => linkedList.CopyTo(array, arrayIndex);

    public Boolean Add(T item)
    {
        if (dictionary.ContainsKey(item))
            return false;
        dictionary.Add(item, linkedList.AddLast(item));
        return true;
    }
}