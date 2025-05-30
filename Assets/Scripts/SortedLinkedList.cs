﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A sorted linked list
/// </summary>
public class SortedLinkedList<T> : LinkedList<T> where T : IComparable
{
    #region Constructor

  
    private T data;
    private LinkedListNode<T> currentNode;


    public SortedLinkedList() : base()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds the given item to the list
    /// </summary>
    /// <param name="item">item to add to list</param>
    public void Add(T item)
    {
       
        if (Contains(item))
        {
            return;
        }

        var currentNode = First;
        while (currentNode != null && currentNode.Value.CompareTo(item) < 0)
        {
            currentNode = currentNode.Next;
        }
        if (currentNode == null)
        {
            AddLast(item);
           
        }
        else
        {
            AddBefore(currentNode, item);
        }


        // add your code here
    }

    /// <summary>
    /// Repositions the given item in the list by moving it
    /// forward in the list until it's in the correct
    /// position. This is necessary to keep the list sorted
    /// when the value of the item changes
    /// </summary>
    public void Reposition(T item)
    {
        Remove(item);
        var traverseNextNode = First;

        while (traverseNextNode != null && item.CompareTo(traverseNextNode.Value) > 0)
        {
            traverseNextNode = traverseNextNode.Next;
        }

        if (traverseNextNode != null)
        {
            AddBefore(traverseNextNode, item);
        }
        else
        {
            AddLast(item);
        }
    }

    #endregion

 
}