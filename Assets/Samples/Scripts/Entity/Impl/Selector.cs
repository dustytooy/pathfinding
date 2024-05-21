using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity.Impl
{
    internal class Selector : ISelector
    {
        public ITerrainCell selected 
        { 
            get => _selected.Value; 
            set
            {
                _selected.Value = value;
            }
        }
        public IObservable<ITerrainCell> onSelected { get; private set; }
        public IObservable<ITerrainCell> onSameSelected { get; private set; }
        public IObservable<ITerrainCell> onDifferentSelected { get; private set; }

        private ReactiveProperty<ITerrainCell> _selected;

        public Selector()
        {
            _selected = new ReactiveProperty<ITerrainCell>(null);

            onSelected = _selected.Skip(1);
            onSameSelected = _selected.Where(_ => _ == selected);
            onDifferentSelected = _selected.DistinctUntilChanged();
        }
        ~Selector()
        {
            _selected.Dispose();
        }
    }
}
