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
    private GUICard[] _guiCardsPile = null;

    private int _currentDrawPilePosition = 0;

    private void Start()
    {
        InitEvents();
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        _guiCardsPile[0].SetCardData(DeckManager.Instance.DrawCard(0));
        _guiCardsPile[0].FlipCard(CardSide.Front);

        if(_currentDrawPilePosition < 3)
        {
            _currentDrawPilePosition++;
        }
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnUndoDeckCard.AddListener(HandleEventUndoDeckCard);
    }

    private void HandleEventUndoDeckCard(GUICard guiCard)
    {
        //throw new NotImplementedException();
        _guiCardsPile[0].SetCardData(DeckManager.Instance.DrawCard(0));
        _guiCardsPile[0].FlipCard(CardSide.Front);
    }
    #endregion
}
