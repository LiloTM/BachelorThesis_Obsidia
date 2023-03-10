using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_UI_Cards : MonoBehaviour
{
    private SO_Card SOcard;
    public UnityEvent<SO_Card> buttonPressEvent;

    [SerializeField] private TMPro.TMP_Text _valueText;
    [SerializeField] private Image _colourImage;
    [SerializeField] private Button button;

    private void Start()
    {
        if(button!=null)
            button.onClick.AddListener(onClick);
    }

    public void OnSpawn(SO_Card cardLogic)
    {
        SOcard = cardLogic;
        _valueText.text = SOcard.value.ToString();
        switch (SOcard.colour)
        {
            case CardColour.Blue:
                _colourImage.color = Color.blue;
                break;
            case CardColour.Green:
                _colourImage.color = Color.green;
                break;
            case CardColour.Red:
                _colourImage.color = Color.red;
                break;
            case CardColour.Purple:
                _colourImage.color = Color.magenta;
                break;
            case CardColour.Special:
                _colourImage.color = Color.white;
                _valueText.text = SOcard.cardType.ToString();
                break;
            default:
                break;
        }
    }

    public void ToggleButton(bool isActive)
    {
        if(button!=null)
            button.interactable = isActive;
    }

    private void onClick()
    {
        buttonPressEvent?.Invoke(SOcard);
        this.gameObject.SetActive(false);
    }
}
