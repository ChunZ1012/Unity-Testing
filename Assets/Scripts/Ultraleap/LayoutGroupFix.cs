using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutGroupFix : MonoBehaviour
{
    public GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LayoutRebuilder.MarkLayoutForRebuild(go.transform as RectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(go.transform as RectTransform);
        Canvas.ForceUpdateCanvases();

        //go.GetComponentInChildren<HorizontalLayoutGroup>().enabled = false;
        //go.GetComponentInChildren<HorizontalLayoutGroup>().enabled = true;
    }
}
