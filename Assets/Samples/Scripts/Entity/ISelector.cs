using System;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ISelector
    {
        public ITerrainCell selected { get; set; }
        public IObservable<ITerrainCell> onSelected { get; }
        public IObservable<ITerrainCell> onSameSelected { get; }
        public IObservable<ITerrainCell> onDifferentSelected { get; }
    }
}
