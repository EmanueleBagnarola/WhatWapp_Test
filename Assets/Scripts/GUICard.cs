using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GUICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Image _bodySprite = null;

    [SerializeField]
    private TextMeshProUGUI _rankText = null;

    [SerializeField]
    private Image _suitImageBig = null;

    [SerializeField]
    private Image _suitImageSmall = null;

    private Vector3 _startingPosition = Vector3.zero;
    private Transform _startingParent = null;

    private Vector3 _currentDragPosition = Vector3.zero;
    private bool _dragging = false;

    private bool _resetPosition = false;

    private CardSide _currentSide = CardSide.Back;

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

        _startingParent = transform.parent;
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
            transform.position = Vector3.Lerp(transform.position, _startingPosition, 50 * Time.deltaTime);

            if (Vector3.Distance(transform.position, _startingPosition) <= 0.01f)
            {
                _resetPosition = false;
                transform.SetParent(_startingParent);
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

    public void OnDrag(PointerEventData eventData)
    {
        _dragging = true;
        _currentDragPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        _startingPosition = transform.position;
        transform.SetParent(GUIManager.Instance.DragParent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_currentSide == CardSide.Back)
            return;

        _dragging = false;
        _resetPosition = true;
    }
}
