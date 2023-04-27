using PdfiumViewer;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;

public class PublicationPDFConverter : MonoBehaviour
{
    public static List<Texture2D> ConvertPDFToTextures(string path)
    {
        List<Texture2D> imagesTexture = new List<Texture2D>();
        // Check if the pdf file is existed
        if (File.Exists(path))
        {
            using PdfDocument doc = PdfDocument.Load(path);
            // Debug
            Debug.Log($"page count: {doc.PageCount}");
            // Loop through each page
            for (int i = 1; i <= doc.PageCount; i++)
            {
                // Render the page out
                using Image image = doc.Render(i - 1, 2400, 2400, PdfRenderFlags.ForPrinting | PdfRenderFlags.LcdText);
                using MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                byte[] pdfBuffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(pdfBuffer, 0, pdfBuffer.Length);

                Texture2D pdfTexture = new Texture2D(image.Width, image.Height);
                pdfTexture.LoadImage(pdfBuffer);
                pdfTexture.name = string.Format("Page {0}", i);

                imagesTexture.Add(pdfTexture);
            }
        }
        // Log warning if not exist
        else Debug.LogWarning($"The file at {path} is not exist!");
        // Return list
        return imagesTexture;
    }

    public static void RemoveAllPDFImages(string dirPath)
    {
        if (Directory.Exists(dirPath))
        {
            // Get all available image under the directory
            string[] pdfFileImages = Directory.GetFiles(dirPath);

            for(int i = 0; i < pdfFileImages.Length; i++)
            {
                // Delete pdf images
                File.Delete(pdfFileImages[i]);
            }
        }
        else Debug.LogWarning($"The directory {dirPath} is not exist! Perhaps typo?");
    }
}
