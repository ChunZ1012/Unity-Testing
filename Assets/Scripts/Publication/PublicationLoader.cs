using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicationLoader : MonoBehaviour
{
    public BookRaw Book;
    public string destPath = @"D:\Swinburne\ICT30001\PDF\";

    private List<Texture2D> _imagesTexture = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        LoadPdf();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed");
            LoadPdf();
        }
    }

    private void OnDestroy()
    {
        PublicationPDFConverter.RemoveAllPDFImages(destPath);
    }

    private void LoadPdf()
    {
        if (Book == null)
            throw new Exception("The Book object is null!");
        // Load texture from the pdf
        _imagesTexture = PublicationPDFConverter.ConvertPDFToTextures(@"D:\Swinburne\ICT30001\sample.pdf", destPath);
        // Remove all preloaded textures from Book
        Book.RemoveBookTextures();
        // Add all pdf textures into the Book
        foreach (Texture2D texture in _imagesTexture) Book.AddBookTexture(texture);
        // Re-initialize the Book by calling its Start function
        Book.Start();
    }
}
