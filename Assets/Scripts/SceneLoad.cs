using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour {
    [Header("Singleton Manager")]
    public static SceneLoad Instance;

    void OnAwake() {
        if (Instance == null) {
            Instance = this;
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
    }

    public void CloseShop() {
        SceneManager.UnloadSceneAsync("ShopScene");
    }
}
