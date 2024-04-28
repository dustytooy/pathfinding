using System;
using R3;

namespace Pathfinding.Grid
{
    public class CellViewModel : IDisposable
    {
        private ICell _model;
        private ICellView _view;

        private readonly CompositeDisposable _disposables;

        public CellViewModel(ICell model, ICellView view)
        {
            _disposables = new();

            _model = model;
            _view = view;
            model.gCost.Subscribe(x =>
            {
                view.UpdateGCost(x);
            }).AddTo(_disposables);
            model.hCost.Subscribe(x =>
            {
                view.UpdateHCost(x);
            }).AddTo(_disposables);
            model.type.Subscribe(x =>
            {
                view.UpdateType(x);
            }).AddTo(_disposables);
        }

        public void Dispose()
        {
            if (_disposables.IsDisposed) return;
            _disposables.Dispose();
        }
    }
}
