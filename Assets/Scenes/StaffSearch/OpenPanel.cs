using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPanel : MonoBehaviour
{
    public int id;
    public string name;
    public string contact;
    public string email;
    public string position;
    public string location;
    public GameObject[] popup;
    public int op = 1;

    // Start is called before the first frame update
    void Awake()
    {
        popup = GameObject.FindGameObjectsWithTag("POPUP");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreatePanel() {
        Debug.Log("yes: " + id + name + contact);
        popup[0].SetActive(false);
        op = 1 + op;
        if (op == 3) { popup[0].SetActive(true); }
    }


}
