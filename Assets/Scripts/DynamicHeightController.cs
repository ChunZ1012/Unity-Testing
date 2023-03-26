using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicHeightController : MonoBehaviour
{
    public bool isUpdated = false;
    // Start is called before the first frame update
    void Start()
    {
        UpdateRectTransform();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRectTransform();
    }

    private void UpdateRectTransform()
    {
        RectTransform self = GetComponent<RectTransform>();
        RectTransform child = self.GetChild(0).GetComponent<RectTransform>();

        self.sizeDelta = new Vector2(self.rect.width, child.rect.height);
        isUpdated = true;

        // Debug.Log($"child name: {child.name}, self name: {self.name}");
        // Debug.Log($"child height: {child.rect.height}, self height: {self.rect.height}");
    }
}
