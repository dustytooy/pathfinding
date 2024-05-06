using UnityEngine;
using Dustytoy.DI;
using Dustytoy.Pathfinding;

public class Bootstrap : MonoBehaviour
{
    public static readonly DIContainer container = new DIContainer();
    private void Start()
    {
        container.Register<PathfindingManager>(DIContainer.Lifetime.Singleton);
        container.Register(FindObjectOfType<GridPathfinder>(), DIContainer.Lifetime.Singleton);
        container.Register(FindObjectOfType<MyGrid>(), DIContainer.Lifetime.Singleton);
        container.Register(FindObjectOfType<OverlayUI>(), DIContainer.Lifetime.Singleton);

        container.Inject<GridPathfinder>();
        container.Inject<MyGrid>();
        container.Inject<OverlayUI>();

    }

    private void OnDestroy()
    {
        container.UnregisterAll();
    }
}
