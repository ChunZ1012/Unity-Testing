using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Linq;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    private void Awake()
    {
        instance = this;
    }

    private enum Direction {
        left,
        right
    }

    public Animator transition;
    public float transitionTime = 1f;
    private string currentSceneName;
    private List<string> allSceneNames = new List<string>();
    private List<string> AboutUsSceneNames = new List<string>();
    private List<string> WhatWeDoSceneNames = new List<string>();

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        // Reset PlayerPrefs ("PreviousScene") when on MainMenu
        if (currentSceneName.ToLower() == "mainmenu") {
            PlayerPrefs.SetString("PreviousScene", "");
        }
        // Check the previous scene
        string prevScene = PlayerPrefs.GetString("PreviousScene");
        Debug.Log($"Name of last scene: { prevScene }");
        // If it is from one of the scenes stated in tempSceneNames, trigger the closing animation
        string[] tempSceneNames = { "mainmenu", "submenuaboutus", "submenuwhatwedo", "", "newseventslistscene", "newseventsdetailsscene", "contactus"};
        if (tempSceneNames.Contains(prevScene.ToLower()))
        {
            transition.SetTrigger("Closing");
        }

        else {
            // Check the swipe method
            string SwipeMethod = PlayerPrefs.GetString("SwipeMethod");
            if (SwipeMethod == "Right")
            {
                transition.SetTrigger("SwipeRight");
            }
            else if (SwipeMethod == "Left")
            {
                transition.SetTrigger("SwipeLeft");
            }
        }

        // Get the list of all scene names
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log(scenePath);

            string pattern = @"^[^\/]+\/[^\/]+\/([^\/]+)\/.*$"; // Matches the text between two slashes

            Regex regex = new Regex(pattern);
            Match match = regex.Match(scenePath);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (match.Success)
            {
                string folder = match.Groups[1].Value;
                //Debug.Log($"Parent Folder: {folder}");
                if (folder == "About Us") {
                    AboutUsSceneNames.Add(sceneName);
                }
                if (folder == "WhatWeDo" || folder == "WhatWeDo2") {
                    WhatWeDoSceneNames.Add(sceneName);
                }
            }
            allSceneNames.Add(sceneName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            this.LoadMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadNext();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadPrev();
        }
    }
    public void LoadPrev()
    {
        //Check if page is able to swipe left
        if (AboutUsSceneNames.Contains(currentSceneName))
        {
            if (currentSceneName != "AboutUsScene")
            {
                //Run Transition and change scene by build index
                string nextSceneName = getNextSceneNameByIndex(Direction.left);
                PlayerPrefs.SetString("SwipeMethod", "Right");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeRight"));
            }
            else
            {
                //Alert user on first page
                Debug.Log("Already on the first page");
            }
        }
        else if (WhatWeDoSceneNames.Contains(currentSceneName))
        {
            if (currentSceneName != "WhatWeDoMain")
            {
                //Run Transition and change scene by build index
                string nextSceneName = getNextSceneNameByIndex(Direction.left);
                PlayerPrefs.SetString("SwipeMethod", "Right");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeRight"));
            }
            else
            {
                //Alert user on first page
                Debug.Log("Already on the first page");
            }
        }
    }
    public void LoadNext()
    {
        //Check if page is able to swipe right
        if (AboutUsSceneNames.Contains(currentSceneName))
        {
            if (currentSceneName != "IntegrityNoticeScene")
            {
                //Run Transition and change scene by build index
                string nextSceneName = getNextSceneNameByIndex(Direction.right);
                PlayerPrefs.SetString("SwipeMethod", "Left");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeLeft"));
            }
            else
            {
                //Alert user on last page
                Debug.Log("Already on the last page");
            }
        }
        else if (WhatWeDoSceneNames.Contains(currentSceneName))
        {
            if (currentSceneName != "TrainingScene")
            {
                //Run Transition and change scene by build index
                string nextSceneName = getNextSceneNameByIndex(Direction.right);
                PlayerPrefs.SetString("SwipeMethod", "Left");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeLeft"));
            }
            else
            {
                //Alert user on last page
                Debug.Log("Already on the last page");
            }
        }
    }

    public void LoadMainMenu()
    {
        Debug.Log(currentSceneName);
        if (currentSceneName.ToLower() != "mainmenu") {
            Debug.Log("Loading Main Menu");
            StartCoroutine(PageLoader("MainMenu", "TriggerClosing"));
        }
    }

    // Called by buttons inside main menu and sub menus
    public void LoadPage(string pageName)
    {
        // Check if pageName/assignedPage on button is not empty
        if (pageName == "") {
            Debug.Log("Page name passed is empty. Possible fix: Add values to assigned page variable on button");
        }
        else {
            bool matchName = false;
            // Check if pageName exists in scenes (Note: Only scenes inside Scene folder will be checked)
            for (int i = 0; i < allSceneNames.Count; i++) {
                if (allSceneNames[i] == pageName) {
                    matchName = true;
                    break;
                }
            }

            // Starts to load the corresponding page with transitions if pageName exists
            if (matchName) {
                Debug.Log($"Scene to be loaded: { pageName }");
                StartCoroutine(PageLoader(pageName, "TriggerClosing"));
            } else {
                Debug.Log("Page name passed doesn't match/exist in Assets/Scenes. Possible fix: Ensure pageName passed is equal to the scene name (case sensitive)");
            }
        }
        
    }

    private string getNextSceneNameByIndex(Direction direction) {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        if (direction == Direction.left)
        {
            string sPath = SceneUtility.GetScenePathByBuildIndex(currentSceneBuildIndex - 1);
            string sName = System.IO.Path.GetFileNameWithoutExtension(sPath);
            return sName;
        }
        else
        {
            string sPath = SceneUtility.GetScenePathByBuildIndex(currentSceneBuildIndex + 1);
            string sName = System.IO.Path.GetFileNameWithoutExtension(sPath);
            return sName;
        }
    }

    IEnumerator PageLoader(string pageName, string triggerMethod)
    {
        PlayerPrefs.SetString("PreviousScene", currentSceneName);

        transition.SetTrigger(triggerMethod);

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadSceneAsync(pageName);
    }
}
