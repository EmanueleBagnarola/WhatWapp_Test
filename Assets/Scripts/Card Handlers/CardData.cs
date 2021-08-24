using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    /// <summary>
    /// The numeric rank of the card (1 to 13)
    /// </summary>
    public int Rank = 0;

    /// <summary>
    /// The symbol of the card (Heart, Diamond, Clubs, Spades)
    /// </summary>
    public CardSuit Suit = CardSuit.Empty;

    /// <summary>
    /// Save the deck position used in the deck generation
    /// </summary>
    public int DeckPosition = 0;

    public CardData(int rank, CardSuit suit, int deckPosition)
    {
        Rank = rank;
        Suit = suit;
        DeckPosition = deckPosition;
    }

    /// <summary>
    /// Get the color of the suit
    /// </summary>
    /// <returns></returns>
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
