using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenScript : MonoBehaviour
{
    [SerializeField] GameObject sainsLogo, buttonGroup;
    public float posY;
    public float animateDuration;
    // Start is called before the first frame update
    void Start()
    {
        LeanTween.moveY(sainsLogo, posY, animateDuration).setEase(LeanTweenType.easeOutCubic);
        LeanTween.moveY(buttonGroup, -500f, animateDuration).setEase(LeanTweenType.easeOutCubic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
