using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsEventsListController : MonoBehaviour
{
    [Header("API Url")]
    public string baseUrl = "http://localhost:8080/api/content/list";
    public GameObject verticalContentContainer;
    [Header("Content Prefab")]
    public GameObject horizontalContentPrefab;
    public GameObject contentPrefab;
    public Texture2D imageNotAvailableTexture;
    [Header("Setting")]
    public float animateDuration;
    public LeanTweenType animateType;

    [Header("Debug")]
    public bool enableLogging = false;
    public bool disableImmediateRebuild = false;

    // Start is called before the first frame update
    void Start()
    {
        if(enableLogging) Debug.Log("ReqController started!");
        RequestData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Transform transform = verticalContentContainer.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                // child.SetParent(null);
                Destroy(child.gameObject);
            }

            RequestData();
        }
    }

    public void RequestData()
    {
        if (enableLogging) Debug.Log("Requesting data!");
        try
        {
            StartCoroutine(HttpManager.GetRequest(baseUrl, (req) =>
            {
                if (req.result == UnityWebRequest.Result.Success)
                {
                    HttpResponseModel resp = JsonConvert.DeserializeObject<HttpResponseModel>(req.downloadHandler.text);
                    if (enableLogging) Debug.Log($"error: {resp.Error}, msg: {resp.Msg}, data: {resp.Data}");

                    if (!resp.Error)
                    {
                        List<NewEventListModel> models = JsonConvert.DeserializeObject<List<NewEventListModel>>(resp.Data);
                        if (enableLogging) Debug.Log($"models' size: {models.Count}");
                        for (int i = 0; i < models.Count; i++)
                        {
                            NewEventListModel model = models[i];
                            CreateContentFromData(model, verticalContentContainer.transform, out Transform contentTransform);
                            contentTransform.name = string.Format("Content {0}", (i + 1));
                        }

                        if (!disableImmediateRebuild) StartCoroutine(RebuildLayout());
                    }
                    else
                    {
                        if (enableLogging) Debug.LogWarning($"Response has error! {resp.Msg}");
                    }
                }
                else
                {
                    if (enableLogging) Debug.Log($"Connection failed! {req.result}, {req.GetResponseHeader("")}");
                }
            }));
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{e.Message}\n{e.StackTrace}");
        }
    }
    private void CreateContentFromData(NewEventListModel model, Transform verticalContentPanel, out Transform contentTransform)
    {
        // Create content transform and set its parent
        contentTransform = Instantiate(contentPrefab, verticalContentPanel).transform;

        Button contentButton = contentTransform.GetComponentInChildren<Button>();
        contentButton.onClick.AddListener(() =>
        {
            NewEventDetailController.CONTENT_ID = model.Id;
            // TODO: Add transition animation
            // Call LoadPage() function in SceneLoader to load the NewsEventsDetailScene
            // By default, LoadPage uses the "closing" animation
            SceneLoader.instance.LoadPage("NewsEventsDetailScene");

            // Can use playerPrefs to pass model.id over to another scene
            // Note: PlayerPrefs are still saved even when exiting the game
            PlayerPrefs.SetInt("modelID", model.Id);

            if (enableLogging) Debug.Log($"model id: {model.Id}");
        });
        Transform contentPanel = contentTransform.GetChild(0);
        for (int x = 0; x < contentPanel.childCount; x++)
        {
            Transform contentChildTransform = contentPanel.GetChild(x);
            if (contentChildTransform.name.Equals("ContentImagePanel"))
            {
                // Fetch image and add to content
                StartCoroutine(LoadImageFromUrl(model.CoverUrl, contentChildTransform));
            }
            // Set title and description
            else if(contentChildTransform.name.Equals("TextContainer"))
            {
                CreateContentTItleAndDescription(contentChildTransform, model);
            }
        }
    }
    private void CreateContentTItleAndDescription(Transform contentChildTransform, NewEventListModel model)
    {
        Transform titleTransform = contentChildTransform.GetChild(0);
        titleTransform.GetComponent<TextMeshProUGUI>().text = model.Title;

        Transform descTransform = contentChildTransform.GetChild(1);
        descTransform.GetComponent<TextMeshProUGUI>().text = model.PublishedDate;
    }
    private IEnumerator LoadImageFromUrl(string url, Transform contentImagePanel)
    {
        yield return HttpManager.GetTexture(url, (req) =>
        {
            Transform contentImage = contentImagePanel.GetChild(0);
            Image imgObj = contentImage.GetComponent<Image>();

            Sprite imgSprite;
            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                // Create sprite from texture
                imgSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            }
            else
            {
                if (enableLogging) Debug.Log($"resp code: {req.responseCode}, result: {req.result}, url: {req.url}");
                // Create sprite from texture
                imgSprite = Sprite.Create(imageNotAvailableTexture, new Rect(0, 0, imageNotAvailableTexture.width, imageNotAvailableTexture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            imgObj.sprite = imgSprite;
            // Set lean animation to the content image
            contentImage.GetComponent<RectTransform>().LeanSize(new Vector2(350, 350), animateDuration).setEase(animateType);
        });
    }

    private IEnumerator RebuildLayout()
    {
        yield return new WaitForSeconds(1f);
        LayoutRebuilder.ForceRebuildLayoutImmediate(verticalContentContainer.transform as RectTransform);
        /*
        for(int i = 0; i < verticalContentContainer.transform.childCount; i++)
        {
            Transform contentContainer = verticalContentContainer.transform.GetChild(i);

            RectTransform contentRT = contentContainer.GetComponentInChildren<RectTransform>();
            BoxCollider bc = contentContainer.GetComponentInChildren<BoxCollider>();
            bc.size = new Vector3(contentRT.rect.width, contentRT.rect.height, bc.size.z);
            Debug.Log($"size: {contentRT.rect.width}, {contentRT.rect.height}");
        }
        */
    }
}