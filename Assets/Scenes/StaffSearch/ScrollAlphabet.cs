using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//scrolling for staff search
public class ScrollAlphabet : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float targetPosition;
    public SlideStaff slideStaff;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //current position of the scroll
        Vector2 contentPosition = scrollRect.content.position;
    }
    //move scroll according to the slider
    public void MoveScroll(float targetPosition)
    {
        Vector2 newPosition = scrollRect.normalizedPosition;
        newPosition.y = targetPosition;
        scrollRect.normalizedPosition = newPosition;

        scrollRect.content.position = scrollRect.content.position;
    }
    //move slider when scrolling
    public void moveSlide()
    {
        Vector2 newPosition = scrollRect.normalizedPosition;
        slideStaff.MoveSlider(1f - newPosition.y);
    }
}
