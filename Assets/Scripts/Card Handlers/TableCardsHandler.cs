using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCardsHandler : MonoBehaviour
{
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
    private Transform[] _tablePilesTransform = null;

    [SerializeField]
    private Transform _landscapeParent = null;
    [SerializeField]
    private Transform _portraitParent = null;

    private void Start()
    {
        InitEvents(); 
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
                Transform columnTransform = _tablePilesTransform[currentRow];
                guiCard.UpdateParent(columnTransform);

                // Create a temp object to use as a position reference where to move the card object
                GameObject spawnPosition = Instantiate(new GameObject("temp", typeof(RectTransform)), columnTransform);
                spawnPosition.GetComponent<RectTransform>().sizeDelta = guiCard.GetComponent<RectTransform>().sizeDelta;

                guiCard.SetCardData(cardsData[0], CardArea.Table);
                //guiCard.EnableRaycast(false);

                // Remove the spawned card from the cards data list in order to let the DeckManager handle the remaining cards
                cardsData.RemoveAt(0);
           
                // Set the last spawned card to be facing its front
                if (i == cardsToInstantiate - 1)
                {
                    guiCard.FlipCard(CardSide.Front);
                    guiCard.EnableRaycast(true);
                }

                yield return new WaitForSeconds(0.01f);
                iTween.MoveTo(guiCard.gameObject, spawnPosition.transform.position, 1.5f);
                yield return new WaitForSeconds(0.01f);
                guiCard.transform.SetParent(spawnPosition.transform);

                // Time to wait to next card to spawn
                yield return new WaitForSeconds(0.1f);
            }

            currentRow++;
        }

        EventsManager.Instance.OnCardsDealed.Invoke(cardsData);
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnShuffleEnded.AddListener(HandleEventShuffleEnded);
        EventsManager.Instance.OnDeviceOrientationUpdate.AddListener(HandleEventDeviceOrientationChange);
    }

    private void HandleEventShuffleEnded(List<CardData> cardsData)
    {
        InitGUICards(cardsData);
    }

    private void HandleEventDeviceOrientationChange(DeviceOrientation deviceOrientation)
    {
        switch (deviceOrientation)
        {
            case DeviceOrientation.Portrait:
                for (int i = 0; i < _tablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _tablePilesTransform[i];
                    tablePileTransform.SetParent(_portraitParent);
                }
                break;

            case DeviceOrientation.PortraitUpsideDown:
                for (int i = 0; i < _tablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _tablePilesTransform[i];
                    tablePileTransform.SetParent(_portraitParent);
                }
                break;

            case DeviceOrientation.LandscapeLeft:
                for (int i = 0; i < _tablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _tablePilesTransform[i];
                    tablePileTransform.SetParent(_landscapeParent);
                }
                break;

            case DeviceOrientation.LandscapeRight:
                for (int i = 0; i < _tablePilesTransform.Length; i++)
                {
                    Transform tablePileTransform = _tablePilesTransform[i];
                    tablePileTransform.SetParent(_landscapeParent);
                }
                break;
        }
    }
    #endregion
}


