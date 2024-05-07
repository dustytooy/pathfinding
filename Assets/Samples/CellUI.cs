using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Dustytoy.Samples
{
    public class CellUI : MonoBehaviour
    {
        [SerializeField]
        private Button button;
        [SerializeField]
        private TMPro.TMP_Text gCost;
        [SerializeField]
        private TMPro.TMP_Text hCost;
        [SerializeField]
        private TMPro.TMP_Text fCost;
        [SerializeField]
        private Image image;

        private static readonly Dictionary<MyCell.State, Color> stateColors = new Dictionary<MyCell.State, Color>()
    {
        { MyCell.State.None, Color.white },
        { MyCell.State.OpenList, Color.yellow },
        { MyCell.State.ClosedList, Color.green },
        { MyCell.State.Start, Color.blue },
        { MyCell.State.End, Color.red },
        { MyCell.State.Path, Color.cyan },
    };
        private static readonly Dictionary<MyCell.Terrain, Color> terrainColors = new Dictionary<MyCell.Terrain, Color>()
    {
        { MyCell.Terrain.None, Color.white },
        { MyCell.Terrain.Obstacle, Color.black },
    };

        public void UpdateCost(Vector2Int cost)
        {
            if (cost.x == -1 || cost.y == -1)
            {
                gCost.text = "";
                hCost.text = "";
                fCost.text = "";
            }
            else
            {
                gCost.text = cost.x.ToString();
                hCost.text = cost.y.ToString();
                fCost.text = (cost.x + cost.y).ToString();
            }
        }

        public void UpdateColor(MyCell.State state)
        {
            image.color = stateColors[state];
        }
        public void UpdateColor(MyCell.Terrain terrain)
        {
            image.color = terrainColors[terrain];
        }
        public IDisposable OnClick(Action action)
        {
            return button.OnClickAsObservable().Subscribe(_ =>
            {
                action.Invoke();
            });
        }
    }
}