using System;
using UnityEngine;
using System.Collections;

namespace Shape
{
    public class Shape<T> where T : Polygon
    {
        private class Node
        {
            public Node nextNode, previousNode;
            public T data;
        }

        private int _size;
        private Node _sentry;
        private Node _currentNode;


        #region Private Methods
        private static Node CopyNode() // TODO
        {
            return null;
        }

        private static void DeleteNode(Node m, Node s)
        {
            if (m != s)
            {
                DeleteNode(m.nextNode, s);
            }
            m = null;
        }

        private Node SearchNode(Node next, T data)
        {
            if (next == null)
                return null;
            else
            {
                if (next.data.Equals(data))
                    return next;
                return SearchNode(next.nextNode, data);
            }
        }
        #endregion

        #region Constructors
        public Shape()
        {
            _size = 0;
            _sentry = new Node();
            _sentry.nextNode = _sentry;
            _sentry.previousNode = _sentry;
            _currentNode = _sentry;
        }

        public Shape(Shape<T> original) // TODO
        {
            _size = original._size;
            Node aux = CopyNode();
        }
        #endregion

        #region Operators
        // Assign =
        #endregion

        #region Modifiers
        public void Add(T data)
        {
            Node aux = new Node();
            aux.data = data;
            aux.nextNode = _currentNode;
            aux.previousNode = _currentNode.previousNode;
            _currentNode.previousNode.nextNode = aux;
            _size++;
        }

        public void Remove(T data)
        {
            _currentNode = SearchNode(_sentry.nextNode, data);
            Node aux = _currentNode;
            _currentNode.previousNode.nextNode = _currentNode.nextNode;
            _currentNode.nextNode.previousNode = _currentNode.previousNode;
            _currentNode = _currentNode.nextNode;
            aux = null;
            _size--;
        }

        public void Clear()
        {
            DeleteNode(_sentry.nextNode, _sentry);
            _size = 0;
            _sentry = new Node();
            _sentry.nextNode = _sentry;
            _sentry.previousNode = _sentry;
            _currentNode = _sentry;
        }

        public void Merge() // TODO. Concat
        {

        }
        #endregion

        #region Consultants
        public bool IsEmpty()
        {
            return _size == 0;
        }

        public int Count
        {
            get { return _size; }
        }
        #endregion

        #region To Iterate
        public T GetCurrent()
        {
            return _currentNode.data;
        }

        public void SetCurrent(T data)
        {
            _currentNode.data = data;
        }

        public void Start()
        {
            _currentNode = _sentry.nextNode;
        }

        public void End()
        {
            _currentNode = _sentry;
        }

        public void Next()
        {
            _currentNode = _currentNode.nextNode;
        }

        public void Previous()
        {
            _currentNode = _currentNode.previousNode;
        }

        public bool IsAllRight()
        {
            return _currentNode == _sentry;
        }

        public bool IsFirst()
        {
            return _currentNode == _sentry.nextNode;
        }
        #endregion

        #region Interface Methods
        private void SetCurrentNodeAt(int index)
        {
            if (index > _size - 1)
                throw new ArgumentOutOfRangeException("index");

            int i = 0;
            _currentNode = _sentry.nextNode;

            while (i != index)
            {
                _currentNode = _currentNode.nextNode;
                i++;
            }
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _size; i++)
            {
                SetCurrentNodeAt(i);
                yield return _currentNode.data;
            }
        }
        #endregion
    }
}