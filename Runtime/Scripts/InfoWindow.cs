using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    [Tooltip("Minimum width of the info window (pixels). Never shrinks below this (e.g. 350 for 3 inputs + 3 outputs).")]
    public float minWidth = 350f;
    [Tooltip("Width of each side (input/output area) when showing up to 3 items. Window expands symmetrically if more items need more space.")]
    public float baseSideWidth = 140f;
    [Tooltip("Width of the center graphic (arrow) between input and output areas (pixels).")]
    public float centerGraphicWidth = 50f;
    [Tooltip("Horizontal padding on each side of the content area (pixels).")]
    public float horizontalPadding = 10f;

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

    private static float ComputeGridRowWidth(GridLayoutGroup grid, int count)
    {
        if (grid == null || count <= 0) return 0f;
        return count * grid.cellSize.x
             + Mathf.Max(0, count - 1) * grid.spacing.x
             + grid.padding.left + grid.padding.right;
    }

    public void InitializeResources(List<Resource> produces, List<Resource> consumes)
    {
        if (inputResource == null || outputResource == null || inputPanel == null || outputPanel == null) return;

        for (int i = inputPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(inputPanel.transform.GetChild(i).gameObject);
        for (int i = outputPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(outputPanel.transform.GetChild(i).gameObject);

        var inputGrid = inputPanel.GetComponent<GridLayoutGroup>();
        if (inputGrid != null)
        {
            inputGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            inputGrid.constraintCount = 1;
        }
        var outputGrid = outputPanel.GetComponent<GridLayoutGroup>();
        if (outputGrid != null)
        {
            outputGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
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

        ResizeToFitContent(consumes.Count, produces.Count, inputGrid, outputGrid);
    }

    private void ResizeToFitContent(int inputCount, int outputCount,
        GridLayoutGroup inputGrid, GridLayoutGroup outputGrid)
    {
        // Required width per side from grid (n cells + gaps + padding) + panel inset
        var inputPanelRT = inputPanel.GetComponent<RectTransform>();
        float inputPanelInset = inputPanelRT != null ? Mathf.Abs(inputPanelRT.sizeDelta.x) : 0f;
        var outputPanelRT = outputPanel.GetComponent<RectTransform>();
        float outputPanelInset = outputPanelRT != null ? Mathf.Abs(outputPanelRT.sizeDelta.x) : 0f;

        float inputRequired = inputCount > 0 ? ComputeGridRowWidth(inputGrid, inputCount) + inputPanelInset : 0f;
        float outputRequired = outputCount > 0 ? ComputeGridRowWidth(outputGrid, outputCount) + outputPanelInset : 0f;

        // Symmetric: both sides use the same width (never shrink below baseSideWidth; expand if either side needs more)
        float perSideWidth = Mathf.Max(baseSideWidth, inputRequired, outputRequired);

        // Total width: padding + left side + center + right side + padding (never shrink below minWidth)
        float totalWidth = horizontalPadding + perSideWidth + centerGraphicWidth + perSideWidth + horizontalPadding;
        totalWidth = Mathf.Max(minWidth, totalWidth);

        var root = GetComponent<RectTransform>();
        if (root != null)
            root.sizeDelta = new Vector2(totalWidth, root.sizeDelta.y);

        // Position the three zones left-to-right (left-anchored, center-y)
        float cursor = horizontalPadding;

        var inputsRT = inputPanel.transform.parent as RectTransform;
        if (inputsRT != null)
        {
            inputsRT.anchorMin = new Vector2(0f, 0.5f);
            inputsRT.anchorMax = new Vector2(0f, 0.5f);
            inputsRT.sizeDelta = new Vector2(perSideWidth, inputsRT.sizeDelta.y);
            inputsRT.anchoredPosition = new Vector2(cursor + perSideWidth / 2f, inputsRT.anchoredPosition.y);
        }
        cursor += perSideWidth;

        RectTransform arrowRT = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Arrow")
            {
                arrowRT = transform.GetChild(i) as RectTransform;
                break;
            }
        }
        if (arrowRT != null)
        {
            arrowRT.anchorMin = new Vector2(0f, 0.5f);
            arrowRT.anchorMax = new Vector2(0f, 0.5f);
            arrowRT.anchoredPosition = new Vector2(cursor + centerGraphicWidth / 2f, arrowRT.anchoredPosition.y);
        }
        cursor += centerGraphicWidth;

        var outputsRT = outputPanel.transform.parent as RectTransform;
        if (outputsRT != null)
        {
            outputsRT.anchorMin = new Vector2(0f, 0.5f);
            outputsRT.anchorMax = new Vector2(0f, 0.5f);
            outputsRT.sizeDelta = new Vector2(perSideWidth, outputsRT.sizeDelta.y);
            outputsRT.anchoredPosition = new Vector2(cursor + perSideWidth / 2f, outputsRT.anchoredPosition.y);
        }
    }
}
