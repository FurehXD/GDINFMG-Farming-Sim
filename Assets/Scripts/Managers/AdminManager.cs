using UnityEngine;

public class AdminPanelManager : MonoBehaviour
{
    public GameObject cropPanel;
    public GameObject qualityPanel;
    public GameObject rarityPanel;
    public GameObject plotPanel;

    // Optional: Add key configuration
    [Header("Hotkeys")]
    public KeyCode cropPanelKey = KeyCode.F1;
    public KeyCode qualityPanelKey = KeyCode.F2;
    public KeyCode rarityPanelKey = KeyCode.F3;
    public KeyCode plotPanelKey = KeyCode.F4;
    public KeyCode closeAllPanelsKey = KeyCode.Escape;

    [Header("Settings")]
    public bool allowMultiplePanels = false; // If true, multiple panels can be open at once

    private void Start()
    {
        // Initially disable all panels
        DisableAllPanels();
    }

    private void Update()
    {
        // Handle panel toggles
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
        if (Input.GetKeyDown(closeAllPanelsKey))
        {
            DisableAllPanels();
        }
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        bool newState = !panel.activeSelf;

        // If we're not allowing multiple panels and we're trying to enable a panel
        if (!allowMultiplePanels && newState)
        {
            DisableAllPanels();
        }

        panel.SetActive(newState);

        // If the panel is being enabled, ensure it's properly initialized
        if (newState)
        {
            // Bring the panel's transform to the top of the hierarchy
            panel.transform.SetAsLastSibling();

            // Trigger any refresh logic the panel might have
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
    }

    // Public methods for external control
    public void ShowCropPanel() => TogglePanel(cropPanel);
    public void ShowQualityPanel() => TogglePanel(qualityPanel);
    public void ShowRarityPanel() => TogglePanel(rarityPanel);
    public void ShowPlotPanel() => TogglePanel(plotPanel);

    // Optional: Helper method to check if any panel is currently open
    public bool IsAnyPanelOpen()
    {
        return (cropPanel != null && cropPanel.activeSelf) ||
               (qualityPanel != null && qualityPanel.activeSelf) ||
               (rarityPanel != null && rarityPanel.activeSelf) ||
               (plotPanel != null && plotPanel.activeSelf);
    }

    // Optional: Get currently active panel
    public GameObject GetActivePanel()
    {
        if (cropPanel != null && cropPanel.activeSelf) return cropPanel;
        if (qualityPanel != null && qualityPanel.activeSelf) return qualityPanel;
        if (rarityPanel != null && rarityPanel.activeSelf) return rarityPanel;
        if (plotPanel != null && plotPanel.activeSelf) return plotPanel;
        return null;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (cropPanelKey == qualityPanelKey || cropPanelKey == rarityPanelKey || cropPanelKey == plotPanelKey ||
            qualityPanelKey == rarityPanelKey || qualityPanelKey == plotPanelKey ||
            rarityPanelKey == plotPanelKey)
        {
            Debug.LogWarning("AdminPanelManager: Hotkeys should be unique for each panel!");
        }
    }
#endif
}