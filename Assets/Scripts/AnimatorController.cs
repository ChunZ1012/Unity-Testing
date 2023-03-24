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
    #endregion

    #region private declaration
    private List<GameObject> _contentList = new List<GameObject>();
    private float _layoutSpacing;
    private float _viewportHeight;
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

    }

    public void OnScrolled(Vector2 scrolledPosition)
    {
        // Trigger the animation during scrolling   
        TriggerAnimation();
    }

    private void TriggerAnimation()
    {
        for (int i = 0; i < _contentList.Count; i++)
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
                float ch = rt.position.y + rt.rect.height + _layoutSpacing;
                bool isThresholdHit = (ch >= (contentHeight * animationTriggerThreshold));
                Debug.Log($"calculated pos y: {ch}, normalized y: {ch / _viewportHeight}, content height: {contentHeight}, threshold: {contentHeight * animationTriggerThreshold}, triggered: {isThresholdHit}");
                // Trigger animation
                if (isThresholdHit) animator.SetBool("content_slide_in", true);

                // animator.SetBool("content_slide_in", true);
                // Debug.Log($"screenpos: {_camera.WorldToScreenPoint(content.transform.position)}");
            }
        }
    }
}
