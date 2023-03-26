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
    public float animationTriggerThreshold;
    public List<int> skipComponentIndex;
    public bool reverseCalc = true;
    #endregion

    #region private declaration
    private List<GameObject> _contentList = new List<GameObject>();
    private float _layoutSpacing;
    private float _viewportHeight;
    private bool _isUpdated = false;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _layoutSpacing = contentPanel.GetComponent<VerticalLayoutGroup>().spacing;
        // Get list of content components from the panel
        for(int i = 0; i < contentPanel.transform.childCount; i++)
        {
            GameObject child = contentPanel.transform.GetChild(i).gameObject;
            if (child.transform.CompareTag("SubContentContainer")) _contentList.Add(child);
        }
        Debug.Log($"Children container count: {_contentList.Count}");
        // Get view port height
        Transform scrollView = contentPanel.transform.parent;
        RectTransform svRt = scrollView.GetComponent<RectTransform>();
        _viewportHeight = svRt.rect.height;

    }

    // Update is called once per frame
    void Update()
    {
        // UpdateRectTransform();
    }

    private void UpdateRectTransform()
    {
        foreach(GameObject go in _contentList)
        {
            // Self's rect transform
            RectTransform self = go.GetComponent<RectTransform>();
            // Get first child's rect transform
            RectTransform child = self.GetChild(0).GetComponent<RectTransform>();

            self.sizeDelta = new Vector2(self.rect.width, child.rect.height);

            Debug.Log($"child name: {child.name}, self name: {self.name}");
            Debug.Log($"child height: {child.rect.height}, self height: {self.rect.height}");
        }
        _isUpdated = true;
    }

    public void OnScrolled(Vector2 scrolledPosition)
    {
        // Trigger the animation during scrolling  
        // Block animation before the content rect transform updates its height
        if(true)
        {
            // TriggerAnimationV1();
 
        }
        TriggerAnimationV2();
    }

    private void TriggerAnimationV1()
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
                    // Animator inside the children of the component
                    Animator animator = content.GetComponentInChildren<Animator>();
                    ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
                    // float scrollValue = content.GetComponent<RectTransform>().anchoredPosition.y / scrollRect.content.rect.height;
                    // Debug.Log($"scrollValue: {scrollValue}, current scroll value: {scrolledPosition.y}");
                    RectTransform rt = content.GetComponent<RectTransform>();
                    // Content's Height
                    float contentHeight = rt.rect.height;
                    // Calculated position y 
                    float ch = rt.position.y;
                    bool isThresholdHit = (ch >= (contentHeight * animationTriggerThreshold));
                    // Debug.Log($"name: {content.name}, calculated pos y: {ch}, normalized y: {ch / _viewportHeight}, content height: {contentHeight}, threshold: {contentHeight * animationTriggerThreshold}, triggered: {isThresholdHit}");
                    // Trigger animation
                    if (isThresholdHit) animator.SetBool("content_slide_in", true);

                    // animator.SetBool("content_slide_in", true);
                    // Debug.Log($"screenpos: {_camera.WorldToScreenPoint(content.transform.position)}");
                }
            }
            catch (Exception) { }
        }
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
                    // Calculated position y (deduct from viewportheight to get a 0 to maxheight from top to bottom)
                    float ch = reverseCalc ? _viewportHeight - MathF.Abs(rt.position.y) : rt.position.y;
                    bool isThresholdHit = (ch >= (contentHeight * animationTriggerThreshold));
                    Debug.Log($"name: {content.name}, calculated pos y: {ch}, normalized y: {ch / _viewportHeight}, content height: {contentHeight}, threshold: {contentHeight * animationTriggerThreshold}, triggered: {isThresholdHit}");
                    // Trigger animation
                    if (isThresholdHit) selfAnimator.SetBool("content_slide_in", true);

                    // animator.SetBool("content_slide_in", true);
                    // Debug.Log($"screenpos: {_camera.WorldToScreenPoint(content.transform.position)}");
                }
            }
            catch(Exception e) { }
        }
    }
}
