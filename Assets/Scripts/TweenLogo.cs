using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenLogo : MonoBehaviour
{
    [SerializeField] GameObject sainsLogo;
    public float posY;
    public float animateDuration;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveY(sainsLogo, posY, animateDuration).setEase(LeanTweenType.easeOutCubic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
