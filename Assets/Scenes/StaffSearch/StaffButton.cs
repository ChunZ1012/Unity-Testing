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
    public SlideStaff slideStaff;
    public Texture2D imageNotAvailableTexture;

    private int tempnumber;
    private int zcount;
    private int tempindex;
    private string tempstring;
    private int z;
    public List<int> alphabet = new List<int>();
    private List<string> alphabets = new List<string>() {"A", "B", "C", "D", "E", "F", "G", "H", "I",
        "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    [Header("API Url")]
    public string baseUrl = "http://localhost:8080/api/staff";

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("ReqController started!");       
        PassObjectToAnotherScript();
        RequestData();
        z = 0;
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
                        models.Sort(delegate (StaffListModel x, StaffListModel y) {
                            return x.Name.CompareTo(y.Name);
                        }); //sort staff list alphabetically
                        for (int i = 0; i < models.Count; i++)
                        {
                            StaffListModel model = models[i];
                            GameObject btn = (GameObject)Instantiate(buttonStaff);
                            btn.transform.SetParent(scrollViewContent.transform);
                            btn.GetComponentInChildren<TextMeshProUGUI>().text = model.Name;
                            //send staff detail to display in staff detail popup
                            btn.GetComponent<OpenPanel>().image = model.Image;
                            btn.GetComponent<OpenPanel>().id = model.Id;
                            btn.GetComponent<OpenPanel>().name = model.Name;
                            btn.GetComponent<OpenPanel>().contact = model.Contact;
                            btn.GetComponent<OpenPanel>().email = model.Email;
                            btn.GetComponent<OpenPanel>().position = model.Position;
                            btn.GetComponent<OpenPanel>().location = model.Location;
                            btn.GetComponent<OpenPanel>().imageNotAvailableTexture = imageNotAvailableTexture;

                            string str = model.Name;
                            string cutstr = str.Substring(0, 1);
                            //calculate how many of each alphabet there is
                            while (z < 25)
                            {
                                if (cutstr == alphabets[z])
                                {
                                    tempnumber += 1;
                                    break;
                                }
                                else
                                {
                                    alphabet.Add(tempnumber);
                                    tempnumber = 0;
                                    z++;
                                }
                            }
                            if (cutstr == "Z")
                            {
                                zcount++;
                            }                                
                        }
                        //pass the data to SlideStaff class
                        alphabet.Add(zcount);
                        slideStaff.Passedlist = alphabet;
                        slideStaff.Passedlist2 = alphabets;
                        slideStaff.Count();
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
        //Code to pass the object to OpenPanel class
        openPanel.PassedGameObject = popup;
        
    }
}
