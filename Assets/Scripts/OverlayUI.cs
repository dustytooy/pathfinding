using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class OverlayUI : MonoBehaviour
{
    public static OverlayUI Instance { get { return _instance; } }
    private static OverlayUI _instance;

    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private TMPro.TMP_Text nextText;

    private static readonly Dictionary<GridPathfinder.Phase, string> colors = new Dictionary<GridPathfinder.Phase, string>()
    {
        { GridPathfinder.Phase.Staging, "Select" },
        { GridPathfinder.Phase.Select, "Pathfind" },
        { GridPathfinder.Phase.Pathfinding, "Select" },
    };

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void Initialize()
    {
        resetButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(GridPathfinder.Instance.phase.Value != GridPathfinder.Phase.Staging)
            {
                GridPathfinder.Instance.phase.Value = GridPathfinder.Phase.Staging;
            }
        });
        nextButton.OnClickAsObservable().Subscribe(_ =>
        {
            GridPathfinder.Instance.NextPhase();
        });

        GridPathfinder.Instance.onPhaseChanged
            .Subscribe(_ =>
            {
                nextText.text = colors[_];
            });
    }
}
