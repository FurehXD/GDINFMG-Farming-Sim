using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour {
    [Header("Singleton Manager")]
    public static SceneLoad Instance;
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject farmAreaUI;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            Destroy(this.gameObject);
        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadShop() {
        SceneManager.LoadScene("ShopScene", LoadSceneMode.Additive);
        this.farmAreaUI.gameObject.SetActive(false);
        this.inventoryUI.gameObject.SetActive(false);
    }

    public void CloseShop() {
        SceneManager.UnloadSceneAsync("ShopScene");
        this.farmAreaUI.gameObject.SetActive(true);
        this.inventoryUI.gameObject.SetActive(true);
    }
}
