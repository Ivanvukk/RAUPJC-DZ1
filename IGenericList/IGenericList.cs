using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGenericList<T> : IEnumerable<T>
{
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    void Add(T item);

    /// <summary>
    /// Removes the first occurrence of an item from the collection.
    /// If the item was not found, method does nothing.
    /// </summary>
    bool Remove(T item);

    /// <summary>
    /// Removes the item at the given index in the collection.
    /// </summary>
    bool RemoveAt(int index);

    /// <summary>
    /// Returns the item at the given index in the collection.
    /// </summary>
    T GetElement(int index);

    /// <summary>
    /// Returns the index of the item in the collection.
    /// If item is not found in the collection, method returns -1.
    /// </summary>
    int IndexOf(T item);

    /// <summary>
    /// Readonly property. Gets the number of items contained in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Removes all items from the collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Determines whether the collection contains a specific value.
    /// </summary>
    bool Contains(T item);
}

public class GenericList<T> : IGenericList<T>
{
    private T[] _internalStorage;
    private T[] _backupStorage;
    private int numberOfElements;
    private int arraySize;
    private int idx;

    public GenericList()
    {
        _internalStorage = new T[4];
        arraySize = 4;
        numberOfElements = 0;
    }

    public GenericList(int initialSize) //constructor
    {
        if (initialSize < 1)
        {
            throw new ArgumentException("Number of elements must be >0");
        }
        _internalStorage = new T[initialSize];
        arraySize = initialSize;
        numberOfElements = 0;
    }

    public int Count
    {
        get
        {
            return numberOfElements;
        }
    }

    public void Add(T item)
    {
        if (numberOfElements == arraySize)                          //Check if there is enough space
        {
            _backupStorage = new T[numberOfElements];
            for (idx = 0; idx < numberOfElements; idx++)            //Backup elements 
            {
                _backupStorage[idx] = _internalStorage[idx];
            }
            _internalStorage = new T[numberOfElements * 2];         //Recreate new array
            for (idx = 0; idx < numberOfElements; idx++)
            {
                _internalStorage[idx] = _backupStorage[idx];        //Copy elements from backup
            }
            Console.WriteLine("Array expanded");
            arraySize = numberOfElements * 2;
        }

        _internalStorage[numberOfElements] = item;                  //Add new element
        numberOfElements++;
    }

    public void Clear()
    {
        for (idx = 0; idx < numberOfElements; idx++)
        {
            _internalStorage[idx] = default(T);
        }
        numberOfElements = 0;
    }

    public bool Contains(T item)
    {
        if (IndexOf(item) >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public T GetElement(int index)
    {
        if (index >= 0 && index < numberOfElements)
        {
            return _internalStorage[index];                         //Address element 
        }
        else
        {
            throw new ArgumentException("Unacceptable index");          // Throw exceptioon if index is out of range
        }
    }

    public int IndexOf(T item)
    {
        idx = 0;

        while ((!EqualityComparer<T>.Default.Equals(item, _internalStorage[idx])) && (idx < numberOfElements))
        {
            idx++;                                                  //find item
        }

        if (idx < numberOfElements)
        {
            return idx;                                             // return index
        }
        else
        {
            return -1;
        }
    }

    public bool Remove(T item)
    {
        idx = 0;

        while ((!EqualityComparer<T>.Default.Equals(item, _internalStorage[idx])) && (idx < numberOfElements))
        {
            idx++;                                                  //find item index
        }
        
        return RemoveAt(idx);                                       //Remove item with selected index
    }

    public bool RemoveAt(int index)
    {
        if ((index + 1) > numberOfElements)                           //index out of range
        {
            return false;
        }

        while (index != (numberOfElements - 1))                       //Shift elements to the right
        {
            _internalStorage[index] = _internalStorage[index + 1];
            index++;
        }
        numberOfElements--;                                         //Decremant total number of elements
        _internalStorage[numberOfElements] = default(T);                     //Set last one to zero
        return true;
    }

    //IEnumerable<T> implementation
    public IEnumerator<T> GetEnumerator()
    {
        return new GenericListEnumerator <T> (this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class GenericListEnumerator<T> : IEnumerator<T>
{
    private GenericList<T> _collection;
    private int curIndex;
    private T curT;

    public GenericListEnumerator(GenericList<T> collection)
    {
        _collection = collection;
        curIndex = -1;
        curT = default(T);
    }

    public T Current
    {
        get
        {
            return curT;
        }
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public void Dispose()
    {
       // throw new Exception("Not implemnted");
    }

    public bool MoveNext()
    {
        if (++curIndex >= _collection.Count)   //ispravi
        {
            return false;
        }
        else
        {
            curT = _collection.GetElement(curIndex);
        }
        return true;
    }

    public void Reset()
    {
        curIndex = -1;
    }
}

