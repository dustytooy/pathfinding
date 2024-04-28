using UnityEngine;
using UnityEngine.UI;
using Pathfinding.Grid;
using Pathfinding;

public class CellUIView : MonoBehaviour, ICellView
{
    [SerializeField]
    private TMPro.TMP_Text text;
    [SerializeField]
    private Image image;
    private int g, h;


    public void UpdateGCost(int value)
    {
        g = value;
        text.text = $"{g}:{h}\n{g+h}";
    }
    public void UpdateHCost(int value)
    {
        h = value;
        text.text = $"{g}:{h}\n{g+h}";
    }
    public void UpdateType(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Obstacle:
                image.color = Color.black;
                break;
            case TerrainType.Traversable:
                image.color = Color.white;
                break;
        }
    }
}
