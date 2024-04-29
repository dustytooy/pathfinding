using UnityEngine;
using R3;

public class MyCell : MonoBehaviour
{
    public static readonly float size = Pathfinding.Grid.Cell.size;
    public ReactiveProperty<int> gCost;
    public ReactiveProperty<int> hCost;
    public ReadOnlyReactiveProperty<int> fCost;
    public ReactiveProperty<bool> isObstacle;
}
