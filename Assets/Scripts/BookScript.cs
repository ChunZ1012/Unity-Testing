using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class BookScript : MonoBehaviour
{
    [SerializeField]
    public Transform bookContainer;
    public GameObject[] bookList;
    public RectTransform bookshelf;
    public GameObject bookPrefab;

    // Testing purpose (To be replaced with Books fetched from Database)
    public string[] bookNames = { "Mobile App", "IoT", "FinTech", "Big Data" };

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate prefab
        for (int i = 0; i < bookNames.Length; i++)
        {
            GameObject newBook = Instantiate(bookPrefab);

            // Change game object (book) name
            newBook.name = bookNames[i];

            // Adding book into bookList array
            int currentLength = bookList.Length;
            Array.Resize(ref bookList, currentLength + 1);
            bookList[currentLength] = newBook;

            // Set bookContainer as the parent of the new book object
            newBook.transform.SetParent(bookContainer);

            //TODO: Change book cover image here
            Image imageComponent = newBook.GetComponent<Image>();
            // Uncomment the code below to change sprite/book cover
                // imageComponent.sprite = bookCoverSprite

            //TODO: OnClick change to PublicationBook scene

            // Do any additional configuration of the new game object here.
        }

        // Change the width of the bookshelf according to the number of books
        float shelfSide = 100;
        float spacing = 40;
        float bookWidth = 150;
        // Calculate width
        float newWidth = (shelfSide * 2) + (bookNames.Length - 1) * spacing + bookNames.Length * bookWidth;
        // Change width
        bookshelf.sizeDelta = new Vector2(newWidth, 25f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
