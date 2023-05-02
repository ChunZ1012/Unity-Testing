using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abutton : MonoBehaviour
{
    public ScrollAlphabet scrollAlphabet;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void move()
    {
        scrollAlphabet.MoveScroll(0.5f);
    }
}
