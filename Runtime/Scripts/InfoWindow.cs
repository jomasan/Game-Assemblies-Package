using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    [Tooltip("Minimum width of the info window (pixels).")]
    public float minWidth = 200f;
    [Tooltip("Width of the arrow/gap between input and output areas (pixels).")]
    public float arrowWidth = 42f;
    [Tooltip("Horizontal padding (each side) for the content area (pixels).")]
    public float horizontalPadding = 20f;

    public Image ownerSprite;
    public GameObject inputPanel;
    public GameObject outputPanel;

    public GameObject inputResource;
    public GameObject outputResource;

    private static Color GetIconTint(Resource r)
    {
        if (r == null) return Color.white;
        return r.iconTint.a < 0.01f ? Color.white : r.iconTint;
    }

    public void InitializeResources(List<Resource> produces, List<Resource> consumes)
    {
        if (inputResource == null || outputResource == null || inputPanel == null || outputPanel == null) return;

        // Clear existing icons so re-initialize doesn't duplicate
        for (int i = inputPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(inputPanel.transform.GetChild(i).gameObject);
        for (int i = outputPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(outputPanel.transform.GetChild(i).gameObject);

        // Use single row so icons lay out horizontally; we'll resize width to fit
        var inputGrid = inputPanel.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        if (inputGrid != null)
        {
            inputGrid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;
            inputGrid.constraintCount = 1;
        }
        var outputGrid = outputPanel.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        if (outputGrid != null)
        {
            outputGrid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;
            outputGrid.constraintCount = 1;
        }

        foreach (Resource c in consumes)
        {
            GameObject inputObjects = Instantiate(inputResource);
            inputObjects.transform.SetParent(inputPanel.transform, false);
            var inputImage = inputObjects.GetComponentInChildren<Image>(true);
            if (inputImage != null)
            {
                if (c.icon != null) inputImage.sprite = c.icon;
                inputImage.color = GetIconTint(c);
            }
        }

        foreach (Resource p in produces)
        {
            GameObject outputObjects = Instantiate(outputResource);
            outputObjects.transform.SetParent(outputPanel.transform, false);
            var outputImage = outputObjects.GetComponentInChildren<Image>(true);
            if (outputImage != null)
            {
                if (p.icon != null) outputImage.sprite = p.icon;
                outputImage.color = GetIconTint(p);
            }
        }

        // Scale window width so all icons fit; use input/output cell sizes and spacing from layout
        float inputCellW = inputGrid != null ? inputGrid.cellSize.x + inputGrid.spacing.x : 44f;
        float outputCellW = outputGrid != null ? outputGrid.cellSize.x + outputGrid.spacing.x : 64f;
        float inputContentWidth = consumes.Count > 0 ? consumes.Count * inputCellW - (inputGrid != null ? inputGrid.spacing.x : 4f) : 0f;
        float outputContentWidth = produces.Count > 0 ? produces.Count * outputCellW - (outputGrid != null ? outputGrid.spacing.x : 4f) : 0f;
        float totalWidth = horizontalPadding + inputContentWidth + arrowWidth + outputContentWidth + horizontalPadding;
        totalWidth = Mathf.Max(minWidth, totalWidth);

        var root = GetComponent<RectTransform>();
        if (root != null)
            root.sizeDelta = new Vector2(totalWidth, root.sizeDelta.y);
    }
}
