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

    private List<Texture2D> _imagesTexture = new List<Texture2D>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HttpManager.GetFile(BookScript.selectedPDFUrl, localPath, (req) =>
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success");
                // Calls LoadPdf() function to start loading the PDF only after it has done downloading the file
                // (This is to prevent sharing violation)
                LoadPdf();
            }
            else
            {
                Debug.Log($"Connection failed! {req.result}, {req.GetResponseHeader("")}");
            }
        }));
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
