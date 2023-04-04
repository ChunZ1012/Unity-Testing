using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorController : MonoBehaviour
{
    #region public declaration
    public GameObject contentPanel;
    public GameObject scrollView;
    public float animationTriggerThreshold = 0.135f;
    public List<int> skipComponentIndex = new List<int> { 1 };
    public bool alternateCalc = true;
    #endregion

    #region private declaration
    private List<GameObject> _contentList = new List<GameObject>();
    private float _viewportHeight;
    // Initialize scrolled position with initiail y value of 1
    // As unity count the y from top to bottom (1 until 0)
    private Vector2 _oldScrolledPosition = new Vector2(0, 1);
    // Variable used to store the count of onScroll triggers on scene load
    private int callCount = 0;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Get list of content components from the panel
        for(int i = 0; i < contentPanel.transform.childCount; i++)
        {
            GameObject child = contentPanel.transform.GetChild(i).gameObject;
            // Add object into _contentList if object is tagged "SubContentContainer"
            if (child.transform.CompareTag("SubContentContainer")) {
                _contentList.Add(child);
            }
        }
        Debug.Log($"Children container count: {_contentList.Count}");
        // Get view port height
        RectTransform svRt = scrollView.GetComponent<RectTransform>();
        _viewportHeight = svRt.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel.transform as RectTransform);
    }

    public void OnScrolled(Vector2 scrolledPosition)
    {
        // Trigger the animation during scrolling  
        // Compare the previous scrolled position y with the current scrolled position y
        // If both y are not equal, then animate the content
        if(scrolledPosition.y != _oldScrolledPosition.y)
        {
            _oldScrolledPosition = scrolledPosition;
            TriggerAnimationV2(scrolledPosition);
        }
        // Check if onScroll triggers on scene load have been called
        // Scene load onScroll triggers = number of animated modules * 3
        if (callCount > (_contentList.Count * 3))
        {
            // TriggerAnimationV2();
        }
        else {
            callCount += 1;
        }
    }

    private void TriggerAnimationV2(Vector2 scrolledPosition)
    {
        for (int i = 0; i < _contentList.Count; i++)
        {
            try
            {
                if (skipComponentIndex.Contains(i)) continue;
                else
                {
                    // content panel
                    GameObject content = _contentList[i];
                    // Animator inside the component
                    Animator selfAnimator = content.GetComponent<Animator>();
                    RectTransform rt = content.GetComponent<RectTransform>();
                    // Content's Height
                    float contentHeight = rt.rect.height;
                    // Get the position relative to the scroll view sensitivity
                    float cy = rt.position.y;
                    // Calculated position y
                    float ch = alternateCalc ? (cy + contentHeight) : cy;
                    bool isThresholdHit = (ch >= (contentHeight * animationTriggerThreshold));
                    Debug.Log($"name: {content.name}, original pos y: {cy}, calculated pos y: {ch}, normalized y: {ch / _viewportHeight}, scrolled y: {scrolledPosition.y}, threshold: {contentHeight * animationTriggerThreshold}, triggered: {isThresholdHit}");
                    // Trigger animation
                    if (isThresholdHit) selfAnimator.SetBool("content_slide_in", true);
                    // Debug.Log($"screenpos: {_camera.WorldToScreenPoint(content.transform.position)}");
                }
            }
            catch(Exception e) { }
        }
    }
}
