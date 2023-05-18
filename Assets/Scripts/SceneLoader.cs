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

    // Animators
    public Animator transition;
    public Animator alertAnim;
    public Animator instructionAnim;

    // Default transitionTime
    public float transitionTime = 1f;

    private string firstPage;
    private string lastPage;
    private bool isSubPage = false;

    private bool firstTrigger = true;
    private string currentSceneName;
    private string currentScenePath;
    private string currentSceneCategory;
    private string prevScene;

    private List<string> allSceneNames = new List<string>();
    string[] closingTransScenes = { "mainmenu", "submenuaboutus", "submenuwhatwedo", "", "newseventslistscene", "newseventsdetailsscene", "contactus", "stafflogin", "staffdetails" };
    string[] loadingTransScenes = { "publicationpage", "publicationbook" };

    void Start()
    {
        // Assign value to currenSceneName
        currentSceneName = SceneManager.GetActiveScene().name;

        // Assign value to currentScenePath
        currentScenePath = SceneManager.GetActiveScene().path;

        // Reset PlayerPrefs ("PreviousScene") when on MainMenu
        if (currentSceneName.ToLower() == "mainmenu")
        {
            PlayerPrefs.SetString("PreviousScene", "");
        }
        // Assign value to prevScene
        prevScene = PlayerPrefs.GetString("PreviousScene");

        // Get subfolder from path (category)
        string pattern = @"^.*\/([^\/]+)\/.*$";
        Match matchRegex = Regex.Match(currentScenePath, pattern);
        if (matchRegex.Success)
        {
            currentSceneCategory = matchRegex.Groups[1].Value;
            if (currentSceneCategory.ToLower() == "about us")
            {
                firstPage = "AboutUsScene";
                lastPage = "IntegrityNoticeScene";
                isSubPage = true;
            }
            else if (currentSceneCategory.ToLower() == "whatwedo")
            {
                firstPage = "WhatWeDoMain";
                lastPage = "TrainingScene";
                isSubPage = true;
            }
        }
        
        // Calls triggerEndAnimation function
        triggerEndAnimation();

        // Trigger show instructions on start
        StartCoroutine(TriggerInstructions());

        // Get the list of all scene names
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            allSceneNames.Add(sceneName);
        }
    }
    public void LoadMainMenu()
    {
        if (currentSceneName.ToLower() != "mainmenu")
        {
            StartCoroutine(PageLoader("MainMenu", "TriggerClosing", transitionTime));
        }
    }
    public void LoadNext()
    {
        // Only allows users to swipe left or right when the scene is one of the sub-pages
        if (isSubPage)
        {
            if (currentSceneName != lastPage)
            {
                string nextSceneName = getNextSceneNameByIndex(1);
                PlayerPrefs.SetString("SwipeMethod", "Left");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeLeft", transitionTime));
            }
            else
            {
                //Alert user on last page
                alertAnim.SetTrigger("TriggerAlertLast");
            }
        }
    }
    public void LoadPrev()
    {
        // Only allows users to swipe left or right when the scene is one of the sub-pages
        if (isSubPage)
        {
            if (currentSceneName != firstPage)
            {
                string nextSceneName = getNextSceneNameByIndex(-1);
                PlayerPrefs.SetString("SwipeMethod", "Right");
                StartCoroutine(PageLoader(nextSceneName, "TriggerSwipeRight", transitionTime));
            }
            else
            {
                //Alert user on first page
                alertAnim.SetTrigger("TriggerAlertFirst");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Loads Main Menu scene when the user press on backspace key
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadMainMenu();
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

    // Trigger end animations based on the previous scene
    private void triggerEndAnimation()
    {
        // If the current scene is included inside loadingTransScenes array, trigger "Loading" animation (LoadingAnimation_End)
        if (loadingTransScenes.Contains(currentSceneName.ToLower()))
        {
            transition.SetTrigger("Loading");
        }
        // Else, if the previous scene is included inside closingTransScenes array, trigger "Closing" animation (ClosingAnimation_End)
        else if (closingTransScenes.Contains(prevScene.ToLower()))
        {
            transition.SetTrigger("Closing");
        }
        // Else, it would be from one of the sub-pages, hence will trigger the Swipe<Left/Right> animation (Swipe<Left/Right>_End)
        else
        {
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
    }

    // Trigger animation for instruction panel
    // Currently set to 5f seconds interval for the first load; 10f seconds interval for the rest
    // Animation duration is 10f seconds
    IEnumerator TriggerInstructions()
    {
        if (firstTrigger)
        {
            yield return new WaitForSeconds(5f);
            firstTrigger = !firstTrigger;
        }
        else
        {
            yield return new WaitForSeconds(10f);
        }
        
        // Triggers the animation
        instructionAnim.SetTrigger("TriggerInstructions");

        yield return new WaitForSeconds(10f);

        // Self call
        StartCoroutine(TriggerInstructions());
    }

    // Gets the name of the next scene in build index based on swipe direction
    private string getNextSceneNameByIndex(int indexChange)
    {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        string sPath = SceneUtility.GetScenePathByBuildIndex(currentSceneBuildIndex + indexChange);
        string sName = System.IO.Path.GetFileNameWithoutExtension(sPath);
        return sName;
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
                if (loadingTransScenes.Contains(pageName.ToLower()))
                {
                    StartCoroutine(PageLoader(pageName, "TriggerLoading", 2f));
                }
                else
                {
                    StartCoroutine(PageLoader(pageName, "TriggerClosing", transitionTime));
                }
            } else {
                Debug.Log("Page name passed doesn't match/exist in Assets/Scenes. Possible fix: Ensure pageName passed is equal to the scene name (case sensitive)");
            }
        }
        
    }

    // Function used to call SceneManager to change scene
    IEnumerator PageLoader(string pageName, string triggerMethod, float transTime)
    {
        PlayerPrefs.SetString("PreviousScene", currentSceneName);

        transition.SetTrigger(triggerMethod);

        yield return new WaitForSeconds(transTime);

        SceneManager.LoadSceneAsync(pageName);
    }
}
