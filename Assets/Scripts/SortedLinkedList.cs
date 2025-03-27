using System;
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

    LinkedList<T> listOfNodes = new LinkedList<T>();
    private T data;
    private LinkedListNode<T> headNode = null;
 
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
        if (headNode == null)
        {
            headNode = new LinkedListNode<T>(item);
            headNode.Value = item;
            AddFirst(headNode);
            
           
        }

        if (currentNode != null && currentNode.Value.CompareTo(item) < 0) //  current itemden önce geliyosa
        {
             LinkedListNode<T> previousNode= currentNode;
             currentNode = currentNode.Next;
             currentNode.Value = item;
             
        }
        else if (currentNode == null)

        {
            AddLast(item);
        }
        else
        {
            Reposition(item);
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
        LinkedListNode<T> newItemNode = new LinkedListNode<T>(item);
        LinkedListNode<T> traversenextNode;
        newItemNode =listOfNodes.First;
       
        if (!listOfNodes.Contains(item))
            
        {
                traversenextNode =newItemNode.Next;
                while (newItemNode.Value.CompareTo(traversenextNode) <= 0)//newıtem traverse den önce ise
                {
                    newItemNode =traversenextNode;
                    traversenextNode = traversenextNode.Next;
                }

                if (traversenextNode == null)
                {
                    AddLast(newItemNode);
                }
                else
                {
                    AddBefore(newItemNode,item);
                }
              
        }

        // add your code here
    }

    #endregion
}