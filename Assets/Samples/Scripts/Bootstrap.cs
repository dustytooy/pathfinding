using UnityEngine;
using Dustytoy.DI;
using Dustytoy.Pathfinding;
using Dustytoy.Collections;

namespace Dustytoy.Samples.Grid2D
{
    public class Bootstrap : MonoBehaviour
    {
        public static readonly DIContainer container = new DIContainer();
        private void Start()
        {
            // Usecases
            container.RegisterAsSingleton<IObjectPool<PathfindingRequest>, ObjectPool<PathfindingRequest>>();
            container.RegisterAsSingleton<IObjectPool<OpenList>, ObjectPool<OpenList>>();
            container.RegisterAsSingleton<IObjectPool<ClosedList>, ObjectPool<ClosedList>>();

            container.RegisterAsSingleton<IOpenListProvider, OpenListProvider>();
            container.RegisterAsSingleton<IClosedListProvider, ClosedListProvider>();
            container.RegisterAsSingleton<IPathfindingRequestProvider, PathfindingRequestProvider>();

            container.Inject<IOpenListProvider>();
            container.Inject<IClosedListProvider>();
            container.Inject<IPathfindingRequestProvider>();

            // Presenters and Views
            container.RegisterAsSingleton(FindObjectOfType<GridPathfinder>());
            container.RegisterAsSingleton(FindObjectOfType<MyGrid>());
            container.RegisterAsSingleton(FindObjectOfType<OverlayUI>());


            container.Inject<GridPathfinder>();
            container.Inject<MyGrid>();
            container.Inject<OverlayUI>();
        }

        private void OnDestroy()
        {
            container.UnregisterAll();
        }
    }
}
