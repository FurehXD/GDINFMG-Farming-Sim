using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoScaleGridLayout : MonoBehaviour
{
    private GridLayoutGroup gridLayout;
    private RectTransform rectTransform;

    [Tooltip("Aspect ratio of the child items (width / height)")]
    public float childAspectRatio = 1.0f; // Default to square items
    private Vector2 spacing;

    private void Start()
    {
        spacing = GetComponent<GridLayoutGroup>().spacing;
        gridLayout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Set initial spacing in the GridLayoutGroup
        gridLayout.spacing = spacing;

        AdjustCellSize();
    }

    private void AdjustCellSize()
    {
        // Parent dimensions
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Children count
        int childCount = transform.childCount;

        if (childCount == 0) return;

        // Calculate dynamic rows and columns
        float aspectRatio = parentWidth / parentHeight;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(childCount * aspectRatio));
        int rows = Mathf.CeilToInt((float)childCount / columns);

        // Calculate available space per cell
        float totalSpacingX = (columns - 1) * spacing.x + gridLayout.padding.left + gridLayout.padding.right;
        float totalSpacingY = (rows - 1) * spacing.y + gridLayout.padding.top + gridLayout.padding.bottom;

        float availableWidth = (parentWidth - totalSpacingX) / columns;
        float availableHeight = (parentHeight - totalSpacingY) / rows;

        // Preserve aspect ratio
        if (availableWidth / availableHeight > childAspectRatio)
            availableWidth = availableHeight * childAspectRatio;
        else
            availableHeight = availableWidth / childAspectRatio;

        // Apply cell size and spacing
        gridLayout.cellSize = new Vector2(availableWidth, availableHeight);
        gridLayout.spacing = spacing; // Ensure spacing is explicitly applied
    }


    private void Update()
    {
        // Recalculate if layout changes during runtime
        AdjustCellSize();
    }
}
