using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIDeck : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Transform[] _drawPilePositions = null;

    [SerializeField]
    private List<GUICard> _guiCardsPile = new List<GUICard>();
    [SerializeField]
    private Transform _drawPileCardsParent = null;

    private List<CardData> _hiddenCards = new List<CardData>();

    private int _pileCounter = 0;

    private void Start()
    {
        InitEvents();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CardData drawCardData = DeckManager.Instance.DrawCard();

        GUICard guiDrawCard = null;

        if(drawCardData == null)
        {
            // RESET DRAW
            _pileCounter = 0;
        }

        if(_pileCounter >= 3)
        {
            guiDrawCard = _guiCardsPile[_pileCounter];
            guiDrawCard.gameObject.SetActive(true);
            guiDrawCard.SetCardData(drawCardData, CardArea.DrawPile);
            guiDrawCard.FlipCard(CardSide.Front);
            guiDrawCard.SetSortingOrder(4);
            iTween.MoveTo(guiDrawCard.gameObject, _drawPilePositions[2].position, 0.4f);

            _guiCardsPile[1].SetSortingOrder(1);
            iTween.MoveTo(_guiCardsPile[1].gameObject, _drawPilePositions[0].position, 0.4f);
            _guiCardsPile[2].SetSortingOrder(2);
            iTween.MoveTo(_guiCardsPile[2].gameObject, _drawPilePositions[1].position, 0.4f);

            GUICard cardToHide = _guiCardsPile[0];
            cardToHide.gameObject.transform.position = _drawPileCardsParent.position;
            cardToHide.gameObject.SetActive(false);
            _guiCardsPile.Remove(cardToHide);
            _guiCardsPile.Add(cardToHide);
            _hiddenCards.Add(cardToHide.CardDataReference);
        }
        else
        {
            guiDrawCard = _guiCardsPile[_pileCounter];
            guiDrawCard.gameObject.SetActive(true);
            guiDrawCard.SetCardData(drawCardData, CardArea.DrawPile);
            guiDrawCard.FlipCard(CardSide.Front);
            guiDrawCard.SetSortingOrder(_pileCounter + 1);
            iTween.MoveTo(guiDrawCard.gameObject, _drawPilePositions[_pileCounter].position, 0.4f);

            if(_pileCounter < 3)
                _pileCounter++;
        }

        //ICommand DrawCommand = new DrawCommand(guiDrawCard, )
    }

    private void ShiftForward()
    {

    }

    private void ShiftBack()
    {

    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardStacked.AddListener(HandleEventCardStacked);
        EventsManager.Instance.OnUndoPick.AddListener(HandleEventUndoPick);
        EventsManager.Instance.OnUndoDraw.AddListener(HandleEventUndoDraw);
    }

    private void HandleEventCardStacked(GUICard guiCard, bool stacked, Transform newParent)
    {
        // Instantiate a new GUI card if it was in the pile
        if (guiCard == null)
            return;

        if (_guiCardsPile.Contains(guiCard))
        {
            _pileCounter = _guiCardsPile.IndexOf(guiCard);
            _guiCardsPile.Remove(guiCard);

            GUICard newPileCard = Instantiate(GUIManager.Instance.GUICardPrefab, _drawPileCardsParent);
            newPileCard.gameObject.SetActive(false);
            _guiCardsPile.Add(newPileCard);
        }
    }

    private IEnumerator PutCardOnDeck(bool decreasePileCounter)
    {
        GUICard shiftInDeckCard = _guiCardsPile[_pileCounter - 1];
        shiftInDeckCard.FlipCard(CardSide.Back);

        iTween.MoveTo(shiftInDeckCard.gameObject, _drawPileCardsParent.position, 0.4f);

        if (decreasePileCounter)
        {
            _pileCounter--;
        }

        yield return new WaitForSeconds(0.4f);

        shiftInDeckCard.gameObject.SetActive(false);
    }

    private void HandleEventUndoPick(GUICard guiCard)
    {
        iTween.MoveTo(guiCard.gameObject, _drawPilePositions[_pileCounter].position, 0.4f);

        //for (int i = 0; i < gi; i++)
        //{

        //}
    }

    private void HandleEventUndoDraw()
    {
        // Check if there are three cards showing in the draw pile
        if (_pileCounter >= 3)
        {
            // Check if there is any hidden card reference (the bottom of the pile)
            if (_hiddenCards.Count > 0)
            {
                GUICard hiddenCard = null;

                // Use one disabled UICard to the hidden card
                for (int i = 0; i < _guiCardsPile.Count; i++)
                {
                    GUICard card = _guiCardsPile[i];
                    if (!_guiCardsPile[i].gameObject.activeInHierarchy)
                        hiddenCard = card;
                }

                //hiddenCard.gameObject.transform.position = _drawPilePositions[0].position;
                iTween.MoveTo(hiddenCard.gameObject, _drawPilePositions[0].position, 0.1f);
                hiddenCard.gameObject.SetActive(true);
                hiddenCard.FlipCard(CardSide.Front);
                hiddenCard.SetCardData(_hiddenCards[_hiddenCards.Count - 1], CardArea.DrawPile);
                _hiddenCards.Remove(hiddenCard.CardDataReference);
                hiddenCard.SetSortingOrder(1);

                _guiCardsPile[0].SetSortingOrder(2);
                iTween.MoveTo(_guiCardsPile[0].gameObject, _drawPilePositions[1].position, 0.4f);
                _guiCardsPile[1].SetSortingOrder(3);
                iTween.MoveTo(_guiCardsPile[1].gameObject, _drawPilePositions[2].position, 0.4f);
                _guiCardsPile[1].SetSortingOrder(3);

                StartCoroutine(PutCardOnDeck(false));

                /// Shift the last card reference in the list to be the first
                GUICard listCardShift = _guiCardsPile[_guiCardsPile.Count - 1];
                _guiCardsPile.Remove(listCardShift);
                _guiCardsPile.Insert(0, listCardShift);
            }
            else
            {
                StartCoroutine(PutCardOnDeck(true));
            }
        }
        else
        {
            StartCoroutine(PutCardOnDeck(true));
        }

    }
    #endregion
}
