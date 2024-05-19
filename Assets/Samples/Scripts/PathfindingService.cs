using System;
using System.Threading;
using Dustytoy.Pathfinding;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples
{
    internal interface IPathfindingService
    {
        public IPathfindingRequestProvider pathfindingRequestProvider { get; }
        public INode[] GetPath(INode from, INode to, CancellationToken cancellationToken = default);
        public IObservable<StreamData> GetPathStream(INode from, INode to, CancellationToken cancellationToken = default);
        public IObservable<StreamData> GetPathStream(IObservable<Unit> source, INode from, INode to, CancellationToken cancellationToken = default);
    }
    internal class PathfindingService : IPathfindingService
    {
        [Inject]
        public IPathfindingRequestProvider pathfindingRequestProvider { get; private set; }

        public INode[] GetPath(INode from, INode to, CancellationToken cancellationToken = default)
        {
            var (handle, request) = ConvertToRequest(from, to, cancellationToken);
            try
            {
                request.Process();
                return request.result;
            }
            finally
            {
                handle.Dispose();
            }
        }

        public IObservable<StreamData> GetPathStream(INode from, INode to, CancellationToken cancellationToken = default)
        {
            var (handle, request) = ConvertToRequest(from, to, cancellationToken);
            return request.ProcessStream()
            .Finally(() => handle.Dispose());
        }

        public IObservable<StreamData> GetPathStream(IObservable<Unit> source, INode from, INode to, CancellationToken cancellationToken = default)
        {
            if(source == null)
            {
                return GetPathStream(from, to, cancellationToken);
            }
            var (handle, request) = ConvertToRequest(from, to, cancellationToken);
            return request.ProcessStream(source)
            .Finally(() => handle.Dispose());
        }

        private (IDisposable, IPathfindingRequest) ConvertToRequest(INode from, INode to, CancellationToken cancellationToken = default)
        {
            return pathfindingRequestProvider.New(from, to, cancellationToken);
        }
    }
}
