using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class popupclose : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        this.gameObject.SetActive(false);
    }
}
