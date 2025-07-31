using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CubeDisplay : MonoBehaviour
{

    [Header("Reference")]
    [SerializeField] GameObject tile2DPrefab;

    int size = 4;
    public int Size
    {
        get => size;
        set
        {
            if (value < 3) value = 3;
            size = value;
            UpdateDisplay();
        }
    }

    GridLayoutGroup layout;
    RectTransform rectTransform;
    void Awake()
    {
        layout = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
        GetComponent<Image>().enabled = false;
        StartCoroutine(UpdateDisplayWithDelay());
    }

    IEnumerator UpdateDisplayWithDelay()
    {
        yield return new WaitForEndOfFrame();
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float gap = 0.1f;
        float fixedGap = rectTransform.rect.width / Size * gap;
        float width = (rectTransform.rect.width - (Size - 1) * fixedGap) / Size;

        // Set layout properties
        layout.constraintCount = Size;
        layout.cellSize = new Vector2(width, width);
        layout.spacing = new Vector2(fixedGap, fixedGap);
        // Instantiate new tiles
        for (int i = 0; i < Size * Size; i++)
        {
            GameObject tile = Instantiate(tile2DPrefab, transform);
            tile.name = $"Tile {i + 1}";
        }
    }
}
