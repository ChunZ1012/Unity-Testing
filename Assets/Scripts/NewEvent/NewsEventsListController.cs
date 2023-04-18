using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("ReqController started!");
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
        // Fix the content size fitter collapsing issue
        // See https://answers.unity.com/questions/1033789/panel-content-size-fitter-not-working.html
        LayoutRebuilder.MarkLayoutForRebuild(verticalContentContainer.transform as RectTransform);
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
                        List<NewEventListModel> models = JsonConvert.DeserializeObject<List<NewEventListModel>>(resp.Data);
                        Debug.Log($"models' size: {models.Count}");
                        for (int i = 0; i < models.Count; i++)
                        {
                            NewEventListModel model = models[i];
                            // Create horizontal content panel to hold the content, and set its parent
                            Transform horizontalContentPanel = Instantiate(horizontalContentPrefab, verticalContentContainer.transform).transform;
                            // Create left content object
                            CreateContentFromData(model, horizontalContentPanel, out Transform contentTransform);
                            // If the next counter has not reach the end of the list
                            // then add the right content to the horizontal panel
                            if (i + 1 < models.Count)
                            {
                                model = models[i + 1];
                                CreateContentFromData(model, horizontalContentPanel, out contentTransform);
                            }
                            else
                            {
                                for(int x = 0; x < contentTransform.GetChild(0).childCount; x++)
                                {
                                    Transform child = contentTransform.GetChild(0).GetChild(x);
                                    if(child.name == "TextContainer")
                                    {
                                        RectTransform childRt = child.GetChild(1).GetComponent<RectTransform>();
                                        Vector2 newSize = new Vector2(1450, childRt.sizeDelta.y);

                                        childRt.sizeDelta = newSize;
                                        child.GetChild(0).GetComponent<RectTransform>().sizeDelta = newSize;
                                    }
                                }
                            }
                            // increment;
                            i++;
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
    private void CreateContentFromData(NewEventListModel model, Transform horizontalContentPanel, out Transform contentTransform)
    {
        // Create content transform and set its parent
        contentTransform = Instantiate(contentPrefab, horizontalContentPanel).transform;
        Button contentButton = contentTransform.GetComponent<Button>();
        contentButton.onClick.AddListener(() =>
        {
            // TODO: Add transition animation
            // Call LoadPage() function in SceneLoader to load the NewsEventsDetailScene
            // By default, LoadPage uses the "closing" animation
            SceneLoader.instance.LoadPage("NewsEventsDetailScene");

            // Can use playerPrefs to pass model.id over to another scene
            // Note: PlayerPrefs are still saved even when exiting the game
            PlayerPrefs.SetInt("modelID", model.Id);

            Debug.Log($"model id: {model.Id}");
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
                Transform titleTransform = contentChildTransform.GetChild(0);
                titleTransform.GetComponent<TextMeshProUGUI>().text = model.Title;

                Transform descTransform = contentChildTransform.GetChild(1);
                descTransform.GetComponent<TextMeshProUGUI>().text = model.PublishedDate;

                // Rebuild layout, fixing the vertical layout group not working issue
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentChildTransform as RectTransform);
            }
        }
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
                Debug.Log($"resp code: {req.responseCode}, result: {req.result}, url: {req.url}");
                // Create sprite from texture
                imgSprite = Sprite.Create(imageNotAvailableTexture, new Rect(0, 0, imageNotAvailableTexture.width, imageNotAvailableTexture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            imgObj.sprite = imgSprite;
            // Set lean animation to the content image
            contentImage.GetComponent<RectTransform>().LeanSize(new Vector2(350, 350), animateDuration).setEase(animateType);
        });
    }
}