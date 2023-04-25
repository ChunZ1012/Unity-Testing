using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupFix : MonoBehaviour
{
    public GameObject buttonGroup;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LayoutRebuilder.MarkLayoutForRebuild(buttonGroup.transform as RectTransform);
    }
}
