using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private void Awake()
    {
        instance = this;
    }

    public Animator transition;
    public float transitionTime = 1f;
    private List<string> sceneNames = new List<string>();


    void Start()
    {
        // Get the list of all scene names
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            sceneNames.Add(sceneName);

            // Print out each scene names
            Debug.Log("Scene name: " + sceneName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            this.LoadMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log(currentSceneName);
        if (currentSceneName.ToLower() != "mainmenu") {
            Debug.Log("Loading Main Menu");
            StartCoroutine(PageLoader("MainMenu", "Start"));
        }
    }

    public void LoadPage(string pageName)
    {
        // Check if pageName/assignedPage on button is not empty
        if (pageName == "") {
            Debug.Log("Page name passed is empty. Possible fix: Add values to assigned page variable on button");
        }
        else {
            bool matchName = false;
            // Check if pageName exists in scenes (Note: Only scenes inside Scene folder will be checked)
            for (int i = 0; i < sceneNames.Count; i++) {
                if (sceneNames[i] == pageName) {
                    matchName = true;
                    break;
                }
            }

            // Starts to load the corresponding page with transitions if pageName exists
            if (matchName) {
                Debug.Log($"Scene to be loaded: { pageName }");
                StartCoroutine(PageLoader(pageName, "Start"));
            } else {
                Debug.Log("Page name passed doesn't match/exist in Assets/Scenes. Possible fix: Ensure pageName passed is equal to the scene name (case sensitive)");
            }
        }
        
    }

    IEnumerator PageLoader(string pageName, string triggerMethod)
    {
        transition.SetTrigger(triggerMethod);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(pageName);
    }
}
