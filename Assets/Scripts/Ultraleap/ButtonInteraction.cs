using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InteractionBehaviour))]
public class ButtonInteraction : MonoBehaviour
{
    [Tooltip("If enabled, the object will lerp to its hoverColor when a hand is nearby.")]
    public bool useHover = false;
    [Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
    public bool usePrimaryHover = true;
    public float ColorTransition = 30f;
    [Header("Hover Setting")]
    public bool enableHoverThreshold = false;
    public float hoverThreshold = 0.15f;

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

        if (_button != null)
        {
            _defaultColor = _button.colors.normalColor;
        }
        _intObj.overrideInteractionLayer = true;
        _intObj.rigidbody.isKinematic = false;
        foreach(Collider cd in _intObj.primaryHoverColliders)
        {
            cd.isTrigger = false;
            cd.enabled = true;
        }

        _intObj.OnSuspensionBegin += (controller) =>
        {
            Debug.Log($"suspend begin on {controller.name}");
        };
        _intObj.OnPrimaryHoverBegin += () =>
        {
            Debug.Log($"primary hover begin on {_button.name}");
        };
        _intObj.OnPrimaryHoverEnd += () =>
        {
            Debug.Log($"primary hover end on {_button.name}");
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (_intObj != null && _button != null && _btnImage != null)
        {
            Color finalColor = _defaultColor;
               // If button is hovered then get its hovered color
            // Debug.Log($"dist: {_intObj.closestHoveringControllerDistance} ({_button.name})");
            // if (_intObj.isHovered && _intObj.closestHoveringControllerDistance <= hoverThreshold)
            float dist = usePrimaryHover ? _intObj.primaryHoverDistance : _intObj.closestHoveringControllerDistance;
            // if (dist != float.PositiveInfinity) Debug.Log($"{_button.name}: {dist}");

            bool flag = enableHoverThreshold && (dist != float.PositiveInfinity && (dist <= hoverThreshold));
            // Debug.Log($"button name: {_button.name}, flag: {flag}, value: {dist}");
            if (_intObj.isPrimaryHovered && usePrimaryHover)
            {
                // Debug.Log("Button hovered!");
                finalColor = _button.colors.highlightedColor;
            }
            else if(_intObj.isHovered && useHover)
            {
                // Debug.Log("Button hovered!");
                finalColor = _button.colors.selectedColor;
            }
            else if(flag)
            {
                finalColor = _button.colors.disabledColor;
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
            _btnImage.color = Color.Lerp(_btnImage.color, finalColor, ColorTransition * Time.deltaTime);
        }
        else Debug.LogWarning("Either _intObj, _btnImage or _button is null!");
    }
}
