using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DeckManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static DeckManager Instance = null;

    private List<CardData> _deckCards = new List<CardData>();

    private void Awake()
    {
        // Init Singleton ------
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        //----------------------
    }

    private void Start()
    {
        InitEvents();
    }

    public CardData DrawCard(int cardIndex)
    {
        return _deckCards[cardIndex];
    }

    public void PickCard(GUICard guiCard)
    {
        _deckCards.Remove(guiCard.CardDataReference);
    }

    public void InsertCard(GUICard guiCard, int deckIndex)
    {
        _deckCards.Insert(deckIndex, guiCard.CardDataReference);
    }

    /// <summary>
    /// Generate the 52 cards and shuffle them 
    /// </summary>
    private void GenerateCards()
    {
        List<CardData> _cardDataList = new List<CardData>();

        // Fill the available suits list. The generation removes one suit each time it generates 13 cards of that suit
        List<CardSuit> availableSuit = new List<CardSuit>();
        availableSuit.Add(CardSuit.Clubs);
        availableSuit.Add(CardSuit.Diamonds);
        availableSuit.Add(CardSuit.Hearts);
        availableSuit.Add(CardSuit.Spades);

        // Fill the available deck positions list. The generation removes one position eache time it generates one card in that position
        List<int> availableDeckPositions = new List<int>();

        for (int i = 0; i < 52; i++)
        {
            availableDeckPositions.Add(i);
        }

        while(availableSuit.Count > 0)
        {
            CardSuit suitToInit = availableSuit[availableSuit.Count - 1];

            for (int i = 1; i < 14; i++)
            {
                int deckPosition = availableDeckPositions[Random.Range(0, availableDeckPositions.Count - 1)];
                CardData cardData = new CardData(i, suitToInit, deckPosition);
                _cardDataList.Add(cardData);
                //Debug.Log("Card: " + cardData.Rank + " of " + cardData.Suit + "[" + cardData.DeckPosition + "]");

                availableDeckPositions.Remove(deckPosition);
            }

            availableSuit.Remove(suitToInit);
        }

        //Debug.Log("Generation Ended - " + _cardDataList.Count + " cards.");

        // order the list by cards data deck position
        _cardDataList = _cardDataList.OrderBy(x => x.DeckPosition).ToList();

        //TestDuplicates();

        EventsManager.Instance.OnShuffleEnded.Invoke(_cardDataList);
    }

    //private void TestDuplicates()
    //{
    //    for (int i = 0; i < _cardDataList.Count; i++)
    //    {
    //        CardData cardData = _cardDataList[i];

    //        if(i < _cardDataList.Count - 2)
    //        {
    //            CardData nextCardData = _cardDataList[i + 1];
    //            if(cardData.DeckPosition == nextCardData.DeckPosition)
    //            {
    //                Debug.LogWarning("FOUND DUPLICATE AT: " + i);
    //            }
    //        }
    //    }
    //}

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnStartGame.AddListener(HandleEventStartGame);
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardStacked.AddListener(HandleEventCardStacked);
    }

    private void HandleEventStartGame()
    {
        GenerateCards();
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        // Assign the deck cards as the remaining card data list after all the table cards are dealed
        _deckCards = cardsData;
    }

    private void HandleEventCardStacked(GUICard guiCard, bool stacked, Transform newParent)
    {
        if (guiCard == null)
            return;

        if (_deckCards.Contains(guiCard.CardDataReference))
        {
            // Pick Command
            ICommand pickCommand = new PickCommand(guiCard, _deckCards.IndexOf(guiCard.CardDataReference));
            GameManager.Instance.CommandHandler.AddCommand(pickCommand);
            pickCommand.Execute();
            EventsManager.Instance.OnUndoDeckCard.Invoke(guiCard);
        }
    }
    #endregion
}
