using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;


public class CardManager : MonoBehaviour
{
    public List<CardController> Cards { get; private set; } = new List<CardController>();


    private LeanGameObjectPool _pool;

    private GameManager _manager;


    private void OnEnable()
    {
        _pool = GetComponent<LeanGameObjectPool>();
    }

    public void Init(GameManager manager)
    {

        _manager = manager;

        GameConfig config = manager.Config;

        int maxCards = Random.Range(config.MinPlayerCards, config.MaxPlayerCards);

        // Spawn initial cards.
        for (int i = 0; i < maxCards; i++)
        {
            CardController card = new CardController();

            GameObject go = _pool.Spawn(config.CardInitialPosition, Quaternion.identity);
            CardView view = go.GetComponent<CardView>();

            card.Init(this, view);
            view.Init(card, _pool);

            card.SetValue(config.CardBaseCost, CardValueType.Cost);
            card.SetValue(config.CardBaseAttack, CardValueType.Attack);
            card.SetValue(config.CardBaseDefense, CardValueType.Defense);

            view.SetImage(_manager.CardImages[i]);
            view.SetOrder(i);

            CalculateTargetPositionAndRotation(i, maxCards, out Vector3 position, out Vector3 rotation);

            view.SetTargetPositionAndRotation(position, rotation, 1.0f);

            card.StateChanged += OnCardStateChanged;

            Cards.Add(card);
        }
    }

    public bool AddCardValue(int cardIndex, int amount, CardValueType type)
    {
        CardController card = Cards[cardIndex];
        if (card.State != CardState.Idle) return false;

        int currentValue = card.GetValue(type);
        card.SetValue(currentValue + amount, type);

        return true;
    }

    // This is how we determine where the card should be.
    public void CalculateTargetPositionAndRotation(int index, int maxCards, out Vector3 position, out Vector3 rotation)
    {
        GameConfig config = _manager.Config;

        Vector2 min = config.HandArcMin;
        Vector2 max = config.HandArcMax;
        float baseRotation = config.HandArcRotation;

        float missingOffset = (1.0f - (float)maxCards / config.MaxCardsInHand) / 2.0f;
        float offset = config.HandArcOffset + missingOffset / 2.0f;

        float phase = 0.5f; // In case of zero division when there is only one card left.
        if (maxCards > 1)
        {
            phase = offset + (float)index / (maxCards - 1) * (1.0f - offset * 2.0f);
        }
        
        // I am using a sine because it's easier to calculate and customize than an arc.
        position = new Vector3(min.x + phase * (max.x - min.x), min.y + Mathf.Sin(phase * Mathf.PI) * (max.y - min.y), 0.1f * index);

        float angle = -(phase - 0.5f) * 2.0f * baseRotation;
        rotation = new Vector3(0.0f, 0.0f, angle);
    }

    public void OnCardStateChanged(CardController card, CardState newState)
    {
        switch(newState)
        {
            case CardState.Dead:
                Cards.Remove(card); // We don't have many cards so O(n) is fine.

                if (Cards.Count < 1)
                {
                    _manager.EndGame();
                    break;
                }

                // Slide all cards into their new position.
                int maxCards = Cards.Count;
                for (int i = 0; i < maxCards; i++)
                {
                    CardController c = Cards[i];
                    CalculateTargetPositionAndRotation(i, maxCards, out Vector3 position, out Vector3 rotation);
                    c.View.SetTargetPositionAndRotation(position, rotation, 1.0f);
                    c.View.SetOrder(i);
                }

                break;
        }
    }
}
