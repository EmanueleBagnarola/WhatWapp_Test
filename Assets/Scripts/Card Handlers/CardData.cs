using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public int Rank = 0;
    public CardSuit Suit = CardSuit.Empty;
    public int DeckPosition = 0;

    public CardData(int rank, CardSuit suit, int deckPosition)
    {
        Rank = rank;
        Suit = suit;
        DeckPosition = deckPosition;
    }

    public CardColor GetCardColor()
    {
        if (Suit == CardSuit.Clubs || Suit == CardSuit.Spades)
        {
            return CardColor.Black;
        }
        if (Suit == CardSuit.Hearts || Suit == CardSuit.Diamonds)
        {
            return CardColor.Red;
        }

        return CardColor.Black;
    }
}
