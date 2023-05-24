using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine;

public class NewEventDetailController : MonoBehaviour
{
    public static int CONTENT_ID = 14;
    [Header("API Url")]
    public string baseUrl = "http://localhost:8080/api/content/{0}";
    [Header("Content Setting")]
    public GameObject slider;
    public GameObject contentTextContainer;
    [Header("Content Prefab")]
    public GameObject contentTextPrefab;
    public GameObject contentTitlePrefab;
    public GameObject contentPublishTimePrefab;
    [Header("Misc")]
    public bool enableLogging = false;

    private Assets.SimpleSlider.Scripts.Slider _sliderScript;
    private void Awake()
    {
        _sliderScript = slider.GetComponent<Assets.SimpleSlider.Scripts.Slider>();
        // Clear content on awake
        ClearContent();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(enableLogging) Debug.Log("Detail Req Controller started!");
        // Start requesting data
        RequestData();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ClearContent();
            // Start request for data
            RequestData();
        }
    }

    private void RequestData()
    {
        string url = string.Format(baseUrl, CONTENT_ID);
        if (enableLogging) Debug.Log($"Requesting data! Url: {url}");

        try
        {
            // Start request
            StartCoroutine(HttpManager.GetRequest(url, (req) =>
            {
                HttpResponseModel resp = JsonConvert.DeserializeObject<HttpResponseModel>(req.downloadHandler.text);
                // Debug.Log($"error: {resp.Error}, msg: {resp.Msg}, data: {resp.Data}");

                if(!resp.Error)
                {
                    NewEventDetailModel model = JsonConvert.DeserializeObject<NewEventDetailModel>(resp.Data);
                    CreateTitleAndPublishTime(model);
                    // TODO: loop through each image object in model, pass the url to the slider and create respective content to text scroll view
                    for(int x = 0; x < model.Images.Count; x++)
                    {
                        NewEventImageModel imageModel = model.Images[x];
                        CreateBanner(imageModel);
                        // Create text content that sit under text scoll view
                        CreateTextContent(imageModel.ImageContent);
                    }
                    // Alert script to load images from url
                    // Use StartCoroutine to trigger the method as it return IEnumerator
                    StartCoroutine(_sliderScript.Start());
                    if (!string.IsNullOrEmpty(model.Content))
                    {
                        // Create text content that sit under text scoll view
                        CreateTextContent(model.Content, true);
                    }
                }
                else
                {
                    if (enableLogging) Debug.LogWarning($"Response return an error! msg: {resp.Msg}");
                }
            }));
        }
        catch(Exception e)
        {
            Debug.LogWarning($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void CreateTitleAndPublishTime(NewEventDetailModel model)
    {
        // Add title into the text scroll view
        GameObject titleGO = Instantiate(contentTitlePrefab, contentTextContainer.transform);
        titleGO.GetComponent<TextMeshProUGUI>().text = model.Title;
        // Add publish time GO into the text scroll view
        GameObject timeGO = Instantiate(contentPublishTimePrefab, contentTextContainer.transform);
        timeGO.GetComponent<TextMeshProUGUI>().text = string.Format(timeGO.GetComponent<TextMeshProUGUI>().text, model.PublishTime.ToString("dd MMM, yyyy"));
    }
    private void CreateBanner(NewEventImageModel imageModel)
    {
        Assets.SimpleSlider.Scripts.Banner banner = new Assets.SimpleSlider.Scripts.Banner();
        banner.Name = imageModel.ImageAltText;
        banner.Url = imageModel.ImageUrl;
        // Add banner to the slider
        _sliderScript.Banners.Add(banner);
    }
    private void CreateTextContent(string unparsedTextContent, bool isUnlinkedTextContent = false)
    {
        GameObject textContentGO = Instantiate(contentTextPrefab, contentTextContainer.transform);
        TextMeshProUGUI tmp = textContentGO.GetComponent<TextMeshProUGUI>();
        tmp.text = ParseHtml(unparsedTextContent);
        if(isUnlinkedTextContent)
        {
            // Set to unlinked text content, as the default prefab has a linked tag associated with it
            textContentGO.tag = "NewEventDetailTextContent";
        }
    }
    private void ClearContent()
    {
        Transform t = contentTextContainer.transform;
        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            // Remove all text content before requesting
            Destroy(child.gameObject);
        }
        // Remove all banners
        _sliderScript.Banners.Clear();
    }

    private string ParseHtml(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        HtmlNode node = doc.DocumentNode.SelectSingleNode("//div");
        return node != null ? node.InnerText : string.Empty;
    }
}
