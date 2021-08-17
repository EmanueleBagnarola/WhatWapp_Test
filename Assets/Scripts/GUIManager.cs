using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance reference
    /// </summary>
    public static GUIManager Instance = null;

    /// <summary>
    /// The prefab of the GUICard that shows the assigned CardData information 
    /// </summary>
    [SerializeField]
    private GUICard _guiCardPrefab = null;

    /// <summary>
    /// The table transforms that contains the position of each column in landscape mode 
    /// </summary>
    [SerializeField]
    private Transform[] _landscapeCardsPositions = null;

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

    private void InitEvents()
    {
        EventsManager.Instance.OnShuffleEnded.AddListener(HandleEventShuffleEnded);
    }

    private void InitGUICards(List<CardData> cardsData)
    {
        int currentRow = 0;

        // cycle each row (7)
        while (currentRow < 7)
        {
            // get the number of cards to spawn for each row, knowing the starting total (28)
            int cardsToInstantiate = 28 - (28 - (currentRow + 1));

            for (int i = 0; i < cardsToInstantiate; i++)
            {
                GUICard guiCard = Instantiate(_guiCardPrefab, _landscapeCardsPositions[currentRow]);           
                guiCard.SetCardData(cardsData[0]);
                cardsData.RemoveAt(0);
                if(i == cardsToInstantiate - 1)
                {
                    guiCard.TurnCard(CardSide.Front);
                }
            }

            currentRow++;
        }
    }

    #region Events Handlers
    private void HandleEventShuffleEnded(List<CardData> cardsData)
    {
        InitGUICards(cardsData);
    }
    #endregion
}


