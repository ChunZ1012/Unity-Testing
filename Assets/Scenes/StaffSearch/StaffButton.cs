using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;



public class StaffButton : MonoBehaviour
{
    public GameObject scrollViewContent;
    public GameObject buttonStaff;
    public GameObject popup;
    public OpenPanel openPanel;

    [Header("API Url")]
    public string baseUrl = "http://localhost:8080/api/staff";

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("ReqController started!");       
        PassObjectToAnotherScript();
        RequestData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RequestData()
    {
        Debug.Log("Requesting data!");
        try
        {
            StartCoroutine(HttpManager.GetRequest(baseUrl, (req) =>
            {
                if (req.result == UnityWebRequest.Result.Success)
                {
                    HttpResponseModel resp = JsonConvert.DeserializeObject<HttpResponseModel>(req.downloadHandler.text);
                    Debug.Log($"error: {resp.Error}, msg: {resp.Msg}, data: {resp.Data}");

                    if (!resp.Error)
                    {
                        List<StaffListModel> models = JsonConvert.DeserializeObject<List<StaffListModel>>(resp.Data);
                        Debug.Log($"models' size: {models.Count}");
                        for (int i = 0; i < models.Count; i++)
                        {
                            StaffListModel model = models[i];

                            GameObject btn = (GameObject)Instantiate(buttonStaff);
                            btn.transform.SetParent(scrollViewContent.transform);
                            btn.GetComponentInChildren<TextMeshProUGUI>().text = model.Name;
                            btn.GetComponent<OpenPanel>().image = model.Image;
                            btn.GetComponent<OpenPanel>().id = model.Id;
                            btn.GetComponent<OpenPanel>().name = model.Name;
                            btn.GetComponent<OpenPanel>().contact = model.Contact;
                            btn.GetComponent<OpenPanel>().email = model.Email;
                            btn.GetComponent<OpenPanel>().position = model.Position;
                            btn.GetComponent<OpenPanel>().location = model.Location;
                        }


                    }
                    else
                    {
                        Debug.LogWarning($"Response has error! {resp.Msg}");
                    }
                }
                else
                {
                    Debug.Log($"Connection failed! {req.result}, {req.GetResponseHeader("")}");
                }
            }));
        }       

        catch (Exception e)
        {
            Debug.LogWarning($"{e.Message}\n{e.StackTrace}");
        }
    }

    void PassObjectToAnotherScript()
    {
        //Code to pass the object to another C# script
        openPanel.PassedGameObject = popup;
    }
}
