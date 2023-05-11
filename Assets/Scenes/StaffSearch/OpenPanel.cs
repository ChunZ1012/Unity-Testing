using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;

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

    public void CreatePanel() {
        popup.gameObject.SetActive(true);
        TMPro.TextMeshProUGUI[] textMeshProList;
        textMeshProList = popup.GetComponentsInChildren<TextMeshProUGUI>();
        textMeshProList[0].text = "Name: " + name;
        textMeshProList[1].text = "Contact: " + contact;
        textMeshProList[2].text = "Email: " + email;
        textMeshProList[3].text = "Position: " + position;
        textMeshProList[4].text = "Location: " + location;

        StartCoroutine(LoadImageFromUrl(image, popup.transform));

        Debug.Log("yes: " + id + name + contact);
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
