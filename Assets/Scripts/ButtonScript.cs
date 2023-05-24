using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ButtonScript : MonoBehaviour
{
    //Declaring variables
    //Public
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public float scaleTime;
    public Color hoverColor;
    public string assignedPage;

    //Private
    private Button button;
    private Shadow btnShadow;
    private Outline btnOutline;
    private Color originalColor;
    private ColorBlock colorB;
    private TMP_Text text;

    // Start is called before the first frame update
    void Start()
    {
        // Outline and Shadow are both referred to as "Shadow". Hence by running GetComponents<Shadow> once will only get the first <Shadow> component.
        // Since both Shadow and Outline components are added to the button gameObject, loop below is used to get both Shadow and Outline components and assign it to their respective variable.
        foreach (var c in GetComponents<Shadow>())
        {
            Outline o = c as Outline;
            if (o == null) // component is not an Outline
                btnShadow = c;
            else
                btnOutline = o;
        }

        // Get button component
        button = GetComponent<Button>();

        // Get ChildTextMeshPro component
        text = GetComponentInChildren<TMP_Text>();

        // Get the colorBlock of the button object
        colorB = button.colors;

        // Retrieve the original color of the button and store it inside the originalColor variable
        originalColor = colorB.selectedColor;
    }

    // Function called when mouse/pointer enters the button
    public void onMouseEnter() {
        // Scale the button with animation using LeanTween
        LeanTween.scale(this.gameObject, new Vector3(scaleX, scaleY, scaleZ), scaleTime).setEase(LeanTweenType.easeOutCirc);

        colorB.selectedColor = hoverColor;
        button.colors = colorB;

        if (text != null)
        {
            // Change TMP Text color to white (on hover)
            text.color = Color.white;
        }

        if(btnOutline != null)
        {
            // Remove outline effectDistance (on hover)
            btnOutline.effectDistance = new Vector2(0, 0);
        }

        if(btnShadow != null)
        {
            // Increase shadow effectDistance (on hover)
            btnShadow.effectDistance = new Vector2(8, -8);
        }
    }

    //Function called when mouse/pointer exits the button
    public void onMouseExit()
    {
        try
        {
            // Scale the button back to normal size with animation using LeanTween
            LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), scaleTime).setEase(LeanTweenType.easeOutCirc);

            colorB.selectedColor = originalColor;
            button.colors = colorB;

            if(btnOutline != null)
            {
                // Set outline effectDistance back to (5, 5)
                btnOutline.effectDistance = new Vector2(5, 5);
            }

            if(btnShadow != null)
            {
                // Remove shadow effectDistance
                btnShadow.effectDistance = new Vector2(0, 0);
            }

            if(text != null)
            {
                // Change TMP Text color back to its original color
                text.color = Color.black;
            }
        }
        catch(Exception e)
        {
            Debug.LogError($"{e.Message}, {e.StackTrace}");
        }
    }
    public void onClick()
    {
        Debug.Log(SceneLoader.instance == null);
        if (assignedPage != null && !string.IsNullOrEmpty(assignedPage))
        {
            SceneLoader.instance.LoadPage(assignedPage);
        }
    }
}
