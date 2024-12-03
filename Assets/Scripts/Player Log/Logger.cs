using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour
{
    [SerializeField]
    private PlayerLogCardBehavior playerCardTemplate;
    public static Logger Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void LogMessage(string message)
    {
        VerticalLayoutGroup verticalLOGParent = this.GetComponentInChildren<VerticalLayoutGroup>();

        if (this.playerCardTemplate)
        {
            PlayerLogCardBehavior playerCard = Instantiate(playerCardTemplate, verticalLOGParent.transform);
            playerCard.SetMessage(message);
        }
    }
}
