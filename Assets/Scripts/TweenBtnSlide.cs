using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenBtnSlide : MonoBehaviour
{
    /*
    [SerializeField] GameObject buttonGroup;
    public float posXDiff = 300;
    public float animateDuration;
    public float delay = 0.5f;
    */

    [SerializeField] GameObject buttonGroup;
    public float xDiff = 300;
    public float slideInDelay = 0.05f;
    public float slideInDuration = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        // Start the animation coroutine
        StartCoroutine(SlideInChildButtons());

        /*
        // Get the transform component of the parent game object
        Transform parentTransform = buttonGroup.transform;

        // Loop through all the child objects of the parent game object
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            // Get the child object by its index
            GameObject childObject = parentTransform.GetChild(i).gameObject;

            // Print out child object name
            Debug.Log($"Child name: {childObject.name}");

            // Do something with the child object
            LeanTween.moveX(childObject, posXDiff, animateDuration).setDelay(delay).setEase(LeanTweenType.easeInCubic);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SlideInChildButtons()
    {
        // Get the child buttons of the buttonGroup game object
        Transform buttonGroupTransform = buttonGroup.transform;
        int childCount = buttonGroupTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            // Get the next child button and its RectTransform component
            Transform childTransform = buttonGroupTransform.GetChild(i);
            RectTransform childRectTransform = childTransform.GetComponent<RectTransform>();

            /*
            // Set the initial position of the child button to be offscreen to the left
            Vector3 startPos = childRectTransform.anchoredPosition;
            childRectTransform.anchoredPosition = new Vector3(-Screen.width, startPos.y, startPos.z);
            */

            // Wait for the slide in delay before starting the slide in animation
            yield return new WaitForSeconds(slideInDelay);

            // Slide in the child button using LeanTween
            LeanTween.moveX(childRectTransform, xDiff, slideInDuration).setEase(LeanTweenType.easeInCubic);

            // Wait for the slide in animation to finish
            //yield return new WaitForSeconds(slideInDuration);
        }
    }
}
