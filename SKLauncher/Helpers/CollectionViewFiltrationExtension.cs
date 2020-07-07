using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace Launcher.Helpers
{
    public class Filter<T>
    {
        public Predicate<T> Logic { get; set; }
        public string Name { get; set; }
        public bool Enable { get; set; }
    }

    public class CollectionViewFiltrationExtension<T>
    {
        private readonly List<Filter<T>> _filtersCollection;
        private bool _enable;

        public CollectionViewFiltrationExtension(CollectionViewSource collectionViewSource)
        {
            CollectionViewSource = collectionViewSource;

            _filtersCollection = new List<Filter<T>>(1);
        }

        public bool Enabled
        {
            get => _enable;
            set
            {
                if (value)
                {
                    CollectionViewSource.Filter += _FilteringCallback;
                }
                else
                {
                    CollectionViewSource.Filter -= _FilteringCallback;
                }

                _enable = value;
            }
        }

        public CollectionViewSource CollectionViewSource { get; }

        public Filter<T> this[string key] => _filtersCollection.Find(f => f.Name == key);

        public void AddFilter(string name, Predicate<T> filterLogic, bool enable)
        {
            if (_Exists(name)) throw new ArgumentException("This filter already exists", nameof(name));

            var filter = new Filter<T> { Name = name, Logic = filterLogic, Enable = enable };
            _filtersCollection.Add(filter);
        }

        public void UpdateView()
        {
            if (! _enable) return;

            CollectionViewSource.Filter -= _FilteringCallback;
            CollectionViewSource.Filter += _FilteringCallback;
        }

        #region Private methods

        private void _FilteringCallback(object sender, FilterEventArgs e)
        {
            e.Accepted = _filtersCollection.TrueForAll(f => !f.Enable || f.Logic((T) e.Item));
        }

        private bool _Exists(string name)
        {
            return this[name] != null;
        }

        #endregion
    }
}