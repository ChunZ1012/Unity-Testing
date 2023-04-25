using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class OpenPanel : MonoBehaviour
{
    public string image;
    public int id;
    public string name;
    public string contact;
    public string email;
    public string position;
    public string location;
    public GameObject popup;

    public GameObject PassedGameObject
    {
        get => popup;
        set
        {
            popup = value;
            Debug.Log($"Receiver[{name}] just received \'{popup.name}\'");
        }
    }
    // Start is called before the first frame update
    void Awake()
    {

    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePanel() {
        popup.gameObject.SetActive(true);
        TMPro.TextMeshProUGUI[] textMeshProList;
        textMeshProList = popup.GetComponentsInChildren<TextMeshProUGUI>();
        textMeshProList[0].text = "Name: " + name;
        textMeshProList[1].text = "Contact: " + contact;
        textMeshProList[2].text = "Email: " + email;
        textMeshProList[3].text = "Position: " + position;
        textMeshProList[4].text = "Location: " + location;
        Debug.Log("yes: " + id + name + contact);
    }


}
