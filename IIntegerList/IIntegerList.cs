using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public interface IIntegerList
    {
    /// <summary>
    /// Adds an item to the collection.
    /// </summary>
    void Add(int item);

    /// <summary>
    /// Removes the first occurrence of an item from the collection.
    /// If the item was not found, method does nothing.
    /// </summary>
    bool Remove(int item);

    /// <summary>
    /// Removes the item at the given index in the collection.
    /// </summary>
    bool RemoveAt(int index);

    /// <summary>
    /// Returns the item at the given index in the collection.
    /// </summary>
    int GetElement(int index);

    /// <summary>
    /// Returns the index of the item in the collection.
    /// If item is not found in the collection, method returns -1.
    /// </summary>
    int IndexOf(int item);

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
    bool Contains(int item);
}

public class IntegerList : IIntegerList
{
    private int[] _internalStorage;
    private int[] _backupStorage;
    private int numberOfElements;
    private int arraySize;
    private int idx;

    public IntegerList()
    {
    _internalStorage = new int[4];
    arraySize = 4;
    numberOfElements = 0;
    }

    public IntegerList(int initialSize) //constructor
    {
        if (initialSize < 1)
        {
            throw new ArgumentException("Number of elements must be >0");
        }
        _internalStorage = new int[initialSize];
        arraySize = initialSize;
        numberOfElements = 0;
    }


                         // ... IIntegerList implementation ...
    public void Add(int item)                                       //-----ADD ELEMENT-----
    {
        if (numberOfElements == arraySize)                          //Check if there is enough space
        {
            _backupStorage = new int[numberOfElements];
            for (idx = 0; idx < numberOfElements; idx++)            //Backup elements 
            {
                _backupStorage[idx] = _internalStorage[idx];
            }
            _internalStorage = new int[numberOfElements*2];         //Recreate new array
            for (idx = 0; idx < numberOfElements; idx++)
            {
                _internalStorage[idx] = _backupStorage[idx];        //Copy elements from backup
            }
            Console.WriteLine("Array expanded");
            arraySize = numberOfElements*2;
        }

        _internalStorage[numberOfElements] = item;                  //Add new element
        numberOfElements++;
    }

    public bool RemoveAt(int index)                                 //-----DELETE ELEMENT-----
    {
        if ((index+1) > numberOfElements)                           //index out of range
        {
            return false;
        }

        while (index != (numberOfElements-1))                       //Shift elements to the right
        {
            _internalStorage[index] = _internalStorage[index + 1];
            index++;
        }
        numberOfElements--;                                         //Decremant total number of elements
        _internalStorage[numberOfElements] = 0;                     //Set last one to zero
        return true;
    }

    public bool Remove(int item)                                    //-----REMOVE ITEM-----
    {
       idx = 0;

       while((item != _internalStorage[idx]) && (idx < numberOfElements))
            {
            idx++;                                                  //find item index
            }
        
        return RemoveAt(idx);                                       //Remove item with selected index
    }
    
    public int GetElement(int index)                                //-----GET ELEMENT-----
    {
        if (index >= 0 && index < numberOfElements)
        {
            return _internalStorage[index];                         //Address element 
        }
        else
        {
            throw new ArgumentException("Negative index");          // Throw exceptioon if index is out of range
        }
    }

    public int IndexOf(int item)                                    //-----FIND INDEX OF ITEM-----
    {
        idx = 0;

        while ((item != _internalStorage[idx]) && (idx < numberOfElements))
        {
            idx++;                                                  //find item
        }

        if (idx < numberOfElements)
        {
            return idx;                                            
        }
        else
        {
            return -1;
        }
    }

    public int Count                                                //-----NUMBER OF ELEMENTS-----
    {
        get
        {
            return numberOfElements;
        }
    }

    public void Clear()                                             //-----CLEAR LIST-----
    {
        for (idx = 0; idx < numberOfElements; idx++)
        {
            _internalStorage[idx] = 0;
        }
        numberOfElements = 0;
    }

    public bool Contains(int item)                                  //-----CHECK IF LIST CONTAINS ELEMENT-----
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
}



