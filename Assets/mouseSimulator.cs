using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class mouseSimulator : MonoBehaviour
{
    private EventSystem eventSystem;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulateMouseClick();
        }
    }

    void SimulateMouseClick()
    {

        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.button = PointerEventData.InputButton.Left;
        
        pointerData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        var raycastResults = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
        }
    }
}
