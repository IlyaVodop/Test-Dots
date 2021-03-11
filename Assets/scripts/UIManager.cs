using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private GameObject _level;

    [SerializeField] private Button _startGame;

    [SerializeField] private Button _exitGame;

    [SerializeField] private Text _score;

    private enum UIState
    {
        Menu,
        Game
    }

    private UIState _currentUIState;

    private void Start()
    {
        Subscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        _startGame.onClick.AddListener(StartGame);
        _exitGame.onClick.AddListener(ExitGame);

        CoreGamePlay.ScoreUpdated += LineDrawOnScoreUpdated;
    }

    private void UnSubscribe()
    {
        _startGame.onClick.RemoveListener(StartGame);
        _exitGame.onClick.RemoveListener(ExitGame);

        CoreGamePlay.ScoreUpdated -= LineDrawOnScoreUpdated;
    }

    private void StartGame()
    {
        SoundManager.Instance.PlayClickSound();
        SetState(UIState.Game);
    }

    void ExitGame()
    {
        SoundManager.Instance.PlayClickSound();
        Application.Quit();
    }

    private void LineDrawOnScoreUpdated(int score)
    {
        _score.text = score.ToString();
    }

  
    private void SetState(UIState state)
    {
        _currentUIState = state;
        UpdateState();
    }
    private void UpdateState()
    {
        _mainPanel.SetActive(_currentUIState == UIState.Menu);
        _level.SetActive(_currentUIState == UIState.Game);
    }
}
