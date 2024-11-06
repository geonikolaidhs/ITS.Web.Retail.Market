using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.ObserverPattern
{
    public class ThreadSafeList<T> : IList<T>
    {
        protected List<T> _interalList = new List<T>();

        // Other Elements of IList implementation

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        protected static object _lock = new object();

        public List<T> Clone()
        {
            List<T> newList = new List<T>();

            lock (_lock)
            {
                _interalList.ForEach(x => newList.Add(x));
            }

            return newList;
        }

        public void Add(T obj)
        {
            lock (_lock)
            {
                _interalList.Add(obj);
            }
        }

        public bool Remove(T obj)
        {
            bool returnvalue;
            lock (_lock)
            {
                returnvalue = _interalList.Remove(obj);
            }

            return returnvalue;
        }



        public int IndexOf(T item)
        {
            lock (_lock)
            {
                return _interalList.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _interalList.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _interalList.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _interalList[index];
                }
            }
            set
            {
                lock (_lock)
                {
                    _interalList[index] = value;
                }
            }
        }


        public void Clear()
        {
            lock (_lock)
            {
                _interalList.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _interalList.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _interalList.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _interalList.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {

                return false;

            }
        }
    }
}
