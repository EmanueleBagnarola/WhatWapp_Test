using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public int Rank = 0;
    public CardSuit Suit = CardSuit.Empty;

    public CardData(int rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }
}
