using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static DeckManager Instance = null;

    private List<CardSuit> unitializedSuits = new List<CardSuit>();

    private List<CardData> cardDataList = new List<CardData>();

    private void Start()
    {
        InitEvents();
    }

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

    private void InitEvents()
    {
        EventsManager.Instance.OnStartGame.AddListener(HandleEventOnStartGame);
    }

    private IEnumerator InitDeck()
    {
        InitCardSuits();

        yield return new WaitForSeconds(0.1f);

        GenerateCards();
    }

    private void InitCardSuits()
    {
        unitializedSuits.Add(CardSuit.Clubs);
        unitializedSuits.Add(CardSuit.Diamonds);
        unitializedSuits.Add(CardSuit.Hearts);
        unitializedSuits.Add(CardSuit.Spades);
    }

    /// <summary>
    /// Generate the 52 cards 
    /// </summary>
    private void GenerateCards()
    {
        if (unitializedSuits.Count <= 0)
        {
            Debug.Log("Generation Ended - " + cardDataList.Count + " cards.");
            return;
        }

        CardSuit suitToInit = unitializedSuits[unitializedSuits.Count - 1];

        for (int i = 1; i < 14; i++)
        {
            CardData cardData = new CardData(i, suitToInit);
            cardDataList.Add(cardData);
            //Debug.Log("Card: " + i + " of " + suitToInit);
        }

        unitializedSuits.Remove(suitToInit);
        GenerateCards();
    }

    #region Events Handlers
    private void HandleEventOnStartGame()
    {
        StartCoroutine(InitDeck());
    }
    #endregion
}
