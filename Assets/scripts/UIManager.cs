using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private UIState _currentUIState;

    [SerializeField] private GameObject _mainPanel;

    [SerializeField] private GameObject _level;

    [SerializeField] private Button _startGame;

    [SerializeField] private Button _exitGame;

    void Start()
    {
        Subscribe();
    }

    private void UpdateState()
    {
        _mainPanel.SetActive(_currentUIState == UIState.Game);
    }

    public enum UIState
    {
        Game
    }

    public void SetState(UIState state)
    {
        _currentUIState = state;
        UpdateState();
    }

    private void Subscribe()
    {
        _startGame.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio(SoundManager.Instance.BtnClip);
            _mainPanel.gameObject.SetActive(false);
            _level.gameObject.SetActive(true);
        });

        _exitGame.onClick.AddListener(() => { SoundManager.Instance.PlayAudio(SoundManager.Instance.BtnClip); });
    }

    void Update()
    {

    }
}
