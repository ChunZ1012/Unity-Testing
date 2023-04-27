using Leap;
using Leap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public LeapProvider _leadProvider;
    public float SwipeFilterThreshold = 0.25f;
    public float TimeoutDuration = 1f;
    [Header("Finger Strength")]
    public float thumbStrengthThreshold = 0.4f;
    public float indexStrengthThreshold = 0.7f;
    public float middleStrengthThreshold = 0.7f;
    public float ringStrengthThreshold = 0.7f;
    public float pinkyStrengthThreshold = 0.5f;

    private void Awake()
    {
        _leadProvider.OnUpdateFrame += OnHandUpdated;
        Debug.Log(_leadProvider.name);
    }
    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        _leadProvider.OnUpdateFrame -= OnHandUpdated;
    }

    private Vector3 _mostLeft = new Vector3(-0.8f, 0, 0);
    private Vector3 _mostRight = new Vector3(0.8f, 0, 0);
    private Vector3 _lastKnownHandPos = Vector3.zero;

    private float _timePassedSinceLastTrigger = 0f;
    private void OnHandUpdated(Frame frame)
    {
        // Helper function to get hands
        _leadProvider.CurrentFrame.GetHand(Chirality.Left);
        _leadProvider.CurrentFrame.GetHand(Chirality.Right);

        if (_leadProvider.CurrentFrame.Hands.Count == 0)
        {
            _timePassedSinceLastTrigger = 0f;
            _lastKnownHandPos = Vector3.zero;
            return;
        }

        // Manual method to get hands
        for (int i = 0; i < _leadProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand hand = _leadProvider.CurrentFrame.Hands[i];
            Debug.Log($"{(hand.IsLeft ? "Left" : "Right")} hand observed!");
            Vector3 curHandPos = Vector3.Normalize(hand.PalmPosition);

            // Debug.Log($"norm: {curHandPos.x}, {curHandPos.y}, {curHandPos.z}");
            // Consider use MoveTowards to determine the hand movement
            // Either from left to right
            // Or from right to left
            // Debug.Log($"{hand.PalmVelocity.x}, {hand.PalmVelocity.y}, {hand.PalmVelocity.z}");

            Vector3 handVelo = hand.PalmVelocity;
            string d = "";

            int numOfThresholdHit = 0;

            foreach (Finger f in hand.Fingers)
            {
                // d += (hand.GetFingerStrength((int)f.Type) + ", ");
                bool isThresholdHit = hand.GetFingerStrength((int)f.Type) > GetPresetFingerThreshold(f.Type);
                numOfThresholdHit += (isThresholdHit ? 1 : 0);

                d += isThresholdHit + ",";
            }
            // Debug.Log($"finger: {d}");
            Debug.Log($"return? {numOfThresholdHit >= (Mathf.CeilToInt(hand.Fingers.Count / 2))}");
            if(numOfThresholdHit >= (Mathf.CeilToInt(hand.Fingers.Count / 2)))
            {
                SceneLoader.instance.LoadMainMenu();
            }

            if (_timePassedSinceLastTrigger >= TimeoutDuration)
            {
                if (Mathf.Abs(handVelo.x) >= SwipeFilterThreshold)
                {
                    bool isSwipeLeft = handVelo.x < 0;
                    // Debug.Log($"is swipe left: {isSwipeLeft}");
                    // Temp disable the below code
                    // if (isSwipeLeft) SceneLoader.instance.LoadPrev();
                    // else SceneLoader.instance.LoadNext();

                    _timePassedSinceLastTrigger = 0f;
                }
                // else Debug.Log("Threshold not hitted!");

                // _lastTriggeredTime = DateTime.Now;
            }
            // else Debug.Log("Not within the time diff");

            _timePassedSinceLastTrigger += Time.deltaTime;

            if (_lastKnownHandPos == Vector3.zero) _lastKnownHandPos = curHandPos;
            // PrintHandData(hand);
        }
    }
    private void PrintHandData(Hand hand)
    {
        Debug.Log($"Hand ({(hand.IsLeft ? "Left" : "Right")}), (x: {hand.Direction.x}, y: {hand.Direction.y}, z: {hand.Direction.z})");
        Debug.Log($"Active finger: {(hand.Fingers.Count < 1 ? "None" : "")}");
        for (int i = 0; i < hand.Fingers.Count; i++)
        {
            Finger f = hand.Fingers[i];
            // PrintFingerName(f);
        }
    }
    private float GetPresetFingerThreshold(Finger.FingerType type)
    {
        float threshold = 0f;
        switch(type)
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
    private void PrintFingerName(Finger f)
    {
        string fingerName = "Unknown";
        switch(f.Type)
        {
            case Finger.FingerType.TYPE_THUMB:
                fingerName = "Thumb";
                break;
            case Finger.FingerType.TYPE_MIDDLE:
                fingerName = "Middle";
                break;
            case Finger.FingerType.TYPE_PINKY:
                fingerName = "Pinky";
                break;
            case Finger.FingerType.TYPE_RING:
                fingerName = "Ring";
                break;
            case Finger.FingerType.TYPE_INDEX:
                fingerName = "Index";
                break;
            default:
                break;
        }

        Debug.Log($"Finger : {fingerName}");
    }
}
