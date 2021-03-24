using System;
using UnityEngine;
using UnityEngine.Assertions;


public enum CardTextType
{
    Title,
    Description
};

public enum CardValueType
{
    Cost,
    Attack,
    Defense
}

public enum CardState
{
    Idle,
    Floating,
    Dead
}


public class CardController
{
    public int Cost { get; private set; } = 10;

    public int Defense { get; private set; } = 10;

    public int Attack { get; private set; } = 10;

    public CardState State { get; private set; }

    public CardManager Manager { get; private set; }

    public CardView View { get; private set; }


    public event Action<CardController, int, CardValueType> ValueChanged;

    public event Action<CardController, CardState> StateChanged;


    public void Init(CardManager manager, CardView view)
    {
        Manager = manager;
        View = view;
    }

    public void EnterState(CardState newState)
    {
        State = newState;

        StateChanged?.Invoke(this, newState);
    }

    public void SetValue(int newValue, CardValueType type)
    {
        switch (type)
        {
            case CardValueType.Cost:
                if (newValue < 0) newValue = 0;
                Cost = newValue;
                break;

            case CardValueType.Attack:
                if (newValue < 0) newValue = 0;
                Attack = newValue;
                break;

            case CardValueType.Defense:
                if (newValue <= 0)
                {
                    Assert.AreEqual(State, CardState.Idle);
                    EnterState(CardState.Dead);
                }

                Defense = newValue;
                break;
        }

        ValueChanged?.Invoke(this, newValue, type);
    }

    public int GetValue(CardValueType type)
    {
        switch (type)
        {
            case CardValueType.Cost:
                return Cost;

            case CardValueType.Attack:
                return Attack;

            case CardValueType.Defense:
                return Defense;
        }

        return int.MinValue;
    }

}
