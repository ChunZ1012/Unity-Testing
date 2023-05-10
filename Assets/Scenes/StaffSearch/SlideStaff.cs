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
    private int op = 1;
    private float floattemp;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //calculate the position of the alphabet from 0-1
    public void Count()
    {
        int res = Passedlist.AsQueryable().Sum();
        for (int x = 0; x < Passedlist.Count; x++)
        {
            float pos = (float)Passedlist[x] / (float)res;
            if (op == 1)
            {
                floatAlphabet.Add(pos);
                floattemp = pos;
                op = 2;
            }
            else
            {
                floattemp = pos + floattemp;
                floatAlphabet.Add(floattemp);
            }
            index.Add(x);
        }
    }
    //move the scroll when slider is moved
    public void moveDrag()
    {
        float currentPosition = slide.value;
        scrollAlphabet.MoveScroll(1f - currentPosition);
    }
    //move slider according to scrolling
    public void MoveSlider(float floatnum)
    {
        slide.value = floatnum;
        float currentPosition = slide.value;
        for (int i = 0; i < floatAlphabet.Count; i++)
        {
            if (currentPosition <= floatAlphabet[i])
            {
                alphabetText.text = Passedlist2[index[i]];
                break;
            }
        }
    }
}
