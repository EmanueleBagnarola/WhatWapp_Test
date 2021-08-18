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

    [SerializeField]
    private Transform _deckTransform = null;

    /// <summary>
    /// The table transforms that contains the position of each column in landscape mode 
    /// </summary>
    [SerializeField]
    private Transform[] _landscapeCardsPositions = null;

    private Transform _currentSpawnedCard = null;
    private Transform _currentSpawnPosition = null;

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

    private void Update()
    {
        HandleSpawnCardsPlacement();

        if (Input.GetKeyDown(KeyCode.U))
        {
            GameManager.Instance.UndoCommand();
        }
    }

    private void InitGUICards(List<CardData> cardsData)
    {
        StartCoroutine(SpawnCards(cardsData));
    }

    private IEnumerator SpawnCards(List<CardData> cardsData)
    {
        int currentRow = 0;

        // cycle each row (7)
        while (currentRow < 7)
        {
            // get the number of cards to spawn for each row, knowing the starting total (28)
            int cardsToInstantiate = 28 - (28 - (currentRow + 1));

            for (int i = 0; i < cardsToInstantiate; i++)
            {
                GUICard guiCard = Instantiate(_guiCardPrefab, _deckTransform);

                // Save the column transform reference inside each GUICard script
                Transform columnTransform = _landscapeCardsPositions[currentRow];
                guiCard.UpdateParent(columnTransform);

                // Create a temp object to use as a position reference where to move the card object
                GameObject spawnPosition = Instantiate(new GameObject("temp", typeof(RectTransform)), columnTransform);
                spawnPosition.GetComponent<RectTransform>().sizeDelta = guiCard.GetComponent<RectTransform>().sizeDelta;

                guiCard.SetCardData(cardsData[0]);
                cardsData.RemoveAt(0);
           
                // Set the last spawned card to be facing its front
                if (i == cardsToInstantiate - 1)
                {
                    guiCard.FlipCard(CardSide.Front);
                }

                _currentSpawnPosition = spawnPosition.transform;
                _currentSpawnedCard = guiCard.transform;

                yield return new WaitForSeconds(0.1f);
            }

            currentRow++;
        }

        EventsManager.Instance.OnCardsDealed.Invoke();

        _currentSpawnedCard = null;
        _currentSpawnPosition = null;
    }

    /// <summary>
    /// Handle starting cards placement animation
    /// </summary>
    private void HandleSpawnCardsPlacement()
    {
        if (_currentSpawnedCard == null)
            return;

        _currentSpawnedCard.transform.position = Vector3.Lerp(_currentSpawnedCard.transform.position, _currentSpawnPosition.position, 50 * Time.deltaTime);
        _currentSpawnedCard.transform.SetParent(_currentSpawnPosition);
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnShuffleEnded.AddListener(HandleEventShuffleEnded);
    }

    private void HandleEventShuffleEnded(List<CardData> cardsData)
    {
        InitGUICards(cardsData);
    }
    #endregion
}


