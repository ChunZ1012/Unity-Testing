using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InteractionBehaviour))]
public class ButtonInteraction : MonoBehaviour
{
    [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
    public bool useHover = true;
    [Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
    public bool usePrimaryHover = false;
    public float ColorTransition = 30f;
    public float hoverThreshold = 0.05f;


    private Color _defaultColor;
    // 
    private InteractionBehaviour _intObj;
    private Button _button;
    private Image _btnImage;
    // Start is called before the first frame update
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        _button = GetComponent<Button>();
        _btnImage = GetComponent<Image>();

        if(_button != null)
        {
            _defaultColor = _button.colors.normalColor;
            // Debug.Log($"button name: {_button.name}");

            Debug.Log($"name: {_button.name}({_intObj.GetInstanceID()})");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_intObj != null && _button != null && _btnImage)
        {
            Color finalColor = _defaultColor;
            // We first check for pressed event, then hover, as the hand will always hover on top of the button
            // thus the pressed event will never be triggered if the hover event is placed before the pressed event
            // If button is pressed, then get the pressed color

            // If button is hovered then get its hovered color
            // Debug.Log($"dist: {_intObj.closestHoveringControllerDistance} ({_button.name})");
            // if (_intObj.isHovered && _intObj.closestHoveringControllerDistance <= hoverThreshold)
            if (_intObj.isPrimaryHovered && usePrimaryHover)
            {
                // Debug.Log("Button hovered!");
                finalColor = _button.colors.highlightedColor;
                Debug.Log($"{_button.name} primary hovered!");
            }
            else if(_intObj.isHovered && useHover)
            {
                // Debug.Log("Button hovered!");
                finalColor = _button.colors.highlightedColor;
                Debug.Log($"{_button.name} hovered!");
            }
            // Check if the button is clicked, and change the button color to corresponding color
            if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed)
            {
                finalColor = _button.colors.pressedColor;
                // _button.onClick.Invoke();
                Debug.Log($"Button clicked!");
            }
            // No need to check if the button is hovered or pressed, to get the default color
            // As we already assign the _defaultColor variable to finalColor
            _btnImage.color = Color.Lerp(_btnImage.color, finalColor, 30F * Time.deltaTime);
        }
        else Debug.LogWarning("Either _intObj, _btnImage or _button is null!");
    }
}
