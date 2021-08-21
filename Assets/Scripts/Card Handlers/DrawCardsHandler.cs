using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DrawCardsHandler : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private Transform[] _drawPilePositions = null;
    [SerializeField]
    private List<GUICard> _guiCardsPile = new List<GUICard>();
    [SerializeField]
    private Transform _drawPileCardsParent = null;

    [SerializeField]
    private Image _deckImage = null;

    private List<CardData> _hiddenCards = new List<CardData>();

    private int _pileCounter = 0;

    private void Start()
    {
        InitEvents();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CardData drawCardData = DeckManager.Instance.DrawCard();

        if(drawCardData == null)
        {
            return;
        }

        DrawCard(drawCardData);
    }

    private void DrawCard(CardData cardToInsert)
    {
        // Check if the current pile cards in one less, due to a previous pick command
        if(_guiCardsPile.Count <= 3)
        {
            GUICard newGuiCard = Instantiate(Resources.Load<GUICard>("GUICard"), _drawPileCardsParent);
            _guiCardsPile.Add(newGuiCard);
            newGuiCard.gameObject.SetActive(false);
        }

        GUICard guiCardToInsert = null;
        if (_pileCounter >= 3)
        {
            guiCardToInsert = _guiCardsPile[_pileCounter];
            guiCardToInsert.SetCardData(cardToInsert, CardArea.DrawPile);
            guiCardToInsert.gameObject.SetActive(true);
            guiCardToInsert.FlipCard(CardSide.Front);
            guiCardToInsert.SetSortingOrder(4);
            guiCardToInsert.EnableRaycast(true);
            iTween.MoveTo(guiCardToInsert.gameObject, _drawPilePositions[2].position, 0.4f);
        }
        else
        {
            guiCardToInsert = _guiCardsPile[_pileCounter];
            guiCardToInsert.gameObject.SetActive(true);
            guiCardToInsert.SetCardData(cardToInsert, CardArea.DrawPile);
            guiCardToInsert.FlipCard(CardSide.Front);
            guiCardToInsert.SetSortingOrder(_pileCounter + 1);
            guiCardToInsert.EnableRaycast(true);
            iTween.MoveTo(guiCardToInsert.gameObject, _drawPilePositions[_pileCounter].position, 0.4f);
        }

        ShiftToLeft();
    }

    private void ShiftToRight() 
    {
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

            iTween.MoveTo(hiddenCard.gameObject, _drawPilePositions[0].position, 0.1f);
            hiddenCard.gameObject.SetActive(true);
            hiddenCard.SetCardData(_hiddenCards[_hiddenCards.Count - 1], CardArea.DrawPile);
            hiddenCard.FlipCard(CardSide.Front);
            hiddenCard.SetSortingOrder(1);
            hiddenCard.EnableRaycast(false);
            _hiddenCards.Remove(hiddenCard.CardDataReference);

            _guiCardsPile[0].SetSortingOrder(2);
            _guiCardsPile[0].EnableRaycast(false);
            iTween.MoveTo(_guiCardsPile[0].gameObject, _drawPilePositions[1].position, 0.4f);

            _guiCardsPile[1].SetSortingOrder(3);
            _guiCardsPile[1].EnableRaycast(true);
            iTween.MoveTo(_guiCardsPile[1].gameObject, _drawPilePositions[2].position, 0.4f);

            /// Shift the last card reference in the list to be the first
            GUICard listCardShift = _guiCardsPile[_guiCardsPile.Count - 1];
            _guiCardsPile.Remove(listCardShift);
            _guiCardsPile.Insert(0, listCardShift);
        }
        else
        {
            _pileCounter--;

            if(_pileCounter > 0)
            {
                _guiCardsPile[_pileCounter -1].EnableRaycast(true);
            }
        }
    }

    private void ShiftToLeft()
    {
        if (_pileCounter >= 3)
        {
            _guiCardsPile[1].SetSortingOrder(1);
            _guiCardsPile[1].EnableRaycast(false);
            iTween.MoveTo(_guiCardsPile[1].gameObject, _drawPilePositions[0].position, 0.4f);

            _guiCardsPile[2].SetSortingOrder(2);
            _guiCardsPile[2].EnableRaycast(false);
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
            if (_pileCounter > 0)
            {
                _guiCardsPile[_pileCounter - 1].EnableRaycast(false);   
            }

            _pileCounter++;
        }
    }

    private IEnumerator PutCardOnDeck()
    {
        GUICard shiftInDeckCard = _guiCardsPile[_pileCounter];
        shiftInDeckCard.FlipCard(CardSide.Back);

        iTween.MoveTo(shiftInDeckCard.gameObject, _drawPileCardsParent.position, 0.4f);

        yield return new WaitForSeconds(0.4f);

        shiftInDeckCard.gameObject.SetActive(false);
    }

    private IEnumerator ResetPile(int pileToReset)
    {
        if (_guiCardsPile[pileToReset].gameObject.activeInHierarchy)
        {
            iTween.MoveTo(_guiCardsPile[pileToReset].gameObject, _drawPileCardsParent.position, 0.4f);
            _guiCardsPile[pileToReset].FlipCard(CardSide.Back);
            yield return new WaitForSeconds(0.4f);
            _guiCardsPile[pileToReset].gameObject.SetActive(false);
        }
    }

    private IEnumerator UndoResetPile(int pileToUndo)
    {
        GUICard guiCardToUndo = _guiCardsPile[pileToUndo];
        guiCardToUndo.gameObject.SetActive(true);
        guiCardToUndo.SetCardData(DeckManager.Instance.DrawnCards[(DeckManager.Instance.DrawnCards.Count - 1) - (2 - pileToUndo)], CardArea.DrawPile);
        guiCardToUndo.FlipCard(CardSide.Front);
        guiCardToUndo.SetSortingOrder(pileToUndo + 1);
        iTween.MoveTo(guiCardToUndo.gameObject, _drawPilePositions[pileToUndo].position, 0.4f);

        yield return null;
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardMove.AddListener(HandleEventCardMove);
        EventsManager.Instance.OnUndoCardMove.AddListener(HandleEventUndoCardMove);
        EventsManager.Instance.OnUndoDraw.AddListener(HandleEventUndoDraw);
        EventsManager.Instance.OnDeckEmpty.AddListener(HandleEventDeckEmpty);
        EventsManager.Instance.OnReset.AddListener(HandleEventReset);
        EventsManager.Instance.OnUndoReset.AddListener(HandleEventUndoReset);
    }

    private void HandleEventCardMove(GUICard guiCard, Transform destinationParent)
    {
        if (!_guiCardsPile.Contains(guiCard))
            return;

        _guiCardsPile.Remove(guiCard);
        ShiftToRight();
    }

    private void HandleEventUndoCardMove(GUICard guiCard, Transform sourceParent)
    {
        if (sourceParent.name != _drawPileCardsParent.name)
            return;

        ShiftToLeft();
        _guiCardsPile.Insert(_pileCounter - 1, guiCard);
        iTween.MoveTo(guiCard.gameObject, _drawPilePositions[_pileCounter-1].position, 0.4f);
        guiCard.transform.SetParent(_drawPileCardsParent);
        guiCard.SetCardArea(CardArea.DrawPile);
    }

    private void HandleEventUndoDraw()
    {
        _deckImage.sprite = Resources.Load<Sprite>("Sprite_Back");

        ShiftToRight();
        StartCoroutine(PutCardOnDeck());
    }

    private void HandleEventDeckEmpty()
    {
        _deckImage.sprite = Resources.Load<Sprite>("Sprite_CardTransparent");
    }

    private void HandleEventReset()
    {
        //Debug.Log("HandleEventReset");

        _deckImage.sprite = Resources.Load<Sprite>("Sprite_Back");

        StartCoroutine(ResetPile(0));
        StartCoroutine(ResetPile(1));
        StartCoroutine(ResetPile(2));

        _hiddenCards.Clear();
        _pileCounter = 0;
    }

    private void HandleEventUndoReset()
    {
        //Debug.Log("HandleEventUndoReset");

        _deckImage.sprite = Resources.Load<Sprite>("Sprite_CardTransparent");

        StartCoroutine(UndoResetPile(0));
        StartCoroutine(UndoResetPile(1));
        StartCoroutine(UndoResetPile(2));

        int totalCardsActive = 0;

        for (int i = 0; i < _drawPileCardsParent.childCount; i++)
        {
            GameObject cardObj = _drawPileCardsParent.GetChild(i).gameObject;
            if (cardObj.activeInHierarchy)
                totalCardsActive++;
        }

        for (int i = 0; i < DeckManager.Instance.DrawnCards.Count - 3; i++)
        {
            CardData drawCard = DeckManager.Instance.DrawnCards[i];
            _hiddenCards.Add(drawCard);
        }

        _pileCounter = totalCardsActive;
    }
    #endregion
}
