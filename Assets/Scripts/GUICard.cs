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
            return _columnTransformReference;
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

    //private Transform _startingParent = null;

    private Vector3 _currentDragPosition = Vector3.zero;

    private bool _dragging = false;

    private bool _resetPosition = false;

    private CardSide _currentSide = CardSide.Back;

    private CardData _currentCardData = null;

    private CanvasGroup _canvasGroup = null;

    private Transform _columnTransformReference = null;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
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
    public void TurnCard(CardSide sideToShow)
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
    /// <param name="columnTransformReference"></param>
    public void UpdateColumnTransformReference(Transform columnTransformReference)
    {
        _columnTransformReference = columnTransformReference;
    }

    public void StackCard(Transform columnStackTransform)
    {
        transform.SetParent(columnStackTransform);
    }

    #region Event System Methods
    public void OnDrag(PointerEventData eventData)
    {
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

        // set the transform parent of the card as an object that isn't the column reference in order to drag it around
        transform.SetParent(GUIManager.Instance.DragParent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        EventsManager.Instance.OnCardDropped.Invoke();
    }

    public void OnDrop(PointerEventData eventData)
    {
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
                transform.SetParent(_columnTransformReference);
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
        transform.SetParent(_columnTransformReference);
    }

    private void HandleEventCardStacked(GUICard guiCard, bool stacked, Transform columnTransformReference)
    {
        if (!stacked && _dragging)
        {
            _resetPosition = true;
        }

        if (stacked && guiCard == this)
        {
            UpdateColumnTransformReference(columnTransformReference);
            transform.SetParent(columnTransformReference);
        }

        _canvasGroup.blocksRaycasts = true;
        _dragging = false;

    }
    #endregion
}
