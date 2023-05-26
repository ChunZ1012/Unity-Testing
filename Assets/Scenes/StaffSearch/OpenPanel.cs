using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

public class OpenPanel : MonoBehaviour
{
    public static StaffListModel model;

    public GameObject popup;
    public GameObject panel;
    public Texture2D imageNotAvailableTexture;

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
    public void ShowPanel()
    {
        panel.gameObject.SetActive(false);
        popup.gameObject.SetActive(true); //activate the popup
        //show staff detail inside popup
        TMPro.TextMeshProUGUI[] textMeshProList;
        textMeshProList = popup.GetComponentsInChildren<TextMeshProUGUI>();
        textMeshProList[0].text = "Name: " + model.Name;
        textMeshProList[1].text = "Contact: " + model.Contact;
        textMeshProList[2].text = "Email: " + model.Email;
        textMeshProList[3].text = "Position: " + model.Position;
        textMeshProList[4].text = "Location: " + model.Location;

        StartCoroutine(LoadImageFromUrl(model.Image, popup.transform));

        LayoutRebuilder.ForceRebuildLayoutImmediate(popup.transform as RectTransform);
    }

    private IEnumerator LoadImageFromUrl(string url, Transform popup)
    {
        yield return HttpManager.GetTexture(url, (req) =>
        {
            Image imgObj = popup.GetComponentsInChildren<Image>()
                .AsEnumerable()
                .Single(i => i.transform.name.ToLower() == "staffavatar");

            Sprite imgSprite;

            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                // Create sprite from texture
                imgSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            }
            else
            {
                // Create sprite from texture
                imgSprite = Sprite.Create(imageNotAvailableTexture, new Rect(0, 0, imageNotAvailableTexture.width, imageNotAvailableTexture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            imgObj.sprite = imgSprite;
        });
    }
}
