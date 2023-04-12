using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandTracking : MonoBehaviour
{
    public LeapProvider _leadProvider;
    public float pressThreshold = 0.05f;
    public float swipeRotationThreshold = 0.5f;
    public float swipeThreshold = 0.4f;
    public GameObject button;

    private void Awake()
    {
        _leadProvider.OnUpdateFrame += OnHandUpdated;
        Debug.Log(_leadProvider.name);
    }
    // Start is called before the first frame update

    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
        }

        // Debug.Log($"mouse location: {Input.mousePosition}, converted: {Camera.main.ScreenToViewportPoint(Input.mousePosition)}, world: {Camera.main.ScreenToWorldPoint(Input.mousePosition)}");
    }

    private void OnApplicationQuit()
    {
        _leadProvider.OnUpdateFrame -= OnHandUpdated;
    }

    private Vector3 _oldHandPos = Vector3.zero;
    private void OnHandUpdated(Frame frame)
    {
        // Helper function to get hands
        _leadProvider.CurrentFrame.GetHand(Chirality.Left);
        _leadProvider.CurrentFrame.GetHand(Chirality.Right);

        // Manual method to get hands
        for (int i = 0; i < _leadProvider.CurrentFrame.Hands.Count; i++)
        {
            Hand hand = _leadProvider.CurrentFrame.Hands[i];
            // PrintHandData(hand);

            if (hand != null && hand.IsRight)
            {
                if (hand.Rotation.z > swipeRotationThreshold)
                {
                    // Debug.Log($"velocity: {hand.PalmVelocity}");
                    bool isSwiped = Mathf.Abs(hand.PalmVelocity.x) > swipeThreshold;
                    // Debug.Log($"is swiped: {isSwiped}");

                    if (isSwiped)
                    {
                        // Left swiped: -ve
                        // Right swiped: +ve
                        bool isLeftSwiped = hand.PalmVelocity.x < 0;
                        Debug.Log($"is left swiped: {isLeftSwiped}");
                    }
                }

                // SetCursorPosition(hand.PalmPosition);
                // Debug.Log($"norm pos: {hand.PalmPosition}");
                // Debug.Log($"Palm norm data: {hand.PalmNormal}, palm pos data: {hand.PalmPosition}");
                // if hand is flat and stretch (testing)
                if (Mathf.Abs(hand.PalmNormal.y) >= 0.95)
                {
                   
                }

                // If the hand is moving backward
                if (IsHandMovingBackward(hand.PalmPosition))
                {
                    // record the palm position
                    _oldHandPos = hand.PalmPosition;
                }
                else
                {
                    float posZDiff = hand.PalmPosition.z - _oldHandPos.z;
                    //Debug.Log($"posydiff: {posZDiff}");
                    if (posZDiff > pressThreshold)
                    {
                        //Debug.Log($"Consider pressed! {posZDiff}");

                        // button.GetComponent<Button>().onClick.Invoke();

                        _oldHandPos = hand.PalmPosition;
                    }
                }
            }
            // Reset hand position data
            else _oldHandPos = Vector3.zero;
        }
    }
    private bool IsHandMovingBackward(Vector3 newHandPos)
    {
        return _oldHandPos.z > newHandPos.z;
    }
    private bool IsHandRisingUp(Vector3 newHandPos)
    {
        return Mathf.Abs(_oldHandPos.y) >= Mathf.Abs(newHandPos.y);
    }

    private void SetCursorPosition(Vector3 pos)
    {
        MouseOperations.MousePoint point = new MouseOperations.MousePoint();
        pos.z = Camera.main.nearClipPlane;

        MouseOperations.SetCursorPosition(point);
    }

    public void OnButtonPressed()
    {
        Debug.Log("On button pressed called!");

        // SceneLoader.instance.LoadPage("AboutUsScene");
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
