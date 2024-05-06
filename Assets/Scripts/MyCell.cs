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

    public void Initialize()
    {
        gCost = new ReactiveProperty<int>(-1).AddTo(this);
        hCost = new ReactiveProperty<int>(-1).AddTo(this);
        fCost = gCost.CombineLatest(hCost, (x, y) => x + y).ToReadOnlyReactiveProperty().AddTo(this);
        state = new ReactiveProperty<State>(State.None).AddTo(this);
        terrain = new ReactiveProperty<Terrain>(Terrain.None).AddTo(this);
    }
}
