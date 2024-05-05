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
    private Toggle manualToggle;
    [SerializeField]
    private Slider timeIntervalSlider;
    [SerializeField]
    private TMPro.TMP_Text displayTimeIntervalText;
    [SerializeField]
    private TMPro.TMP_Text nextText;

    private static readonly Dictionary<GridPathfinder.Phase, string> nextButtonTextDict = new Dictionary<GridPathfinder.Phase, string>()
    {
        { GridPathfinder.Phase.SelectObstacles, "X" },
        { GridPathfinder.Phase.SelectStartAndEndPositions, "Start" },
        { GridPathfinder.Phase.Pathfinding, "End" },
    };

    private GridPathfinder _pathfinder;
    private MyGrid _grid;

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
    private void OnDestroy()
    {
        _instance = null;
    }

    public void Initialize()
    {
        var disposables = new CompositeDisposable();
        _pathfinder = GridPathfinder.Instance;
        _grid = MyGrid.Instance;

        // [UI -> Game State] Reset button reset the phase to staging
        resetButton.OnClickAsObservable().Subscribe(_ =>
        {
            _pathfinder.phase.Value = GridPathfinder.Phase.SelectStartAndEndPositions;
            _grid.Clean(false);
        }).AddTo(disposables);

        // [UI -> Game State] Next button move the current phase to next phase
        nextButton.OnClickAsObservable().Subscribe(_ =>
        {
            if(_pathfinder.phase.Value == GridPathfinder.Phase.SelectStartAndEndPositions)
            {
                _pathfinder.phase.Value = GridPathfinder.Phase.Pathfinding;
            }
            if (_pathfinder.phase.Value == GridPathfinder.Phase.Pathfinding)
            {
                _pathfinder.phase.Value = GridPathfinder.Phase.SelectStartAndEndPositions;
            }
        }).AddTo(disposables);

        // [UI -> Game State] Auto step toggle change the step mode
        manualToggle.OnValueChangedAsObservable().Subscribe(x =>
        {
            _pathfinder.mode.Value = x ? GridPathfinder.Mode.ManualStep : GridPathfinder.Mode.TimeStep;
        }).AddTo(disposables);

        // [UI -> Game State] Time interval slider toggle change the time interval between each steps
        timeIntervalSlider.OnValueChangedAsObservable().Subscribe(x =>
        {
            _pathfinder.intervalInSeconds.Value = x;
            displayTimeIntervalText.text = x.ToString("0.##");
        }).AddTo(disposables);

        // [Game State -> UI] Next button move the current phase to next phase
        GridPathfinder.Instance.onPhaseChanged
            .Subscribe(x =>
            {
                switch (x)
                {
                    case GridPathfinder.Phase.SelectObstacles:
                        nextButton.interactable = false;
                        timeIntervalSlider.interactable = true;
                        manualToggle.interactable = true;
                        break;
                    case GridPathfinder.Phase.SelectStartAndEndPositions:
                        nextButton.interactable = true;
                        timeIntervalSlider.interactable = true;
                        manualToggle.interactable = true;
                        break;
                    case GridPathfinder.Phase.Pathfinding:
                        nextButton.interactable = true;
                        timeIntervalSlider.interactable = false;
                        manualToggle.interactable = false;
                        break;
                }
                nextText.text = nextButtonTextDict[x];
            }).AddTo(disposables);

        disposables.AddTo(this);
    }
}
