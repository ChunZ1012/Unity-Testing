using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Close popup panel when pressing close button
public class popupclose : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        //wait for button click and call TaskOnClick
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        //deactivate panel
        this.gameObject.SetActive(false);
    }
}