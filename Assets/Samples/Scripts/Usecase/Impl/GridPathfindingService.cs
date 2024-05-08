using System;
using System.Threading;
using Dustytoy.Pathfinding;
using Dustytoy.Pathfinding.Grid;
using Dustytoy.DI;
using UniRx;
using UnityEngine;

using Grid = Dustytoy.Pathfinding.Grid.Grid;

namespace Dustytoy.Samples.Grid2D.Usecase.Impl
{
    internal class GridPathfindingService : IGridPathfindingService
    {
        [Inject]
        private IPathfindingRequestProvider _requestProvider;
        [Inject]
        private IGridProvider _gridProvider;
        [Inject]
        private IConfigurationService _configurationService;

        public Vector2[] GetPath(Vector2 from, Vector2 to, CancellationToken cancellationToken = default)
        {
            var cells = _gridProvider.GetCells();
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            try
            {
                request.Process();
                var response = ConvertToVector2Array(request.result);
                return response;
            }
            catch(OperationCanceledException)
            {
                throw PathfindingErrors.OperationCanceledException;
            }
            finally
            {
                handle.Dispose();
            }
        }

        public IObservable<Vector2> GetPathStream(Vector2 from, Vector2 to, CancellationToken cancellationToken = default)
        {
            var cells = _gridProvider.GetCells();
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            try
            {
                return request.ProcessStream()
                .Finally(() =>
                {
                    handle.Dispose();
                })
                .Select(data =>
                {
                    var cell = data.node as ICell;
                    var action = data.action;
                    int width = _configurationService.GetGridDimensions().Item1;
                    float cellSize = _configurationService.GetCellSize();
                    int i = cell.yCoordinate * width + cell.xCoordinate;
                    return cell.CellToPosition(cellSize);
                });
            }
            catch (OperationCanceledException)
            {
                throw PathfindingErrors.OperationCanceledException;
            }
        }

        public IObservable<Vector2> GetPathStream(IObservable<Unit> source, Vector2 from, Vector2 to, CancellationToken cancellationToken = default)
        {
            var cells = _gridProvider.GetCells();
            var (handle, request) = ConvertToRequest(cells, from, to, cancellationToken);
            try
            {
                return request.ProcessStream(source)
                .Finally(() =>
                {
                    handle.Dispose();
                })
                .Select(data =>
                {
                    var cell = data.node as ICell;
                    var action = data.action;
                    int width = _configurationService.GetGridDimensions().Item1;
                    float cellSize = _configurationService.GetCellSize();
                    int i = cell.yCoordinate * width + cell.xCoordinate;
                    return cell.CellToPosition(cellSize);
                });
            }
            catch (OperationCanceledException)
            {
                throw PathfindingErrors.OperationCanceledException;
            }
        }

        private (IDisposable, IPathfindingRequest) ConvertToRequest(MyCell[] cells, Vector2 from, Vector2 to, CancellationToken cancellationToken = default)
        {
            var (width, height) = _configurationService.GetGridDimensions();
            float cellSize = _configurationService.GetCellSize();

            var pathfindingGrid = new Grid(width, height);
            var endPosition = CellUtilities.PositionToInt(to, cellSize);

            pathfindingGrid.cells = Array.ConvertAll(cells, x =>
            {
                var position = CellUtilities.PositionToInt(x.position, cellSize);
                return new Cell(
                    position.x, position.y,
                    x.terrain.Value == MyCell.Terrain.Obstacle,
                    pathfindingGrid,
                    endPosition.x, endPosition.y);
            });
            if (!pathfindingGrid.IsValidPosition(endPosition.x, endPosition.y))
            {
                throw PathfindingErrors.InvalidPositionException;
            }

            INode start = pathfindingGrid.PositionToCell(from, cellSize);
            INode end = pathfindingGrid.PositionToCell(to, cellSize);
            return _requestProvider.New(start, end, cancellationToken);
        }

        private Vector2[] ConvertToVector2Array(INode[] nodes)
        {
            return Array.ConvertAll(nodes, ConvertToVector2);
        }

        private Vector2 ConvertToVector2(INode node)
        {
            var position = node as ICell;
            float cellSize = _configurationService.GetCellSize();
            var v = position.CellToPosition(cellSize);
            return new Vector2(v.x, v.y);
        }
    }
}
