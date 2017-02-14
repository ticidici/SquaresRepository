using System;
using UnityEngine;
using System.Collections;

public class Shape<T> where T : Polygon
{
    private class Node
    {
        public Node nextNode, previousNode;
        public T data;
    }

    private Node _firstNode;
    private Node _lastNode;
    private Node _currentNode;

    private int _size;

    #region Private Methods
    private void RemoveNode(Node next)
    {
        if(next != null)
        {
            RemoveNode(next.nextNode);
            next = null;
        }
    }

    private Node SearchNode(Node next, T data)
    {
        if (next == null)
            return null;
        else
        {
            if (next.data.Equals(data))
                return next;
            return SearchNode(next.nextNode,data);
        }
    }

    private void SetCurrentNodeAt(int index)
    {
        if (index > _size - 1)
            throw new ArgumentOutOfRangeException("index");

        int i = 0;
        _currentNode = _firstNode;

        while (i != index)
        {
            _currentNode = _currentNode.nextNode;
            i++;
        }
    }

    private Node MinimumDistanceTo(Node target) // Puto bodrio. Rev
    {
        /*Node closestNode = _firstNode;
        float minDistance = Vector3.SqrMagnitude(_firstNode.data.transform.position - target.data.transform.position);
        closestNode = MinimumDistanceToAux(_firstNode,target, ref minDistance);
        if (closestNode == null)
            Debug.Log("El mes proper es null.");
        return closestNode;*/
        
        Node closest = _firstNode;
        float minDistance = Vector3.SqrMagnitude(closest.data.transform.position - target.data.transform.position);

        for (int i = 1; i < _size; i++)
        {
            SetCurrentNodeAt(i);
            float distance = Vector3.SqrMagnitude(_currentNode.data.transform.position - target.data.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = _currentNode;
            }
        }
        return closest;
    }
    #endregion

    #region Constructors
    public Shape()
    {
        _size = 0;
        _firstNode = null;
        _lastNode = null;
        _currentNode = null;
    }
    #endregion

    #region Public Methods
    public void Add(T newData) // Attach
    {
        Node aux = new Node();
        aux.data = newData;
        aux.nextNode = _currentNode;

        // Fer el attach a un membre exitent
        if(_size != 0)
        {
            Node closestNode = MinimumDistanceTo(aux);
            aux.data.AttachTo(closestNode.data);
        }
        
        // Afergirlo a l'estructura
        if (_size == 0)
        {
            aux.previousNode = null;
            _firstNode = aux;
            _lastNode = aux;
        }
        else if(_currentNode == null)
        {
            aux.previousNode = _lastNode;
            _lastNode.nextNode = aux;
            _lastNode = aux;
        } else if(_currentNode == _firstNode)
        {
            aux.previousNode = null;
            _currentNode.previousNode = aux;
            _firstNode = aux;
        }
        else
        {
            aux.previousNode = _currentNode.previousNode;
            _currentNode.previousNode.nextNode = aux;
            _currentNode.previousNode = aux;
        }

        _size++;
    }

    public void Remove(T data)
    {
        _currentNode = SearchNode(_firstNode,data);        

        /*if (_currentNode == null)
            Debug.Log("NULL");
        else Debug.Log("NOT NULL");*/

        //_currentNode.data.Detach();

        if (_size == 1)
        {
            _firstNode = null;
            _lastNode = null;
        }
        else if (_currentNode == _firstNode)
        {
            _firstNode = _currentNode.nextNode;
            _firstNode.previousNode = null;
        }
        else if (_currentNode == _lastNode)
        {
            _lastNode = _currentNode.previousNode;
            _lastNode.nextNode = null;
        }
        else
        {
            _currentNode.previousNode.nextNode = _currentNode.nextNode;
            _currentNode.nextNode.previousNode = _currentNode.previousNode;
        }

        _currentNode = _currentNode.nextNode;
        _size--;
    }

    public void Clear()
    {
        RemoveNode(_firstNode);
        _size = 0;
        _firstNode = null;
        _lastNode = null;
        _currentNode = null;
    }

    public int Count // Millorar
    {
        get { return _size; }        
    }  

    public bool Contains(T data)
    {
        _currentNode = SearchNode(_firstNode, data);

        if (_currentNode == null)
            return false;
        else return true;
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }
    
    public T FindById(int id) // Possible eliminacio
    {
        return SearchNodeId(_firstNode, id);
    }

    private T SearchNodeId(Node next, int id)
    {
        if (next == null)
            return null;
        else
        {
            if(next.data.Id.Equals(id))
            {
                return next.data;
            }
            return SearchNodeId(next.nextNode,id);
        }
    }
    #endregion

    #region Interface Methods
    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < _size; i++)
        {
            SetCurrentNodeAt(i);
            yield return _currentNode.data;
        }
    }
    #endregion

    #region Indexer
    public T this[int i]
    {
        get
        {
            try
            {
                SetCurrentNodeAt(i);                
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogException(e);
                return default(T);
            }
            return _currentNode.data;
        }
        set
        {
            SetCurrentNodeAt(i);
            _currentNode.data = value;
        }
    }
    #endregion
}