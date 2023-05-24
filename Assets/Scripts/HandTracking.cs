using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HandTracking : MonoBehaviour
{
    public LeapProvider _leadProvider;
    [Header("Hand Setting")]
    public float startTriggeringGestureThreshold = 1.5f;
    [Header("Click Setting")]
    public bool enableClick = true;
    [Header("Swipe Setting")]
    public bool enableSwipe = true;
    public float SwipeFilterThreshold = 0.25f;
    [Tooltip("In second")]
    public float SwipeTimeoutDuration = 1f;
    public float handRotationUpperThreshold = 0.785f;
    public float handRotationLowerThreshold = 0.675f;
    [Header("Back Gesture Setting")]
    public bool enableBackGesture = true;
    public float backGestureThreshold = 0.4f;
    public float thumbStrengthThreshold = 0.33f;
    public float indexStrengthThreshold = 0.725f;
    public float middleStrengthThreshold = 0.725f;
    public float ringStrengthThreshold = 0.725f;
    public float pinkyStrengthThreshold = 0.625f;
    [Header("Scroll Setting")]
    public bool enableScroll = true;
    [Tooltip("In second")]
    public float scrollTimeoutDuration = 1.5f;
    // Hand is slightly up
    public float palmFlatUpperThreshold = 0.145f;
    // Hand is slightly down
    public float palmFlatLowerThreshold = -0.145f;
    public float palmRotationZThreshold = 0.145f;
    public float scrollAmplifyValue = 0.1f;
    public ScrollRect scrollRect;
    [Header("Raycasting")]
    public bool enableRaycast = false;
    public InteractionHand interactionLeftHand;
    public InteractionHand interactionRightHand;
    public float dist = 10;
    public bool withLayerMask = false;
    public int layerMaskID = 6;
    [Header("Misc")]
    public bool disableDataLogging = false;

    public static HandTracking instance;
    public bool isScrolling = false;
    private void Awake()
    {
        instance = this;
        _leadProvider.OnUpdateFrame += OnHandUpdated;
    }
    // Start is called before the first frame update

    void Start()
    {
        TryGetTheFirstActiveScrollView();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnApplicationQuit()
    {
        if(_leadProvider != null) _leadProvider.OnUpdateFrame -= OnHandUpdated;
    }

    private void TryGetTheFirstActiveScrollView()
    {
        // Do not search for inactive object
        ScrollRect possibleSR = FindFirstObjectByType<ScrollRect>(FindObjectsInactive.Exclude);
        if (scrollRect == null && possibleSR != null)
        {
            scrollRect = possibleSR;
        }
        else Debug.LogWarning("Scroll Rect is not set! Please manually assign one before using scroll gesture!");
    }

    private float _timePassedSinceHandFirstDetected = 0f;
    private float _timePassedSinceLastSwipeTrigger = 0f;
    private float _timePassedSinceLastScrollTrigger = 0f;
    private void OnHandUpdated(Frame frame)
    {
        // If there is no hand detected
        // Then we reset the variables
        if (_leadProvider.CurrentFrame.Hands.Count == 0)
        {
            ResetGestureTrigger();
            return;
        }
        // Manual method to get hands
        for (int i = 0; i < _leadProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand hand = _leadProvider.CurrentFrame.Hands[i];

            if(ShouldStartGestureDetection())
                HandleGestureDetection(hand);

            if (enableRaycast)
                RaycastHand(hand);

            IncreaseGestureTimers();
        }
    }
 
    private void RaycastHand(Hand hand)
    {
        if (enableRaycast && hand != null)
        {
            InteractionHand interactioHand = hand.IsRight ? interactionRightHand : interactionLeftHand;
            // Palm transform
            Transform palmTransform = interactioHand.transform.GetChild(0);
            if (withLayerMask)
            {
                int layerMaskShifted = 1 << layerMaskID;
                if (Physics.Raycast(palmTransform.position, palmTransform.TransformDirection(Vector3.forward), out RaycastHit hit, dist, layerMaskShifted))
                {
                    Debug.DrawRay(palmTransform.position, palmTransform.TransformDirection(Vector3.forward) * dist, Color.red);
                    Debug.Log($"Hit something, {hit.transform.name}, layerMaskId: {layerMaskShifted}");
                }
                else
                {
                    Debug.DrawRay(palmTransform.position, palmTransform.TransformDirection(Vector3.forward) * dist, Color.green);
                    Debug.Log("Hit nothing");
                }
            }
            else
            {
                if (Physics.Raycast(palmTransform.position, palmTransform.TransformDirection(Vector3.forward), out RaycastHit hit))
                {
                    Debug.DrawRay(palmTransform.position, palmTransform.TransformDirection(Vector3.forward) * dist, Color.red);
                    Debug.Log($"Hit something, {hit.transform.name}");
                }
                else
                {
                    Debug.DrawRay(palmTransform.position, palmTransform.TransformDirection(Vector3.forward) * 10, Color.green);
                    Debug.Log("Hit nothing");
                }
            }
        }
    }
    private void ResetGestureTrigger()
    {
        _timePassedSinceLastSwipeTrigger = 0f;
        _timePassedSinceLastScrollTrigger = 0f;
        _timePassedSinceHandFirstDetected = 0f;
    }
    private bool ShouldStartGestureDetection()
    {
        _timePassedSinceHandFirstDetected += (Time.deltaTime / (_leadProvider.CurrentFrame.Hands.Count > 1 ? 2 : 1));
        return _timePassedSinceHandFirstDetected >= startTriggeringGestureThreshold;
    }
    private void HandleGestureDetection(Hand hand)
    {
        float handRotationX = hand.Rotation.x;
        float absHandRotationZ = Math.Abs(hand.Rotation.z);
        int numOfFingersThresholdHit = 0;
        foreach (Finger f in hand.Fingers)
        {
            // d += (hand.GetFingerStrength((int)f.Type) + ", ");
            bool isThresholdHit = hand.GetFingerStrength((int)f.Type) > GetPresetFingerThreshold(f.Type);
            numOfFingersThresholdHit += (isThresholdHit ? 1 : 0);
        }

        if(enableScroll)
            HandleScrollGesture(handRotationX, absHandRotationZ, numOfFingersThresholdHit);
        if (enableBackGesture)
            HandleBackGesture(handRotationX, numOfFingersThresholdHit, hand.Fingers.Count);
        if (enableSwipe)
            HandleSwipeGesture(hand.PalmVelocity, absHandRotationZ);
    }
    private void IncreaseGestureTimers()
    {
        _timePassedSinceLastScrollTrigger += Time.deltaTime;
        _timePassedSinceLastSwipeTrigger += Time.deltaTime;
    }
    private void HandleSwipeGesture(Vector3 handVelo, float handRotationZ)
    {
        if (_timePassedSinceLastSwipeTrigger >= SwipeTimeoutDuration)
        {
            bool isHandVerticalBasedOnRotation = handRotationZ >= handRotationLowerThreshold && handRotationZ <= handRotationUpperThreshold;
            // bool isHandFlat = handRotationW >= handRotationLowerThreshold && handRotationW <= handRotationUpperThreshold;
            if (isHandVerticalBasedOnRotation)
            {
                if (!disableDataLogging) Debug.Log($"Hand is ready for performing swipe gesture! magnitude: {handVelo.magnitude}");
                if (Mathf.Abs(handVelo.x) >= SwipeFilterThreshold)
                {
                    bool isSwipeLeft = handVelo.x < 0;
                    if (!disableDataLogging) Debug.Log($"is swipe left: {isSwipeLeft}");

                    if (SceneLoader.instance.currentSceneName == "PublicationBook")
                    {
                        if (isSwipeLeft) AutoFlipRaw.instance.FlipRightPage();
                        else AutoFlipRaw.instance.FlipLeftPage();
                    }
                    else
                    {
                        if (isSwipeLeft) SceneLoader.instance.LoadNext();
                        else SceneLoader.instance.LoadPrev();
                    }
                    _timePassedSinceLastSwipeTrigger = 0f;
                }
            }
        }
    }
    private void HandleBackGesture(float handRotationX, int numOfFingersThresholdHit, int totalFingersCount)
    {
        // If hit the go back gesture threshold
        // Then go back
        bool isFingerCrawled = numOfFingersThresholdHit >= (Mathf.CeilToInt(totalFingersCount / 2));
        float reversedRotationX = handRotationX * -1;
        if(!disableDataLogging) Debug.Log($"HandleBackGesture called, isFingerCrawled: {isFingerCrawled}, hand rotation: {reversedRotationX}, threshold: {backGestureThreshold}");
        if (isFingerCrawled && reversedRotationX >= backGestureThreshold && enableBackGesture)
        {
            if (!disableDataLogging) Debug.Log("Go back gesture detected!");
            SceneLoader.instance.LoadMainMenu();
        }
    }
    private void HandleScrollGesture(float rotationX, float handRotationZ, int numOfFingersThresholdHit)
    {
        // If the scroll timeout threshold is hitted
        if (_timePassedSinceLastScrollTrigger >= scrollTimeoutDuration)
        {
            if (!disableDataLogging) Debug.Log("Timeout hit");
            // ###
            // rotationX 
            // -ve => Hand points upward
            // +ve => Hand points downward
            // ###
            bool isHandFlatBasedOnRotation = rotationX >= palmFlatLowerThreshold && rotationX <= palmFlatUpperThreshold;
            bool isFingerAllStretched = numOfFingersThresholdHit == 0;
            // Debug.Log($"stretch: {isFingerAllStretched}, num: {numOfFingersThresholdHit}");
            // Fixed value for handRotaionZ comparison first
            if (!isHandFlatBasedOnRotation && handRotationZ <= palmRotationZThreshold && isFingerAllStretched)
            {
                bool isPointingDownward = rotationX > 0;
                if (!disableDataLogging) Debug.Log($"{(isPointingDownward ? "Hand down" : "Hand up")}, raw: {rotationX}, amplify: {rotationX * scrollAmplifyValue}");

                if (scrollRect != null)
                {
                    scrollRect.verticalNormalizedPosition += (rotationX * scrollAmplifyValue);
                }
                isScrolling = true;
            }
            else isScrolling = false;
        }
    }

    private float GetPresetFingerThreshold(Finger.FingerType type)
    {
        float threshold;
        switch (type)
        {
            case Finger.FingerType.TYPE_THUMB:
                threshold = thumbStrengthThreshold;
                break;
            case Finger.FingerType.TYPE_INDEX:
                threshold = indexStrengthThreshold;
                break;
            case Finger.FingerType.TYPE_MIDDLE:
                threshold = middleStrengthThreshold;
                break;
            case Finger.FingerType.TYPE_RING:
                threshold = ringStrengthThreshold;
                break;
            case Finger.FingerType.TYPE_PINKY:
                threshold = pinkyStrengthThreshold;
                break;
            default:
            case Finger.FingerType.TYPE_UNKNOWN:
                threshold = 0f;
                break;
        }

        return threshold;
    }
}
