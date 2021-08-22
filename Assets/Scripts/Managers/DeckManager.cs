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

    public List<CardData> DrawnCards
    {
        get
        {
            return _drawnCards;
        }
    }

    private List<CardData> _deckCards = new List<CardData>();
    private List<CardData> _drawnCards = new List<CardData>();

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

    private IEnumerator Start()
    {
        InitEvents();

        yield return new WaitForSeconds(1);

        GenerateCards();
    }

    public bool IsDeckEmpty()
    {
        return _deckCards.Count <= 0;
    }

    public CardData DrawCard()
    {
        if(_deckCards.Count <= 0)
        {
            ICommand resetCommand = new ResetCommand();
            GameManager.Instance.CommandHandler.AddCommand(resetCommand);
            resetCommand.Execute();
            return null;
        }

        CardData cardToDraw = _deckCards[0];
        _deckCards.RemoveAt(0);
        _drawnCards.Add(cardToDraw);

        ICommand drawCommand = new DrawCommand();
        GameManager.Instance.CommandHandler.AddCommand(drawCommand);
        drawCommand.Execute();

        if(_deckCards.Count <= 0)
        {   
            EventsManager.Instance.OnDeckEmpty.Invoke();
        }

        return _drawnCards[_drawnCards.Count-1];
    }

    public void UndoDrawCard()
    {
        CardData cardToUndo = _drawnCards[_drawnCards.Count - 1];
        _drawnCards.Remove(cardToUndo);
        _deckCards.Insert(0, cardToUndo);
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
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnPick.AddListener(HandleEventPick);
        EventsManager.Instance.OnUndoPick.AddListener(HandleEventUndoPick);
        EventsManager.Instance.OnUndoDraw.AddListener(HandleEventUndoDraw);
        EventsManager.Instance.OnReset.AddListener(HandleEventReset);
        EventsManager.Instance.OnUndoReset.AddListener(HandleEventUndoReset);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        // Assign the deck cards as the remaining card data list after all the table cards are dealed
        _deckCards = cardsData;
    }

    private void HandleEventCardMove(GUICard guiCard, Transform destinationParent)
    {
        // Check if the moved card was in the drawn cards pile
        if (_drawnCards.Contains(guiCard.CardDataReference))
        {
            // Call the Pick Command to save the card drawn pile index
            ICommand pickCommand = new PickCommand(guiCard, _drawnCards.IndexOf(guiCard.CardDataReference));
            GameManager.Instance.CommandHandler.AddCommand(pickCommand);
            pickCommand.Execute();
        }
    }

    private void HandleEventPick(GUICard guiCard)
    {
        _drawnCards.Remove(guiCard.CardDataReference);
    }

    private void HandleEventUndoPick(GUICard guiCard, int drawnCardIndex)
    {
        _drawnCards.Insert(drawnCardIndex, guiCard.CardDataReference);
    }

    private void HandleEventUndoDraw()
    {
        UndoDrawCard();
    }

    private void HandleEventReset()
    {
        for (int i = 0; i < _drawnCards.Count; i++)
        {
            CardData drawnCard = _drawnCards[i];
            _deckCards.Add(drawnCard);
        }

        _drawnCards.Clear();
    }

    private void HandleEventUndoReset()
    {
        for (int i = 0; i < _deckCards.Count; i++)
        {
            CardData deckCard = _deckCards[i];

            _drawnCards.Add(deckCard);
        }

        _deckCards.Clear();

        EventsManager.Instance.OnDeckEmpty.Invoke();
    }
    #endregion
}
