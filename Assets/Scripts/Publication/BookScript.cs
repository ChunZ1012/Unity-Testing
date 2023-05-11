using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BookScript : MonoBehaviour
{
    // Public variables (displayed on the inspector view)
    [SerializeField]
    public List<GameObject> bookList;
    public int limitPerShelf;
    public Transform inspirePublicationContainer;
    public Transform pastPublicationContainer;
    public Transform shelfBackground;
    // Prefabs to be instantiated
    public GameObject bookShelfPrefab;
    public GameObject bookPrefab;

    // Private variables
    private List<GameObject> bookShelves = new List<GameObject>();
    private Transform currentBookShelf;

    // For calculation of bookshelf width
    private float shelfSide = 100;
    private float spacing = 40;
    private float bookWidth = 150;

    // For fetching data from database
    public string baseUrl = "http://localhost:8080/api/publish";
    private List<PublicationModel> models;
    public Texture2D imageNotAvailableTexture;

    // Static variables (for sending PDF url to PublicationBook)
    public static string selectedPDFUrl;

    // For rebuilding
    public GameObject contentContainer;

    // Start is called before the first frame update
    void Start()
    {
        // Call RequestData() function to retrieve data from API
        RequestData();
    }

    void Update()
    {
        // To fix content size fitter collapsing issue
        LayoutRebuilder.MarkLayoutForRebuild(contentContainer.transform as RectTransform);
    }

    // Function to retrieve/fetch data from API
    private void RequestData()
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
                        models = JsonConvert.DeserializeObject<List<PublicationModel>>(resp.Data);
                        Debug.Log($"models' size: {models.Count}");
                        // Calls booksPopulator to populate books based on fetched data
                        // For inspire publication - category: "INSP"
                        booksPopulator(models, inspirePublicationContainer, "INSP");

                        // For past publication - category: "PAST"
                        booksPopulator(models, pastPublicationContainer, "PAST");
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
            Debug.Log($"{e.Message}\n{e.StackTrace}");
        }
    }

    // Function to populate books
    private void booksPopulator(List<PublicationModel> bookList, Transform publicationContainer, string category)
    {
        // Create the first bookShelf on load
        bookShelfCreator(publicationContainer);

        // limitCounter tracks the number of books on the currentBookShelf
        int limitCounter = 0;

        // Tracks the number of bookshelves on the category
        int bookShelfCount = 1;

        // Loops through bookList (to be replaced) to generate book object for each book
        for (int i = 0; i < bookList.Count; i++)
        {
            if (bookList[i].Category == category)
            {
                // Checks if books on the current shelf exceeds the limit
                if (limitCounter < limitPerShelf)
                {
                    // Calls bookCreator to create a book
                    bookCreator(bookList[i]);

                    // Adds limitCounter by 1
                    limitCounter += 1;
                }
                else
                {
                    // Calls bookShelfCreator to create a new book shelf
                    bookShelfCreator(publicationContainer);
                    // Reset limit counter (=1 as bookCreator is called after this)
                    limitCounter = 1;
                    // Adds bookShelfCount by 1
                    bookShelfCount += 1;
                    // Calls bookCreator to create book instance
                    bookCreator(bookList[i]);
                }

                // Get rectTransform of the bookshelf object
                Transform childTransform = currentBookShelf.transform.Find("Bookshelf");
                RectTransform bookShelfRectTransform = childTransform.GetComponent<RectTransform>();

                // Calls bookShelfWidthSizer() function to change the width of the bookshelf
                bookShelfWidthSizer(limitCounter, bookShelfRectTransform);
            }
        }
    }

    // Function to create bookshelf
    private void bookShelfCreator(Transform publicationContainer)
    {
        // Instantiate the bookShelfPrefab
        GameObject newBookShelf = Instantiate(bookShelfPrefab, publicationContainer.transform.Find("Background"));

        // Set parent
        // newBookShelf.transform.SetParent(publicationContainer.transform.Find("Background"));

        //newBookShelf.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        // Set bookshelf object name "bookShelf[ID]", id is the index in bookShelves array
        // bookShelves holds bookShelf objects of both Inspire and Past publications
        newBookShelf.name = "bookShelf" + bookShelves.Count;

        // Add gameObject to bookShelves List;
        bookShelves.Add(newBookShelf);

        // Set it as the currentBookShelf
        currentBookShelf = newBookShelf.transform;
    }

    // Function to change bookshelf width
    private void bookShelfWidthSizer(int bookCount, RectTransform bookShelfRect)
    {
        // Calculate width of the bookshelf according to the book count
        float newWidth = (shelfSide * 2) + (bookCount - 1) * spacing + bookCount * bookWidth;
        bookShelfRect.sizeDelta = new Vector2(newWidth, 25f);
    }

    // Function to create book
    private void bookCreator(PublicationModel bookModel)
    {
        // Instantiate the bookPrefab
        GameObject newBook = Instantiate(bookPrefab, currentBookShelf.transform.Find("BooksContainer"));

        // Change game object (book) name
        newBook.name = bookModel.Title;

        // newBook.GetComponent<RectTransform>().localScale = Vector3.one;

        // Adding book into bookList List
        bookList.Add(newBook);

        // Set currentBookShelf as the parent of the new book object
        // newBook.transform.SetParent(currentBookShelf.transform.Find("BooksContainer"));

        // Get image component of the book object
        Image imageComponent = newBook.GetComponent<Image>();

        // Call LoadImageFromUrl() function to load image (book cover) to the image component
        StartCoroutine(LoadImageFromUrl(bookModel.CoverUrl, imageComponent));

        // Get button component of the book object
        Button buttonComponent = newBook.GetComponent<Button>();

        // Add onClick listener for the button component
        buttonComponent.onClick.AddListener(() => {
            // Assing static variable "selectedPDFUrl" to be loaded in PublicationBook scene
            selectedPDFUrl = bookModel.PDFUrl;

            // Debug purposes
            Debug.Log($"Book named: {bookModel.Title} was clicked");

            // Call SceneLoader to change to PublicationBook scene
            SceneLoader.instance.LoadPage("PublicationBook");
        });
    }

    // Function to load image by URL passed
    public IEnumerator LoadImageFromUrl(string coverUrl, Image imageComponent)
    {
        yield return HttpManager.GetTexture((coverUrl), (req) =>
        {
            Sprite imgSprite;
            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(req);
                // Create sprite from texture
                imgSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                Debug.Log("Testing cover URL: " + coverUrl);
            }
            else
            {
                // Use alternative image if image is not available
                Debug.Log($"resp code: {req.responseCode}, result: {req.result}, url: {req.url}");
                // Create sprite from texture
                imgSprite = Sprite.Create(imageNotAvailableTexture, new Rect(0, 0, imageNotAvailableTexture.width, imageNotAvailableTexture.height), new Vector2(0.5f, 0.5f), 100f);
            }

            // Load cover image onto component
            imageComponent.sprite = imgSprite;
        });
    }
}
