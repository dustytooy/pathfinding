using System;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface ICellClicker
    {
        public ITerrainCell clickedCell { get; set; }
        public IObservable<ITerrainCell> onCellClicked { get; }
        public IObservable<ITerrainCell> onSameCellClicked { get; }
        public IObservable<ITerrainCell> onDifferentCellClicked { get; }
    }
    internal class CellClicker : ICellClicker
    {
        public ITerrainCell clickedCell 
        { 
            get => _clickedCell.Value; 
            set
            {
                _clickedCell.Value = value;
            }
        }
        public IObservable<ITerrainCell> onCellClicked { get; private set; }
        public IObservable<ITerrainCell> onSameCellClicked { get; private set; }
        public IObservable<ITerrainCell> onDifferentCellClicked { get; private set; }

        private ReactiveProperty<ITerrainCell> _clickedCell;

        public CellClicker()
        {
            _clickedCell = new ReactiveProperty<ITerrainCell>(null);

            onCellClicked = _clickedCell.Skip(1);
            onSameCellClicked = _clickedCell.Where(_ => _ == clickedCell);
            onDifferentCellClicked = _clickedCell.DistinctUntilChanged();
        }
        ~CellClicker()
        {
            _clickedCell.Dispose();
        }
    }
}
