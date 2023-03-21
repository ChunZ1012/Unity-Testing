using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracking : MonoBehaviour
{
    public LeapProvider _leadProvider;

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

    private void OnHandUpdated(Frame frame)
    {
        // Helper function to get hands
        _leadProvider.CurrentFrame.GetHand(Chirality.Left);
        _leadProvider.CurrentFrame.GetHand(Chirality.Right);

        // Manual method to get hands
        for (int i = 0; i < _leadProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand hand = _leadProvider.CurrentFrame.Hands[i];
            PrintHandData(hand);
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
