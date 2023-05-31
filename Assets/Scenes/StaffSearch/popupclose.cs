using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Close popup scrollVIew when pressing close button
public class popupclose : MonoBehaviour
{
    public Button button;
    public Image StaffImage;
    public GameObject panel;
    // Start is called before the first frame update

    private void OnEnable()
    {
        //wait for button click and call TaskOnClick
        // Close button
        button.onClick.AddListener(TaskOnClick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    void TaskOnClick()
    {
        StaffImage.sprite = null;
        Debug.Log("Close button clicked!");

        this.panel.SetActive(true);
        //deactivate scrollVIew
        this.gameObject.SetActive(false);
        // StartCoroutine(ShowPanel());
    } 

    private IEnumerator ShowPanel()
    {
        yield return new WaitForSeconds(0.5f);
    }
}
