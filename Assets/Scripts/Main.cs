using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject myButtonPrefab;
    public GameObject myMainPanel;
    public int myButtonCountToBeGenerated;
    public uint myColumnCount;

    private Transform myMainContentPanel;
    private List<ButtonData> _buttonData = new List<ButtonData>();
    private readonly string _baseUrl = "http://localhost:8080/api/";

    private void Awake()
    {
        var vp = myMainPanel.transform.Find("Viewport");
        myMainContentPanel = vp.Find("MainContent");

        var gridLayout = myMainContentPanel.GetComponent<GridLayoutGroup>();
        Debug.Log(Screen.width);
        decimal cellWidth = (decimal)(Screen.width / myColumnCount);
        gridLayout.cellSize = new Vector2((float)Math.Floor(cellWidth), gridLayout.cellSize.y);

        AddButtonToPanel(myButtonCountToBeGenerated);

        /*
        Debug.Log($"GameObject Null? : {myGameObject == null}");
        Debug.Log($"GameObject Name: {myGameObject.name}");

        var panels = GameObject.FindGameObjectsWithTag("ButtonPanel");

        Debug.Log($"btnPanel Null? : {panels == null}");
        foreach(var obj in panels)
        {
            Debug.Log(obj.name);
        }
        Debug.Log("\n");
        var btns = GameObject.FindGameObjectsWithTag("Button");
        foreach(var obj in btns)
        {
            Debug.Log(obj.name);
        }
        Debug.Log($"Awake Log: {Log}");
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log($"Child count: {myMainPanel.transform.childCount}");
        for(int i = 0; i < myMainPanel.transform.childCount; i++)
        {
            // Get button panel
            var child = myMainPanel.transform.GetChild(i).gameObject;
            // Get button object
            Button btn = child.transform.GetComponentInChildren<Button>();
            // Debug.Log(btn.name);
        }
        */
    }

    // Update is called once per frame (time may vary, use fixedupdate for fixed time)
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.I))
        {
            AddButtonToPanel(1);
        }
        else if(Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.C))
        {
            RemoveButtonFromPanel();
        }
        /// 0 -> Left Click,
        /// 1 -> Right Click,
        /// 2 -> Middle Click
        else if(Input.GetMouseButtonDown(1))
        {
            if(SceneManager.GetActiveScene().buildIndex - 1 < 0)
            {
                Debug.Log("This is the first scene!");
            }
            else
            {
                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex - 1);
                Debug.Log("Loaded previous scene");
            }
        }
    }

    private void AddButtonToPanel(int numberOfButtonsToBeAdded)
    {
        for (int i = 0; i < numberOfButtonsToBeAdded; i++)
        {
            int id = (_buttonData.Count > 0 ? _buttonData[^1].Id : 0) + 1;
            var btnData = new ButtonData(id, "Placeholder " + id, "Url " + id);
            _buttonData.Add(btnData);
        }

        for(int i = 0; i < myMainContentPanel.childCount; i++)
        {
            var child = myMainContentPanel.GetChild(i).transform;
            RemoveChildFromParent(child);
        }

        // Reattach the button;
        for (int i = 0; i < _buttonData.Count; i++)
        {
            var btnData = _buttonData[i];
            var btnPnObj = Instantiate(myButtonPrefab);
            var rectTrans = btnPnObj.GetComponent<RectTransform>();

            rectTrans.pivot = Vector2.zero;
            btnPnObj.transform.SetParent(myMainContentPanel, false);
            rectTrans.position = new Vector3(rectTrans.position.x, 0, rectTrans.position.z);

            btnPnObj.name = btnPnObj.name.Replace("(Clone)", "") + " " + btnData.Id;
            var btn = btnPnObj.transform.GetComponentInChildren<Button>();
            // Set button name
            btn.name = btnData.Name;
            // Set button text
            btn.transform.GetComponentInChildren<TextMeshProUGUI>().text = btnData.Name;

            // Debug.Log($"button {i + 1}, x: {btnPnObj.transform.localPosition.x}, y: {btnPnObj.transform.localPosition.y}, z: {btnPnObj.transform.localPosition.z}");
        }
    }
    private void RemoveButtonFromPanel()
    {
        int childCount = myMainContentPanel.childCount;
        if(childCount > 0)
        {
            var child = myMainContentPanel.GetChild(childCount - 1);
            // Remove child from parent
            RemoveChildFromParent(child);
        }
    }
    private void RemoveChildFromParent(Transform t)
    {
        // Detach from parent
        t.SetParent(null);
        // Delete from scene
        Destroy(t.gameObject);
    }

    private void FindChildAndGrandChild()
    {
        // #####################################
        // The hierarchy will be
        // Canvas
        //  -> myMainPanel
        //   -> Viewport
        //    -> MainContent (Grandchild of myMainPanel)
        // #####################################
        var vp = myMainPanel.transform.Find("Viewport");
        var mc = vp.Find("MainContent");
        Debug.Log($"Content GO: {mc.name}, {mc.tag}");
    }
}
