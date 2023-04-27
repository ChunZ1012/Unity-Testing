using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PublicationLoader : MonoBehaviour
{
    public BookRaw Book;
    private string localPath = @"./Assets/TempPDF"; // By default, the temp PDF file created is stored in a folder named "TempPDF" in Assets folder
    // Was:
    //public string destPath = "http://localhost:8080/assets/uploads/pubs/1682561455_5b78198f0623772164f4.pdf";
    // To be tested: Use static variable named: "selectedPDFUrl" of type string; Declared in BookScript.cs

    private List<Texture2D> _imagesTexture = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HttpManager.GetFile(BookScript.selectedPDFUrl, localPath, (req) =>
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success");
            }
            else
            {
                Debug.Log($"Connection failed! {req.result}, {req.GetResponseHeader("")}");
            }
        }));
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

    // Destroy created temp PDF file on gameEnd
    private void OnDestroy()
    {
        PublicationPDFConverter.RemoveAllPDFImages(localPath);
    }

    private void LoadPdf()
    {
        if (Book == null)
            throw new Exception("The Book object is null!");
        // Load texture from the pdf
        _imagesTexture = PublicationPDFConverter.ConvertPDFToTextures(localPath + @"/temp.pdf");
        // Remove all preloaded textures from Book
        Book.RemoveBookTextures();
        // Add all pdf textures into the Book
        foreach (Texture2D texture in _imagesTexture) Book.AddBookTexture(texture);
        // Re-initialize the Book by calling its Start function
        Book.Start();
    }
}
