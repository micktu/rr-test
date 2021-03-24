using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public int MaxCardsInHand = 10;

    public int MinPlayerCards = 5;

    public int MaxPlayerCards = 10;


    public int CardArtWidth = 200;

    public int CardArtHeight = 200;


    public Vector3 CardInitialPosition = new Vector3(-10.0f, -5.0f, 0.0f);

    public Vector2 HandArcMin = new Vector2(-4.0f, 0.0f);

    public Vector2 HandArcMax = new Vector2(4.0f, 1.0f);

    public float HandArcRotation = 15.0f;

    public float HandArcOffset = 0.2f;


    public int CardBaseCost = 10;

    public int CardBaseAttack = 10;

    public int CardBaseDefense = 10;

    public int MinCardDamage = -2;

    public int MaxCardDamage = -9;

}
