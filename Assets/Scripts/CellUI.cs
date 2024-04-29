using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
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
    public void UpdateCellColor(bool isObstable)
    {
        image.color = isObstable ? Color.black : Color.white;
    }
}
