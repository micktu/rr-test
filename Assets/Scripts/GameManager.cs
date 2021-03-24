using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using System;


public enum GameState
{
    None,
    Loading,
    Playing,
    Finished
}


public class GameManager : MonoBehaviour
{
    public GameUI UI;

    public GameConfig Config;


    public GameState State { get; private set; }

    public Texture2D[] CardImages { get; private set; }
    
    public CardManager CardManager { get; private set; }


    private int _numValueTypes;

    private int _currentCardIndex;


    public event Action<GameState> StateChanged;


    public GameManager()
    {
        _numValueTypes = Enum.GetValues(typeof(CardValueType)).Length;
    }

    void Start()
    {
        CardImages = new Texture2D[Config.MaxCardsInHand];

        UI.Init(this);

        EnterState(GameState.Loading);
    }

    void Update()
    {
        switch (State)
        {
            case GameState.Loading:
                break;
            case GameState.Playing:
                break;
        }
    }

    public void EnterState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Loading:
                StartCoroutine(DoLoading());
                break;
            case GameState.Playing:
                break;
            case GameState.Finished:
                break;
        }

        StateChanged?.Invoke(newState);
    }

    public IEnumerator DoLoading()
    {
        int w = Config.CardArtWidth;
        int h = Config.CardArtHeight;

        // I'm doing it sequentually to be considerate to the website and not DoS it - @micktu
        for (int i = 0; i < CardImages.Length; i++)
        {
            float salt = UnityEngine.Random.value; // Prevent Unity from caching - @micktu

            UnityWebRequest request = UnityWebRequest.Get($"https://picsum.photos/{w}/{h}?p={salt}");
            yield return request.SendWebRequest();

            Texture2D image = new Texture2D(w, h);
            image.LoadImage(request.downloadHandler.data);
            CardImages[i] = image;
        }


        // Addressables flex
        var handle = Addressables.InstantiateAsync("CardManager");
        yield return handle;

        CardManager = handle.Result.GetComponent<CardManager>();
        CardManager.Init(this);

        EnterState(GameState.Playing);
    }

    public void MutateCard()
    {
        int numCards = CardManager.Cards.Count;
        if (numCards < 1) return;

        int amount = UnityEngine.Random.Range(Config.MinCardDamage, Config.MaxCardDamage);
        CardValueType type = (CardValueType)UnityEngine.Random.Range(0, _numValueTypes);

        bool didChange = false;
        while (!didChange)
        {
            didChange = CardManager.AddCardValue(_currentCardIndex, amount, type);

            _currentCardIndex++;
            _currentCardIndex %= numCards;
        }
    }

    public void EndGame()
    {
        EnterState(GameState.Finished);
    }
}
