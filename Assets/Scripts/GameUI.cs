using UnityEngine;
using UnityEngine.UI;


public class GameUI : MonoBehaviour
{
    public Button MutateButton;

    public Text GameOverText;

    public Text LoadingText;

    private GameManager _manager;


    public void Init(GameManager manager)
    {
        _manager = manager;

        MutateButton.onClick.AddListener(manager.MutateCard);

        _manager.StateChanged += OnGameStateChanged;
    }

    public void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Loading:
                MutateButton.gameObject.SetActive(false);
                GameOverText.gameObject.SetActive(false);
                LoadingText.gameObject.SetActive(true);
                break;

            case GameState.Playing:
                MutateButton.gameObject.SetActive(true);
                GameOverText.gameObject.SetActive(false);
                LoadingText.gameObject.SetActive(false);
                break;

            case GameState.Finished:
                MutateButton.gameObject.SetActive(false);
                GameOverText.gameObject.SetActive(true);
                break;
        }
    }

}
