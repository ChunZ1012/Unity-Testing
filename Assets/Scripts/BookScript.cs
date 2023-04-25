using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class BookScript : MonoBehaviour
{
    // Public variables (displayed on the inspector view)
    [SerializeField]
    public List<GameObject> bookList;
    public int limitPerShelf;
    public Transform inspirePublicationContainer;
    public Transform pastPublicationContainer;

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

    // Testing purpose (To be replaced with Books fetched from Database)
    private string[] bookNamesInspire = { "Mobile App", "IoT", "FinTech", "Big Data", "WebApp", "MySQL", "Backend", "123", "456", "789"};
    private string[] bookNamesPast = { "Mobile App", "IoT", "FinTech", "Big Data", "WebApp", "MySQL", "Backend", "123", "456", "789" };

    // Start is called before the first frame update
    void Start()
    {
        booksPopulator(bookNamesInspire, inspirePublicationContainer);
        booksPopulator(bookNamesPast, pastPublicationContainer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void booksPopulator(string[] bookList, Transform publicationContainer)
    {
        // Create the first bookShelf on load
        bookShelfCreator(publicationContainer);
        // limitCounter tracks the number of books on the currentBookShelf
        int limitCounter = 0;

        // Loops through bookList (to be replaced) to generate book object for each book
        for (int i = 0; i < bookList.Length; i++)
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
                // Calls bookCreator to create book instance
                bookCreator(bookList[i]);
            }

            // Change the width of the bookshelf according to the number of books
            // Calculate width
            float newWidth = (shelfSide * 2) + (limitCounter - 1) * spacing + limitCounter * bookWidth;

            // Update current bookshelf width
            Transform childTransform = currentBookShelf.transform.Find("Bookshelf");
            RectTransform bookShelfRectTransform = childTransform.GetComponent<RectTransform>();
            bookShelfRectTransform.sizeDelta = new Vector2(newWidth, 25f);
        }
    }

    // Function to create bookshelf
    private void bookShelfCreator(Transform publicationContainer)
    {
        // Instantiate the bookShelfPrefab
        GameObject newBookShelf = Instantiate(bookShelfPrefab);

        // Set parent
        newBookShelf.transform.SetParent(publicationContainer.transform.Find("Background"));

        // Set bookshelf object name "bookShelf[ID]", id is the index in bookShelves array
        // bookShelves holds bookShelf objects of both Inspire and Past publications
        newBookShelf.name = "bookShelf" + bookShelves.Count;

        // Add gameObject to bookShelves List;
        bookShelves.Add(newBookShelf);

        // Set it as the currentBookShelf
        currentBookShelf = newBookShelf.transform;
    }

    // Function to create book
    private void bookCreator(string bookName /*Image parameter to be passed here*/)
    {
        // Instantiate the bookPrefab
        GameObject newBook = Instantiate(bookPrefab);

        // Change game object (book) name
        newBook.name = bookName;

        // Adding book into bookList List
        bookList.Add(newBook);

        // Set currentBookShelf as the parent of the new book object
        newBook.transform.SetParent(currentBookShelf.transform.Find("BooksContainer"));

        //TODO: Change book cover image here
        // Get image component of the book object
        Image imageComponent = newBook.GetComponent<Image>();
        // Uncomment the code below to change sprite/book cover
        // imageComponent.sprite = bookCoverSprite

        // Get button component of the book object
        Button buttonComponent = newBook.GetComponent<Button>();

        // Add onClick listener for the button
        buttonComponent.onClick.AddListener(() => {
            // Code to execute when the button is clicked
            //TODO: Set static variable to tell PublicationBook scene which book content to load
            //TODO: Call SceneLoader to change to PublicationBook scene
            Debug.Log($"Book named: {bookName} was clicked");
        });

        // Do any additional configuration of the new book object here.
    }
}
