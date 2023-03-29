using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();    
        }
    }

    public void LoadNextScene() 
    {
        StartCoroutine(LoadPage(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadPage(int pageIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(pageIndex);
    }
}
