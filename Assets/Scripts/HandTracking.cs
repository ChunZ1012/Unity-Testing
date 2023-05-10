using Leap;
using Leap.Unity;
using Leap.Unity.Interaction;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HandTracking : MonoBehaviour
{
    public LeapProvider _leadProvider;
    [Header("Swipe Setting")]
    public float SwipeFilterThreshold = 0.25f;
    [Tooltip("In second")]
    public float SwipeTimeoutDuration = 1f;
    public float handRotationUpperThreshold = 0.785f;
    public float handRotationLowerThreshold = 0.675f;
    [Header("Back Gesture Setting")]
    public float backGestureThreshold = 0.4f;
    public float thumbStrengthThreshold = 0.33f;
    public float indexStrengthThreshold = 0.725f;
    public float middleStrengthThreshold = 0.725f;
    public float ringStrengthThreshold = 0.725f;
    public float pinkyStrengthThreshold = 0.625f;
    [Header("Scroll Setting")]
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
    public InteractionHand interactionHand;
    public float dist = 10;
    [Header("Misc")]
    public bool disableDataLogging = false;
    
    private void Awake()
    {
        _leadProvider.OnUpdateFrame += OnHandUpdated;
        Debug.Log(_leadProvider.name);
    }
    // Start is called before the first frame update

    void Start()
    {
        // _onScrollUp += OnScrollUpEvent.Invoke;
        // _onScrollDown += OnScrollDownEvent.Invoke;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        if(_leadProvider != null) _leadProvider.OnUpdateFrame -= OnHandUpdated;
    }

    private float _timePassedSinceLastSwipeTrigger = 0f;
    private float _timePassedSinceLastScrollTrigger = 0f;
    private void OnHandUpdated(Frame frame)
    {
        // Helper function to get hands
        _leadProvider.CurrentFrame.GetHand(Chirality.Left);
        Hand rightHand = _leadProvider.CurrentFrame.GetHand(Chirality.Right);

      
        if(enableRaycast)
        {
            if (rightHand != null)
            {
                foreach (Finger f in rightHand.Fingers)
                {
                    if (f.Type == Finger.FingerType.TYPE_INDEX)
                    {
                        Vector3 tipPos = f.TipPosition;

                        if (Physics.Raycast(rightHand.PalmNormal, rightHand.Direction, out RaycastHit hit))
                        {
                            Debug.DrawRay(rightHand.PalmNormal, rightHand.Direction * dist, Color.red);
                            Debug.Log("Hit something");
                        }
                        else
                        {
                            Debug.DrawRay(rightHand.PalmNormal, rightHand.Direction * dist, Color.green);
                            Debug.Log("Hit nothing");
                        }
                    }
                }
            }
        }


        // If there is no hand detected
        // Then we reset the variables
        if (_leadProvider.CurrentFrame.Hands.Count == 0)
        {
            _timePassedSinceLastSwipeTrigger = 0f;
            _timePassedSinceLastScrollTrigger = 0f;
            return;
        }
        // Manual method to get hands
        for (int i = 0; i < _leadProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand hand = _leadProvider.CurrentFrame.Hands[i];
            // Debug.Log($"{(hand.IsLeft ? "Left" : "Right")} hand observed!");
            Quaternion handRotation = hand.Rotation;

            // if (!disableDataLogging) Debug.Log($"rotation, x: {handRotation.x}, y: {handRotation.y}, z: {handRotation.z}, w: {handRotation.w}");
            #region Unused
            float handRotationW = handRotation.w;
            float palmFlatPosY = hand.PalmarAxis().y * -1;
            #endregion

            float handRotationZ = Math.Abs(handRotation.z);
            int numOfFingersThresholdHit = 0;
            
            foreach (Finger f in hand.Fingers)
            {
                // d += (hand.GetFingerStrength((int)f.Type) + ", ");
                bool isThresholdHit = hand.GetFingerStrength((int)f.Type) > GetPresetFingerThreshold(f.Type);
                numOfFingersThresholdHit += (isThresholdHit ? 1 : 0);
            }

            // Debug.Log($"Position x: {hand.DistalAxis().x}, y: {hand.DistalAxis().y}, z: {hand.DistalAxis().z}");
            #region Scroll
            // If the scroll timeout threshold is hitted
            if (_timePassedSinceLastScrollTrigger >= scrollTimeoutDuration)
            {
                // bool isHandFlatBasedOnRotation = handRotationW >= handRotationLowerThreshold || handRotationW <= handRotationUpperThreshold;

                // ###
                // -ve => Hand points upward
                // +ve => Hand points downward
                // ###
                float rotationX = hand.Rotation.x;
                bool isHandFlatBasedOnRotation = rotationX >= palmFlatLowerThreshold && rotationX <= palmFlatUpperThreshold;
                bool isFingerAllStretched = numOfFingersThresholdHit == 0;
                // Debug.Log($"stretch: {isFingerAllStretched}, num: {numOfFingersThresholdHit}");
                // Fixed value for handRotaionZ comparison first
                if (!isHandFlatBasedOnRotation && handRotationZ <= palmRotationZThreshold && isFingerAllStretched)
                {
                    bool isPointingDownward = rotationX > 0;
                    if (!disableDataLogging) Debug.Log($"{(isPointingDownward ? "Hand down" : "Hand up")}, raw: {rotationX}, amplify: {rotationX * scrollAmplifyValue}");

                    if(scrollRect != null)
                    {
                        scrollRect.verticalNormalizedPosition += (rotationX * scrollAmplifyValue);
                    }
                }

                /*
                if (palmFlatPosY <= palmFlatUpperThreshold && palmFlatPosY >= palmFlatLowerThreshold && isHandFlatBasedOnRotation)
                {
                    // Debug.Log("Hand is ready to perform scroll gesture!");

                    float palmCurrentPosY = hand.DistalAxis().y;
                    // Pointing down
                    if(palmCurrentPosY < 0)
                    {
                        _onScrollDown.Invoke(palmCurrentPosY);
                    }
                    else
                    {
                        _onScrollUp.Invoke(palmCurrentPosY);
                    }

                    // Get scrolling velocity (if any)
                    float palmVeloY = hand.PalmVelocity.y;
                    if (palmVeloY <= (scrollDownGestureThreshold * -1))
                    {
                        // Debug.Log("Scrolled down!");
                        
                        _timePassedSinceLastScrollTrigger = 0f;
                    }
                    else if (palmVeloY >= scrollUpGestureThreshold)
                    {
                        // Debug.Log("Scolled up!");
                        
                        _timePassedSinceLastScrollTrigger = 0f;
                    }
                }
                */
            }
            #endregion
            // Debug.Log($"minY: {minY}, maxY: {maxY}");
            // Debug.Log($"x: {hand.PalmarAxis().x}, y: {hand.PalmarAxis().y}, z: {hand.PalmarAxis().z}");
            #region Back Gesture
            // Debug.Log($"finger: {d}");
            // Debug.Log($"return? {(numOfFingersThresholdHit >= (Mathf.CeilToInt(hand.Fingers.Count / 2)) && Mathf.Abs(hand.PalmarAxis().y) <= 0.5)}, y: {Mathf.Abs(hand.PalmarAxis().y)}");
            // If hit the go back gesture threshold
            // Then go back
            // float handRotationY = hand.PalmarAxis().y;
            float handRotationX = handRotation.x * -1;
            bool isFingerCrawled = numOfFingersThresholdHit > (Mathf.CeilToInt(hand.Fingers.Count / 2));
            // Debug.Log($"handrotation y: {handRotationX}, is fin hit: {numOfFingersThresholdHit >= Mathf.CeilToInt(hand.Fingers.Count / 2)}, is rot hit: {handRotationX >= backGestureThreshold}");
            if (isFingerCrawled && handRotationX >= backGestureThreshold)
            {
                if (!disableDataLogging) Debug.Log("Go back gesture detected!");
                // SceneLoader.instance.LoadMainMenu();
            }
            #endregion

            #region Swipe
            Vector3 handVelo = hand.PalmVelocity;
            if (_timePassedSinceLastSwipeTrigger >= SwipeTimeoutDuration)
            {
                bool isHandVerticalBasedOnRotation = handRotationZ >= handRotationLowerThreshold && handRotationZ <= handRotationUpperThreshold;
                // bool isHandFlat = handRotationW >= handRotationLowerThreshold && handRotationW <= handRotationUpperThreshold;
                if (isHandVerticalBasedOnRotation)
                {
                    if (!disableDataLogging) Debug.Log("Hand is ready for performing swipe gesture!");
                    if (!disableDataLogging) Debug.Log($"magnitude: " + hand.PalmVelocity.magnitude);
                    if (Mathf.Abs(handVelo.x) >= SwipeFilterThreshold)
                    {
                        bool isSwipeLeft = handVelo.x < 0;
                        if (!disableDataLogging) Debug.Log($"is swipe left: {isSwipeLeft}");
                        // Temp disable the below code
                        //if (isSwipeLeft) SceneLoader.instance.LoadPrev();
                        // else SceneLoader.instance.LoadNext();

                        _timePassedSinceLastSwipeTrigger = 0f;
                    }
                    // else Debug.Log("Threshold not hitted!");
                }
            }
            // else Debug.Log("Not within the time diff");
            #endregion
            // Increase the time
            _timePassedSinceLastScrollTrigger += Time.deltaTime;
            _timePassedSinceLastSwipeTrigger += Time.deltaTime;
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
