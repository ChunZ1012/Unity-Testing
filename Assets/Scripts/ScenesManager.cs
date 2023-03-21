using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesManager : MonoBehaviour
{
    public Button myButton;
    public bool myLoadNextScene;

    private TextMeshProUGUI _tmp;
    // Start is called before the first frame update
    void Awake()
    {
        _tmp = myButton.transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // This function is called when the behaviour becomes disabled or inactive
    private void OnDisable()
    {
        PlayerPrefs.SetString("btn_text", _tmp.text);
    }

    // This function is called when the object becomes enabled and active
    private void OnEnable()
    {
        _tmp.text = PlayerPrefs.GetString("btn_text", _tmp.text);
    }

    public void LoadNextScene()
    {
        int totalScenes = SceneManager.sceneCountInBuildSettings;
       
        if(SceneManager.GetActiveScene().buildIndex + 1 >= totalScenes)
        {
            Debug.Log("This is the last scene!");
        }
        else
        {
            _tmp.text = "Some testing text here";
            if(myLoadNextScene) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            Debug.Log("Loaded next scene");
        }
    }
}
