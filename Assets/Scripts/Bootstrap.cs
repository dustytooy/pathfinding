using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private void Start()
    {
        
        GridPathfinder.Instance.Initialize();
        MyGrid.Instance.Initialize();
        OverlayUI.Instance.Initialize();
    }
}
