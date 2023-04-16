using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewEventVerticalScrollSnap : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ScrollRect ImageScrollRect;
    public ScrollRect TextContainerScrollRect;
    public float SwipeThreshold = 50;
    public float SwipeTime = 0.5f;

    private int _content;
    private int _contentSize;
    private bool _drag;
    private float _dragTime;
    private bool _lerp;
    public void Initialization()
    {
        TextContainerScrollRect.verticalNormalizedPosition = 0;
        _contentSize = TextContainerScrollRect.content.childCount;

        enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_lerp || _drag) return;

        float verticalNormalizedPosition = _content / (_contentSize - 1);

        // ImageScrollRect.horizontalNormalizedPosition = Mathf.Lerp(ImageScrollRect.horizontalNormalizedPosition, (1 - verticalNormalizedPosition), 5 * Time.deltaTime);

    }

    private void Scroll(int index)
    {
        index = Math.Sign(index);

        if ((_content == 0 && index == -1) || (_content == _contentSize && index == 1)) return;

        _lerp = true;
        _content += index;
    }

    private int GetCurrentDisplayedContent()
    {
        return Mathf.RoundToInt(TextContainerScrollRect.verticalNormalizedPosition / (_contentSize - 1));
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        _drag = true;
        _dragTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // _content = GetCurrentDisplayedContent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var delta = eventData.pressPosition.x - eventData.position.x;

        if (Mathf.Abs(delta) > SwipeThreshold && Time.time - _dragTime < SwipeTime)
        {
            var direction = Math.Sign(delta);

            Scroll(direction);
        }
        _drag = false;
        _lerp = false;
    }
}
