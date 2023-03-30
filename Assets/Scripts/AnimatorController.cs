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
    public bool alternateCalc = false;
    #endregion

    #region private declaration
    private List<GameObject> _contentList = new List<GameObject>();
    private float _viewportHeight;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Get list of content components from the panel
        for(int i = 0; i < contentPanel.transform.childCount; i++)
        {
            GameObject child = contentPanel.transform.GetChild(i).gameObject;
            if (child.transform.CompareTag("SubContentContainer")) _contentList.Add(child);
        }
        Debug.Log($"Children container count: {_contentList.Count}");
        // Get view port height
        RectTransform svRt = scrollView.GetComponent<RectTransform>();
        _viewportHeight = svRt.rect.height;
    }

    // Update is called once per frame
    void Update() { }

    public void OnScrolled(Vector2 scrolledPosition)
    {
        // Trigger the animation during scrolling  
        TriggerAnimationV2();
    }

    private void TriggerAnimationV2()
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
                    Debug.Log($"name: {content.name}, calculated pos y: {ch}, normalized y: {ch / _viewportHeight}, content height: {contentHeight}, threshold: {contentHeight * animationTriggerThreshold}, triggered: {isThresholdHit}");
                    // Trigger animation
                    if (isThresholdHit) selfAnimator.SetBool("content_slide_in", true);
                    // Debug.Log($"screenpos: {_camera.WorldToScreenPoint(content.transform.position)}");
                }
            }
            catch(Exception e) { }
        }
    }
}
