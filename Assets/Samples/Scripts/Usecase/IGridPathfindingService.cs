using System;
using System.Threading;
using UnityEngine;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Usecase
{
    internal interface IGridPathfindingService
    {
        public Vector2[] GetPath(Vector2 from, Vector2 to, CancellationToken cancellationToken = default);
        public IObservable<Vector2> GetPathStream(Vector2 from, Vector2 to, CancellationToken cancellationToken = default);
        public IObservable<Vector2> GetPathStream(IObservable<Unit> source, Vector2 from, Vector2 to, CancellationToken cancellationToken = default);
    }
}
