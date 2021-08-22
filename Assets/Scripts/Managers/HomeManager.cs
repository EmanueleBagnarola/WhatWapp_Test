using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField]
    private GUICard[] _guiCards = null;

    [SerializeField]
    private GameObject[] _goldenSuits = null;

    [SerializeField]
    private Button _startGameButton = null;

    private void Start()
    {
        StartCoroutine(FlipCards());

        _startGameButton.onClick.AddListener(() => GameManager.Instance.OnGameSceneButton());
    }

    private IEnumerator FlipCards()
    {
        for (int i = 0; i < _guiCards.Length; i++)
        {
            GUICard guiCard = _guiCards[i];

            guiCard.FlipCard(CardSide.Front);

            yield return new WaitForSeconds(0.2f);
        }

        for (int i = 0; i < _goldenSuits.Length; i++)
        {
            GameObject goldenSuit = _goldenSuits[i];

            iTween.ScaleTo(goldenSuit, Vector3.one, 0.2f);

            yield return new WaitForSeconds(0.1f);

            iTween.PunchScale(goldenSuit, Vector3.one * 1.5f, 1.5f);

            yield return new WaitForSeconds(0.2f);
        }
    }
}
