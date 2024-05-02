using UnityEngine;
using UniRx;
using Dustytoy.Pathfinding.Grid;

public class MyCell : MonoBehaviour
{
    public enum State : int
    {
        None,
        OpenList,
        ClosedList,
        Start,
        End,
        Path
    }
    public enum Terrain
    {
        None,
        Obstacle,
    }
    public Vector2 position { get; set; }
    public static readonly float size = Cell.size;
    public ReactiveProperty<int> gCost { get; set; }
    public ReactiveProperty<int> hCost { get; set; }
    public ReadOnlyReactiveProperty<int> fCost { get; private set; }
    public ReactiveProperty<State> state { get; private set; }
    public ReactiveProperty<Terrain> terrain { get; private set; }

    private void Start()
    {
        gCost = new ReactiveProperty<int>(-1);
        hCost = new ReactiveProperty<int>(-1);
        fCost = gCost.CombineLatest(hCost, (x,y)=>x+y).ToReadOnlyReactiveProperty();
        state = new ReactiveProperty<State>(State.None);
        terrain = new ReactiveProperty<Terrain>(Terrain.None);
    }
}
