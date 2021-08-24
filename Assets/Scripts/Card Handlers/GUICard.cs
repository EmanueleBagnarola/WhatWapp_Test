using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GUICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Public reference variables
    /// <summary>
    /// The card data reference set for this GUICard
    /// </summary>
    public CardData CardDataReference
    {
        get
        {
            return _currentCardData;
        }
    }

    /// <summary>
    /// The current side shown by the card (Front or Back) 
    /// </summary>
    public CardSide CurrentSide
    {
        get
        {
            return _currentSide;
        }
    }

    /// <summary>
    /// The current game area in which this card is (Table, AcePile, DrawnPile)
    /// </summary>
    public CardArea CardArea
    {
        get
        {
            return _cardArea;
        }
    }

    /// <summary>
    /// The list of appended card filled when the player tries to move a stacked pile of cards
    /// </summary>
    public List<GUICard> AppendedCards
    {
        get
        {
            return _appendedCards;
        }
    }

    /// <summary>
    /// The reference of the table pile this card is stored in
    /// </summary>
    public PileHandler PileHandlerParent
    {
        get
        {
            return _pileHandlerParent;
        }
    }
    #endregion

    #region Private reference variables
    private CardSide _currentSide = CardSide.Back;
    private CardArea _cardArea = CardArea.Table;
    private CardData _currentCardData = null;
    private Transform _currentParent = null;
    private List<GUICard> _appendedCards = new List<GUICard>();
    private bool _isAppended = false;
    private PileHandler _pileHandlerParent = null;
    #endregion

    #region GUI Editor variables
    [SerializeField]
    private Image _bodySprite = null;

    [SerializeField]
    private TextMeshProUGUI _rankText = null;

    [SerializeField]
    private Image _suitImageBig = null;

    [SerializeField]
    private Image _suitImageSmall = null;
    #endregion

    #region Drag variables
    /// <summary>
    /// Store the position where the card was before drag started
    /// </summary>
    private Vector3 _dragStartPosition = Vector3.zero;

    /// <summary>
    /// Store the updated position that the card has to follow during the drag
    /// </summary>
    private Vector3 _currentDragPosition = Vector3.zero;

    /// <summary>
    /// Store if the card is currently in drag state
    /// </summary>
    private bool _dragging = false;
    //private bool _resetPosition = false;
    #endregion

    #region Visual sorting variables
    /// <summary>
    /// Component used to enable or disable the raycast event on this UI element
    /// </summary>
    private CanvasGroup _canvasGroup = null;

    /// <summary>
    /// Component used to update the layer order use to see this UI element in top of other elements
    /// </summary>
    private Canvas _canvas = null;

    /// <summary>
    /// Store the value of the override sorting bool before the drag started
    /// </summary>
    private bool _beginDragOverrideSorting = false;

    /// <summary>
    /// Store the sorting order of the UI element before the drag started
    /// </summary>
    private int _beginDragSortingOrder = 0;
    #endregion

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        InitEvents();

        if (_canvas == null)
            return;

        _beginDragOverrideSorting = _canvas.overrideSorting;
        _beginDragSortingOrder = _canvas.sortingOrder;
    }

    private void Update()
    {
        HandleDrag();
    }

    /// <summary>
    /// Initialize the card GUI with the assigned card data information and the area where the card began its life
    /// </summary>
    /// <param name="cardData"></param>
    public void SetCardData(CardData cardData, CardArea cardArea)
    {
        _currentCardData = cardData;

        _rankText.text = cardData.Rank.ToString();

        Sprite suitSprite = Resources.Load<Sprite>("Sprite_" + cardData.Suit);
        _suitImageSmall.sprite = suitSprite;
        _suitImageBig.sprite = suitSprite;

        switch (cardData.Rank)
        {
            case 1:
                _rankText.text = "A";
                _suitImageSmall.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jolly");
                break;

            case 11:
                _rankText.text = "J";
                _suitImageSmall.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jack");
                break;

            case 12:
                _rankText.text = "Q";
                _suitImageSmall.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Queen");
                break;

            case 13:
                _rankText.text = "K";
                _suitImageSmall.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_King");
                break;
        }

        _cardArea = cardArea;
        _rankText.color = GetColor(cardData.GetCardColor());
    }

    public void SetCardArea(CardArea cardArea)
    {
        _cardArea = cardArea;
    }

    /// <summary>
    /// Switch card GUI visualization from back to front or viceversa
    /// </summary>
    /// <param name="sideToShow"></param>
    public void FlipCard(CardSide sideToShow)
    {
        StartCoroutine(FlipAnimation(sideToShow));
    }

    /// <summary>
    /// Update the current parent reference
    /// </summary>
    /// <param name="newParent"></param>
    public void UpdateParent(Transform newParent)
    {
        _currentParent = newParent;
    }
    
    /// <summary>
    /// Set the Canvas sorting order
    /// </summary>
    /// <param name="sortingOrder"></param>
    public void SetSortingOrder(int sortingOrder)
    {
        if (sortingOrder < 0)
        {
            _canvas.overrideSorting = false;
            return;
        }

        _canvas.overrideSorting = true;
        _canvas.sortingOrder = sortingOrder;
    }

    /// <summary>
    /// Enable or disable the ability of this card to be detected by raycast events (i.e. mouse over)
    /// </summary>
    /// <param name="value"></param>
    public void EnableRaycast(bool value)
    {
        _canvasGroup.blocksRaycasts = value;
    }

    /// <summary>
    /// Save the position of this card before the stacked cards drag started.
    /// </summary>
    /// <param name="position"></param>
    public void SetStartAppendDragPosition(Vector3 position)
    {
        _dragStartPosition = position;
    }

    /// <summary>
    /// It this card is one of the appended cards in stacked cards drag, update its state
    /// </summary>
    /// <param name="value"></param>
    public void SetAppended(bool value)
    {
        _isAppended = value;
    }

    /// <summary>
    /// Append cards to this card in case of multiple stacked cards dragging
    /// </summary>
    /// <param name="cardToAppend"></param>
    public void AppendDraggingCards(GUICard cardToAppend)
    {
        cardToAppend.SetAppended(true);
        cardToAppend.SetStartAppendDragPosition(cardToAppend.transform.position);
        cardToAppend.EnableRaycast(false);
        _appendedCards.Add(cardToAppend);   
    }

    /// <summary>
    /// Release the references of stacked dragging cards
    /// </summary>
    public void ReleaseAppendedCards()
    {
        for (int i = 0; i < _appendedCards.Count; i++)
        {
            GUICard appendedCard = _appendedCards[i];
            appendedCard.SetSortingOrder(-1);
            appendedCard.EnableRaycast(true);
        }

        _appendedCards.Clear();
    }

    /// <summary>
    /// Check if the pile in which this card is has a back faced card in the previous position. Used to decide if the MoveCommand has to assign 5 points
    /// </summary>
    /// <param name="cardsPileCount"></param>
    /// <returns></returns>
    public bool IsLastFrontCardInPile(int cardsPileCount)
    {
        Debug.Log(cardsPileCount);
        Debug.Log("IsLastFrontCardInPile");
        PileHandler pileHandler = GetComponentInParent<PileHandler>();

        if (pileHandler == null || pileHandler.CardArea != CardArea.Table)
            return false;

        if (pileHandler.GUICards.Count < 2)
            return false;

        if (pileHandler.GUICards.Count == cardsPileCount)
            return false;

        Debug.Log("PileHandler:" + pileHandler.GUICards.Count);

        GUICard previousCardInPile = pileHandler.GUICards[pileHandler.GUICards.IndexOf(this) - 1];

        Debug.Log("previousCardInPile:" + previousCardInPile.CardDataReference.Rank);

        if (previousCardInPile == null)
            return false;

        if (previousCardInPile != null)
        {
            if (previousCardInPile.CurrentSide == CardSide.Back)
                return true;
        }
        return false;
    }

    #region Event System Methods
    public void OnDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        _dragging = true;
        _currentDragPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardDragging.Invoke(this);

        // disable the raycast block to prevent the cursor to detect this card when trying to drop it on a different card
        _canvasGroup.blocksRaycasts = false;

        _dragStartPosition = transform.position;

        // get the canvas settings before the drag started
        _beginDragOverrideSorting = _canvas.overrideSorting;
        _beginDragSortingOrder = _canvas.sortingOrder;

        // override sorting when the card is dragged for the first time. The sorting is set to false when the card is dealed in order to prevent sorting visualization bugs
        // and set the canvas sorting order to be on top of other cards  
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 5;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        Debug.Log("OnEndDrag: " + CardDataReference.Rank + " of " + CardDataReference.Suit);
        EventsManager.Instance.OnCardDropped.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardPointerEnter.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardPointerExit.Invoke();
    }
    #endregion

    private IEnumerator FlipAnimation(CardSide sideToShow)
    {
        iTween.ScaleTo(gameObject, new Vector3(0, 1, 1), 0.2f);

        yield return new WaitForSeconds(0.1f);

        _bodySprite.sprite = Resources.Load<Sprite>("Sprite_" + sideToShow);

        switch (sideToShow)
        {
            case CardSide.Back:
                _rankText.gameObject.SetActive(false);
                _suitImageBig.gameObject.SetActive(false);
                _suitImageSmall.gameObject.SetActive(false);
                break;

            case CardSide.Front:
                _rankText.gameObject.SetActive(true);
                _suitImageBig.gameObject.SetActive(true);
                _suitImageSmall.gameObject.SetActive(true);
                break;
        }

        iTween.ScaleTo(gameObject, new Vector3(1, 1, 1), 0.2f);

        _currentSide = sideToShow;
    }

    private void HandleDrag()
    {
        if (!_dragging || _currentSide == CardSide.Back)
            return;

        // Move the dragging card
        transform.position = Vector3.Lerp(transform.position, _currentDragPosition, 20 * Time.deltaTime);

        // Move all the appended cards during drag
        if(_appendedCards.Count > 0)
        {
            for (int i = 0; i < _appendedCards.Count; i++)
            {
                Transform cardTransform = _appendedCards[i].transform;

                if(i == 0)
                {
                    cardTransform.position = Vector3.Lerp(cardTransform.position, transform.position + new Vector3(0, -70f, 0), 17 * Time.deltaTime);
                }

                else
                {
                    cardTransform.position = Vector3.Lerp(cardTransform.position, _appendedCards[i-1].transform.position + new Vector3(0, -70f, 0), 17 * Time.deltaTime);
                }
            }
        }
    }

    private Color GetColor(CardColor cardColor)
    {
        if (cardColor == CardColor.Red)
            return Color.red;

        if (cardColor == CardColor.Black)
            return Color.black;

        return Color.black;
    }

    /// <summary>
    /// Move back to saved position before the drag started
    /// </summary>
    private void MoveToEndDragPosition()
    {
        iTween.MoveTo(gameObject, _dragStartPosition, 0.5f);

        if (_isAppended)
            _isAppended = false;
    }

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardMove.AddListener(HandleCardMove);
        EventsManager.Instance.OnCardFailMove.AddListener(HandleCardFailMove);
    }

    private void HandleEventCardsDealed(List<CardData> cardsData)
    {
        EnableRaycast(true);

        if (!transform.parent.name.Contains("temp"))
            return;

        // Disable the temp object used to place the card using move animation
        transform.parent.gameObject.SetActive(false);
        // Set the new parent 
        transform.SetParent(_currentParent);
    }

    /// <summary>
    /// Handles what happens if any MoveCommand has been called
    /// </summary>
    /// <param name="guiCard"></param>
    /// <param name="destinationParent"></param>
    private void HandleCardMove(GUICard guiCard, Transform destinationParent)
    {
        // Check if the the card moved is this
        if (guiCard != this)
            return;

        if (_appendedCards.Count > 0)
        {
            for (int i = 0; i < _appendedCards.Count; i++)
            {
                GUICard appendedCard = _appendedCards[i];
                appendedCard.SetSortingOrder(-1);
            }
        }

        _canvas.overrideSorting = false;
        _canvas.sortingOrder = 1;
        _canvasGroup.blocksRaycasts = true;
        _dragging = false;
    }

    /// <summary>
    /// Handles what happens if the drag ended and any MoveCommand was called
    /// </summary>
    /// <param name="guiCard"></param>
    private void HandleCardFailMove(GUICard guiCard)
    {
        if (guiCard != this)
            return;

        MoveToEndDragPosition();

        if(_appendedCards.Count > 0)
        {
            for (int i = 0; i < _appendedCards.Count; i++)
            {
                GUICard appendedCard = _appendedCards[i];
                appendedCard.MoveToEndDragPosition();
            }

            ReleaseAppendedCards();
        }

        _canvas.overrideSorting = _beginDragOverrideSorting;
        _canvas.sortingOrder = _beginDragSortingOrder;
        _canvasGroup.blocksRaycasts = true;
        _dragging = false;
    }
    #endregion
}
