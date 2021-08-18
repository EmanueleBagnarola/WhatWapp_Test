using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GUICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public CardData CardDataReference
    {
        get
        {
            return _currentCardData;
        }
    }

    public Transform ColumnTransformReference
    {
        get
        {
            return _currentParent;
        }
    }

    public CardSide CurrentSide
    {
        get
        {
            return _currentSide;
        }
    }

    #region GUI settings
    [SerializeField]
    private Image _bodySprite = null;

    [SerializeField]
    private TextMeshProUGUI _rankText = null;

    [SerializeField]
    private Image _suitImageBig = null;

    [SerializeField]
    private Image _suitImageSmall = null;
    #endregion

    private Vector3 _dragStartPosition = Vector3.zero;

    private Vector3 _currentDragPosition = Vector3.zero;

    private bool _dragging = false;

    private bool _resetPosition = false;

    private CardSide _currentSide = CardSide.Back;

    private CardData _currentCardData = null;

    private CanvasGroup _canvasGroup = null;
    private Canvas _canvas = null;

    private Transform _currentParent = null;

    private GUIColumn _guiColumn = null;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvas = GetComponent<Canvas>();
    }

    private void Start()
    {
        InitEvents();
    }

    private void Update()
    {
        HandleDrag();

        HandleResetPosition();
    }

    /// <summary>
    /// Initialize the card GUI with the assigned card data information
    /// </summary>
    /// <param name="cardData"></param>
    public void SetCardData(CardData cardData)
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
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jolly");
                break;

            case 11:
                _rankText.text = "J";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Jack");
                break;

            case 12:
                _rankText.text = "Q";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_Queen");
                break;

            case 13:
                _rankText.text = "K";
                _suitImageBig.sprite = Resources.Load<Sprite>("Sprite_" + cardData.GetCardColor() + "_King");
                break;
        }

        _rankText.color = GetColor(cardData.GetCardColor());
    }

    /// <summary>
    /// Switch card GUI visualization from back to front or viceversa
    /// </summary>
    /// <param name="sideToShow"></param>
    public void FlipCard(CardSide sideToShow)
    {
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

        _currentSide = sideToShow;
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
        _canvas.sortingOrder = sortingOrder;
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

        // override sorting when the card is dragged for the first time. The sorting is set to false when the card is dealed in order to prevent sorting visualization bugs
        // and set the canvas sorting order to be on top of other cards
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 2;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        //EventsManager.Instance.OnCardDropped.Invoke();
        EventsManager.Instance.OnCardStacked.Invoke(null, false, null);
        //_canvas.sortingOrder = 1;
        _canvas.overrideSorting = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardDropped.Invoke();
        //_canvas.sortingOrder = 1;
        _canvas.overrideSorting = false;
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

    private void HandleDrag()
    {
        if (!_dragging || _currentSide == CardSide.Back)
            return;

        transform.position = Vector3.Lerp(transform.position, _currentDragPosition, 20 * Time.deltaTime);
    }

    private void HandleResetPosition()
    {
        if (_resetPosition)
        {
            transform.position = Vector3.Lerp(transform.position, _dragStartPosition, 50 * Time.deltaTime);

            if (Vector3.Distance(transform.position, _dragStartPosition) <= 0.01f)
            {
                _resetPosition = false;
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

    #region Events Handlers
    private void InitEvents()
    {
        EventsManager.Instance.OnCardsDealed.AddListener(HandleEventCardsDealed);
        EventsManager.Instance.OnCardStacked.AddListener(HandleEventCardStacked);
    }

    private void HandleEventCardsDealed()
    {
        // Disable the temp object used to place the card using move animation
        transform.parent.gameObject.SetActive(false);
        // Set the new parent 
        transform.SetParent(_currentParent);

        _guiColumn = transform.parent.GetComponent<GUIColumn>();
        _guiColumn.AddCard(this);
    }

    private void HandleEventCardStacked(GUICard guiCard, bool stacked, Transform newParent)
    {
        // If the stack check failed on the current dragging card, reset its position
        if (!stacked && _dragging)
        {
            _resetPosition = true;
        }

        // Execute the move command
        if (stacked && guiCard == this)
        {
            ICommand moveCommand = new MoveCommand(transform, transform.parent, newParent);
            GameManager.Instance.CommandHandler.AddCommand(moveCommand);

            moveCommand.Execute();

            //_guiColumn.CheckCardFlip();
        }

        _canvasGroup.blocksRaycasts = true;
        _dragging = false;

    }
    #endregion
}
