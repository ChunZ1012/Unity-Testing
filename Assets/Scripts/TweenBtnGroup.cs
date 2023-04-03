using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenBtnGroup : MonoBehaviour
{
    [SerializeField] GameObject buttonGroup;
    public float animateDuration;

    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveY(buttonGroup, -500f, animateDuration).setEase(LeanTweenType.easeOutCubic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
