using UnityEngine;
using Dustytoy.DI;
using Dustytoy.Pathfinding;
using Dustytoy.Collections;

public class Bootstrap : MonoBehaviour
{
    public static readonly DIContainer container = new DIContainer();
    private void Start()
    {
        // Usecases
        container.RegisterAsSingleton<IObjectPool<IPathfindingRequestPoolable>, ObjectPool<PathfindingRequestPoolable>>();
        container.RegisterAsSingleton<IObjectPool<IOpenListPoolable>, ObjectPool<OpenListPoolable>>();
        container.RegisterAsSingleton<IObjectPool<IClosedListPoolable>, ObjectPool<ClosedListPoolable>>();
        container.RegisterAsSingleton<IPathfindingService, PathfindingService>();

        // Presenters and Views
        container.RegisterAsSingleton(FindObjectOfType<GridPathfinder>());
        container.RegisterAsSingleton(FindObjectOfType<MyGrid>());
        container.RegisterAsSingleton(FindObjectOfType<OverlayUI>());

        container.Inject<IPathfindingService>();
        container.Inject<GridPathfinder>();
        container.Inject<MyGrid>();
        container.Inject<OverlayUI>();

    }

    private void OnDestroy()
    {
        container.UnregisterAll();
    }
}
