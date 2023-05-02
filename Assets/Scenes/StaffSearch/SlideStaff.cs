using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlideStaff : MonoBehaviour
{
    public Slider slide;
    private string alphabetstr;
    private int z;
    public ScrollAlphabet scrollAlphabet;
    public TextMeshProUGUI alphabetText;
    public List<int> Passedlist;
    public List<string> Passedlist2;
    public List<int> index = new List<int>();
    private List<float> floatAlphabet = new List<float>();
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
            
    }



    public void moveDrag()
    {
        float currentPosition = slide.value;
        scrollAlphabet.MoveScroll(1f - currentPosition);
    }

    public void MoveSlider(float floatnum)
    {
        slide.value = floatnum;
        float currentPosition = slide.value;
        
    }
}
