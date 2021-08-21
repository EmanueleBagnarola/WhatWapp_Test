using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GUICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Public reference variables
    public CardData CardDataReference
    {
        get
        {
            return _currentCardData;
        }
    }
    public CardSide CurrentSide
    {
        get
        {
            return _currentSide;
        }
    }
    public CardArea CardArea
    {
        get
        {
            return _cardArea;
        }
    }

    public List<GUICard> AppendedCards
    {
        get
        {
            return _appendedCards;
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
    private Vector3 _dragStartPosition = Vector3.zero;
    private Vector3 _currentDragPosition = Vector3.zero;
    private bool _dragging = false;
    //private bool _resetPosition = false;
    #endregion

    #region Visual sorting variables
    private CanvasGroup _canvasGroup = null;
    private Canvas _canvas = null;
    private bool _beginDragOverrideSorting = false;
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
    /// 
    /// </summary>
    /// <param name="newParent"></param>
    public void UpdateParent(Transform newParent)
    {
        _currentParent = newParent;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = sortingOrder;
    }

    public void EnableRaycast(bool value)
    {
        _canvasGroup.blocksRaycasts = value;
    }

    public void SetStartAppendDragPosition(Vector3 position)
    {
        _dragStartPosition = position;
    }

    public void SetAppended(bool value)
    {
        _isAppended = value;
    }

    public void AppendDraggingCards(GUICard cardToAppend)
    {
        cardToAppend.SetAppended(true);
        cardToAppend.SetStartAppendDragPosition(cardToAppend.transform.position);
        cardToAppend.EnableRaycast(false);
        _appendedCards.Add(cardToAppend);   
    }

    public void ReleaseAppendedCard(GUICard cardToRelease)
    {
        _appendedCards.Remove(cardToRelease);
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

        GameManager.Instance.MoveSystem.CheckIfStackable();

        _canvas.overrideSorting = _beginDragOverrideSorting;
        _canvas.sortingOrder = _beginDragSortingOrder;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardDropped.Invoke();

        _canvas.overrideSorting = _beginDragOverrideSorting;
        _canvas.sortingOrder = _beginDragSortingOrder;
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

        transform.position = Vector3.Lerp(transform.position, _currentDragPosition, 20 * Time.deltaTime);

        if(_appendedCards.Count > 0)
        {
            for (int i = 0; i < _appendedCards.Count; i++)
            {
                Transform cardTransform = _appendedCards[i].transform;

                cardTransform.position = Vector3.Lerp(cardTransform.position, transform.position + new Vector3(0, -70f, 0), 17 * Time.deltaTime);
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
        if (!transform.parent.name.Contains("temp"))
            return;

        // Disable the temp object used to place the card using move animation
        transform.parent.gameObject.SetActive(false);
        // Set the new parent 
        transform.SetParent(_currentParent);
    }

    private void HandleCardMove(GUICard guiCard, Transform destinationParent)
    {
        // Check if the the card moved is this
        if (guiCard != this)
            return;

        _canvas.overrideSorting = false;
        _canvas.sortingOrder = 1;
        _canvasGroup.blocksRaycasts = true;
        _dragging = false;
    }

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
        }

        _appendedCards.Clear();

        _canvas.overrideSorting = _beginDragOverrideSorting;
        _canvas.sortingOrder = _beginDragSortingOrder;
        _canvasGroup.blocksRaycasts = true;
        _dragging = false;
    }
    #endregion
}
