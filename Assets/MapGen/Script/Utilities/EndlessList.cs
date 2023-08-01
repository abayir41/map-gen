using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace MapGen.Utilities
{
    [System.Serializable]
    public class EndlessList<TItem>
    {
        [SerializeField] private List<TItem> _items;
        private int _index;
        public TItem CurrentItem => _items[_index];

        public void NextItem()
        {
            _index++;
            if (_index >= _items.Count)
            {
                _index = 0;
            }
        }
        
        public void PreviousItem()
        {
            _index--;
            if (_index <= -1)
            {
                _index = _items.Count - 1;
            }
        }
    }
}