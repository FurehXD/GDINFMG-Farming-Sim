using UnityEngine;

public class AdminPanelManager : MonoBehaviour
{
    public GameObject cropPanel;
    public GameObject qualityPanel;
    public GameObject rarityPanel;
    public GameObject plotPanel;
    public GameObject itemPanel;

    [Header("Hotkeys")]
    public KeyCode cropPanelKey = KeyCode.F1;
    public KeyCode qualityPanelKey = KeyCode.F2;
    public KeyCode rarityPanelKey = KeyCode.F3;
    public KeyCode plotPanelKey = KeyCode.F4;
    public KeyCode itemPanelKey = KeyCode.F5;
    public KeyCode closeAllPanelsKey = KeyCode.Escape;

    [Header("Settings")]
    public bool allowMultiplePanels = false;

    private void Start()
    {
        DisableAllPanels();
    }

    private void Update()
    {
        if (Input.GetKeyDown(cropPanelKey))
        {
            TogglePanel(cropPanel);
        }
        if (Input.GetKeyDown(qualityPanelKey))
        {
            TogglePanel(qualityPanel);
        }
        if (Input.GetKeyDown(rarityPanelKey))
        {
            TogglePanel(rarityPanel);
        }
        if (Input.GetKeyDown(plotPanelKey))
        {
            TogglePanel(plotPanel);
        }
        if (Input.GetKeyDown(itemPanelKey))
        {
            TogglePanel(itemPanel);
        }
        if (Input.GetKeyDown(closeAllPanelsKey))
        {
            DisableAllPanels();
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        bool newState = !panel.activeSelf;

        if (!allowMultiplePanels && newState)
        {
            DisableAllPanels();
        }

        panel.SetActive(newState);

        if (newState)
        {
            panel.transform.SetAsLastSibling();

            var basePanel = panel.GetComponent<BaseAdminPanel>();
            if (basePanel != null)
            {
                basePanel.enabled = true;
            }
        }
    }

    public void DisableAllPanels()
    {
        if (cropPanel != null) cropPanel.SetActive(false);
        if (qualityPanel != null) qualityPanel.SetActive(false);
        if (rarityPanel != null) rarityPanel.SetActive(false);
        if (plotPanel != null) plotPanel.SetActive(false);
        if (itemPanel != null) itemPanel.SetActive(false);
    }

    // Public methods for external control
    public void ShowCropPanel() => TogglePanel(cropPanel);
    public void ShowQualityPanel() => TogglePanel(qualityPanel);
    public void ShowRarityPanel() => TogglePanel(rarityPanel);
    public void ShowPlotPanel() => TogglePanel(plotPanel);
    public void ShowItemPanel() => TogglePanel(itemPanel);

    public bool IsAnyPanelOpen()
    {
        return (cropPanel != null && cropPanel.activeSelf) ||
               (qualityPanel != null && qualityPanel.activeSelf) ||
               (rarityPanel != null && rarityPanel.activeSelf) ||
               (plotPanel != null && plotPanel.activeSelf) ||
               (itemPanel != null && itemPanel.activeSelf);
    }

    public GameObject GetActivePanel()
    {
        if (cropPanel != null && cropPanel.activeSelf) return cropPanel;
        if (qualityPanel != null && qualityPanel.activeSelf) return qualityPanel;
        if (rarityPanel != null && rarityPanel.activeSelf) return rarityPanel;
        if (plotPanel != null && plotPanel.activeSelf) return plotPanel;
        if (itemPanel != null && itemPanel.activeSelf) return itemPanel;
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (cropPanelKey == qualityPanelKey ||
            cropPanelKey == rarityPanelKey ||
            cropPanelKey == plotPanelKey ||
            cropPanelKey == itemPanelKey ||
            qualityPanelKey == rarityPanelKey ||
            qualityPanelKey == plotPanelKey ||
            qualityPanelKey == itemPanelKey ||
            rarityPanelKey == plotPanelKey ||
            rarityPanelKey == itemPanelKey ||
            plotPanelKey == itemPanelKey)
        {
            Debug.LogWarning("AdminPanelManager: Hotkeys should be unique for each panel!");
        }
    }
#endif
}