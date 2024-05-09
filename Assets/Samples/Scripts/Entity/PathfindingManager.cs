using System;
using System.Threading;
using Dustytoy.Pathfinding;
using Dustytoy.Pathfinding.Grid;
using Dustytoy.DI;
using UniRx;

namespace Dustytoy.Samples.Grid2D.Entity
{
    internal interface IPathfindingManager
    {
        public IPathfindingRequestProvider pathfindingRequestProvider { get; }
        public ITerrainGrid terrainGrid { get; }
        public ITerrainCell[] GetPath(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default);
        public IObservable<StreamData> GetPathStream(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default);
        public IObservable<StreamData> GetPathStream(IObservable<Unit> source, ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default);
    }
    internal class PathfindingManager : IPathfindingManager
    {
        [Inject]
        public IPathfindingRequestProvider pathfindingRequestProvider { get; private set; }
        [Inject]
        public ITerrainGrid terrainGrid { get; private set; }


        public ITerrainCell[] GetPath(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            var cells = terrainGrid.cells;
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            try
            {
                request.Process();
                return Array.ConvertAll(request.result, x => x as ITerrainCell);
            }
            finally
            {
                handle.Dispose();
            }
        }

        public IObservable<StreamData> GetPathStream(ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            var cells = terrainGrid.cells;
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            return request.ProcessStream()
            .Finally(() => handle.Dispose());
        }

        public IObservable<StreamData> GetPathStream(IObservable<Unit> source, ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            var cells = terrainGrid.cells;
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            return request.ProcessStream(source)
            .Finally(() => handle.Dispose());
        }

        private (IDisposable, IPathfindingRequest) ConvertToRequest(ITerrainCell[] terrains, ITerrainCell from, ITerrainCell to, CancellationToken cancellationToken = default)
        {
            int width = terrainGrid.width;
            int height = terrainGrid.height;
            // TODO: DI instead of concrete class
            var pathfindingGrid = new Grid(width, height);
            Converter<ITerrainCell, ICell> converter = (x) =>
            {
                // TODO: DI instead of concrete class
                return new PathfindingCell(
                    x.xCoordinate, x.yCoordinate,
                    x.terrainProperties.type == TerrainProperties.Type.Obstacle,
                    pathfindingGrid,
                    to.xCoordinate, to.yCoordinate);
            };
            var cells = Array.ConvertAll(terrains, converter);
            pathfindingGrid.cells = cells;

            INode start = converter.Invoke(from);
            INode end = converter.Invoke(to);
            return pathfindingRequestProvider.New(start, end, cancellationToken);
        }
    }
}
