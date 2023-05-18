using Leap.Unity.Interaction;
using System.Collections;
using System.Threading.Tasks;
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
    [Header("Misc")]
    public int invokedThresholdBeforeIgnoring = 1;
    public bool requireImageComponent = true;
    public bool enableClickDelay = false;
    [Tooltip("In seconds")]
    public float enableClickDelaySecond = 0.25f;

    private int _invokedCount = 0;
    private ButtonScript _buttonScript;
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
        if(requireImageComponent) _btnImage = GetComponent<Image>();

        _buttonScript = GetComponent<ButtonScript>();

        if (_button != null) _defaultColor = _button.colors.normalColor;
        /*
        _intObj.overrideInteractionLayer = true;
        _intObj.rigidbody.isKinematic = false;
        foreach(Collider cd in _intObj.primaryHoverColliders)
        {
            cd.isTrigger = false;
            cd.enabled = true;
        }
        */
        _intObj.OnSuspensionBegin += (controller) =>
        {
            Debug.Log($"suspend begin on {controller.name}");
        };
        _intObj.OnPrimaryHoverBegin += () =>
        {
            // Debug.Log($"primary hover begin on {_button.name}");

            if (_buttonScript != null) _buttonScript.onMouseEnter();
        };
        _intObj.OnPrimaryHoverEnd += () =>
        {
            // Debug.Log($"primary hover end on {_button.name}");
            if (_buttonScript != null) _buttonScript.onMouseExit();
        };
    }

    // Update is called once per frame
    void Update()
    {
        // if (_intObj != null && _button != null && (!requireImageComponent || _btnImage != null) && !HandTracking.instance.isScrolling)
        if (_intObj != null && _button != null && (!requireImageComponent || _btnImage != null))
        {
            // Debug.Log("ButtonInteraction Update called");
            Color finalColor = _defaultColor;
              // If button is hovered then get its hovered color
            float dist = usePrimaryHover ? _intObj.primaryHoverDistance : _intObj.closestHoveringControllerDistance;

            bool flag = enableHoverThreshold && (dist != float.PositiveInfinity && (dist <= hoverThreshold));
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
            if (_intObj is InteractionButton && (_intObj as InteractionButton).isPressed && !_button.IsInvoking() && _invokedCount < invokedThresholdBeforeIgnoring && HandTracking.instance.enableClick)
            {
                finalColor = _button.colors.pressedColor;
                ++_invokedCount;
                if (enableClickDelay)
                {
                    StartCoroutine(TriggerButtonClick());
                }
                else _button.onClick.Invoke();

                Debug.Log($"Button clicked!");
            }
            // No need to check if the button is hovered or pressed, to get the default color
            // As we already assign the _defaultColor variable to finalColor
            if(requireImageComponent) _btnImage.color = Color.Lerp(_btnImage.color, finalColor, ColorTransition * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("Either _intObj, _btnImage or _button is null!");
        }

    }
    private IEnumerator TriggerButtonClick()
    {
        yield return new WaitForSeconds(enableClickDelaySecond);
        _button.onClick.Invoke();
    }
    private void OnDestroy()
    {
        _button = null;
        _intObj = null;
        _buttonScript = null;
        _btnImage = null;
        _invokedCount = 0;
    }
}
