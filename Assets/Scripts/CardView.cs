using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using TMPro;
using DG.Tweening;

public class CardView : MonoBehaviour, IPoolable
{
    public Material CardArtMaterial;

    public TextMeshProUGUI TitleText;

    public TextMeshProUGUI DescriptionText;

    public TextMeshProUGUI CostText;

    public TextMeshProUGUI AttackText;

    public TextMeshProUGUI DefenseText;

    public Image ArtImage;


    public CardManager Manager { get; private set; }

    public CardController Controller { get; private set; }


    private Canvas _canvas;

    private LeanGameObjectPool _pool;


    public void Init(CardController controller, LeanGameObjectPool pool)
    {
        Controller = controller;
        _pool = pool;

        Controller.ValueChanged += OnValueChanged;
        Controller.StateChanged += OnCardStateChanged;

        // A material instance is required so we can apply different textures to different cards.
        ArtImage.material = new Material(CardArtMaterial);

        _canvas = GetComponentInChildren<Canvas>();
    }

    public void OnSpawn()
    {

    }

    public void OnDespawn()
    {
        Controller = null; // Release the reference for GC.
    }

    public void OnValueChanged(CardController card, int newValue, CardValueType type)
    {
        TextMeshProUGUI textToChange = null;

        switch (type)
        {
            case CardValueType.Cost:
                textToChange = CostText;
                break;

            case CardValueType.Attack:
                textToChange = AttackText;
                break;

            case CardValueType.Defense:
                textToChange = DefenseText;
                break;
        }

        textToChange.text = newValue.ToString();

        _canvas.sortingOrder += 3; // Bring the card to the top.

        // Animate the number.
        DOTween.Punch(() => { return textToChange.transform.localScale; }, (Vector3 newScale) => { textToChange.transform.localScale = newScale; }, new Vector3(1.0f, 1.0f), 0.5f, 5);
    }

    public void SetText(string newText, CardTextType type)
    {
        switch (type)
        {
            case CardTextType.Title:
                TitleText.text = newText;
                break;

            case CardTextType.Description:
                DescriptionText.text = newText;
                break;
        }
    }

    public void SetImage(Texture2D newImage)
    {
        ArtImage.material.mainTexture = newImage;
    }

    public void SetOrder(int newIndex)
    {
        _canvas.sortingOrder = newIndex;
    }

    public void SetTargetPositionAndRotation(Vector3 position, Vector3 rotation, float duration = 0.0f)
    {
        // Always tween unless it's immediate.
        if (duration == 0.0f)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
            return;
        }

        transform.DOMove(position, duration);
        transform.DORotate(rotation, duration);
    }

    public void OnCardStateChanged(CardController card, CardState newState)
    {
        switch (newState)
        {
            case CardState.Dead:
                // Animate and destroy the view.
                transform.DOMoveY(1.0f, 1.0f).onComplete += () => { _pool.Despawn(gameObject); };
                break;
        }
    }
}
