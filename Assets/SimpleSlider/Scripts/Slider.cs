using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.SimpleSlider.Scripts
{
	/// <summary>
	/// Creates banners and paginator by given banner list.
	/// </summary>
	public class Slider : MonoBehaviour
	{
		[Header("Settings")]
		public List<Banner> Banners;
		public bool Elastic = true;

		[Header("UI")]
		public Transform BannerGrid;
		public Button BannerPrefab;
		public Transform PaginationGrid;
		public Toggle PagePrefab;
		public HorizontalScrollSnap HorizontalScrollSnap;

		[Header("Animation")]
		public LeanTweenType AnimationType;
		public float AnimationDuration = 0.5f;

		public void OnValidate()
		{
			GetComponent<ScrollRect>().content.GetComponent<GridLayoutGroup>().cellSize = GetComponent<RectTransform>().sizeDelta;
		}

		public IEnumerator Start()
		{
			// Remove all children before start requesting
			RemoveAllChildren();
			foreach (Banner banner in Banners)
			{
				var instance = Instantiate(BannerPrefab, BannerGrid);
				var button = instance.GetComponent<Button>();

				button.onClick.RemoveAllListeners();

				if (string.IsNullOrEmpty(banner.Url))
				{
					button.enabled = false;
					instance.GetComponent<Image>().sprite = banner.Sprite;
				}
				else
				{
					// button.onClick.AddListener(() => { Application.OpenURL(banner.Url); });
					StartCoroutine(HttpManager.GetTexture(banner.Url, (req) =>
					{
						// Set not_available image as default texture
						Texture2D texture = Resources.Load("Images/not_available") as Texture2D;
						// Get the texture object if the web request if success
						if (req.result == UnityWebRequest.Result.Success) texture = DownloadHandlerTexture.GetContent(req);
						// Assign the texture to the banner
						instance.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0f, 0f), 100f);
						instance.GetComponent<Image>().preserveAspect = true;

						if (string.IsNullOrEmpty(banner.Name))
						{
							// Disable banner text background if the alt text is empty
							instance.transform.GetChild(0).gameObject.SetActive(false);
						}
						else
						{
							// Get the banner text transform
							Transform bannerTextTransform = instance.transform.GetChild(0).GetChild(0);
							// Set banner text
							bannerTextTransform.GetComponent<TextMeshProUGUI>().text = banner.Name;
						}

						RectTransform rect = instance.GetComponent<RectTransform>();
						rect.LeanSize(rect.sizeDelta, AnimationDuration).setEase(AnimationType);
					}));
				}


				if (Banners.Count > 1)
				{
					var toggle = Instantiate(PagePrefab, PaginationGrid);

					toggle.group = PaginationGrid.GetComponent<ToggleGroup>();
				}
			}

			yield return null;

			HorizontalScrollSnap.Initialize(Banners.Count);
			HorizontalScrollSnap.GetComponent<ScrollRect>().movementType = Elastic ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
		}
		public void RemoveAllChildren()
		{
			foreach (Transform child in BannerGrid)
			{
				Destroy(child.gameObject);
			}

			foreach (Transform child in PaginationGrid)
			{
				Destroy(child.gameObject);
			}
		}
	}
}