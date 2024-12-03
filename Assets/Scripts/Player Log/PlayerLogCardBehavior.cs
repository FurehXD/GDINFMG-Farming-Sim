using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerLogCardBehavior : MonoBehaviour
{
    [SerializeField]
    private float lifespan = 20f;
    [SerializeField]
    private float countdownPrecision = 0.1f;

    private void OnEnable()
    {
        StartCoroutine(StartDecay());
    }
    public void SetMessage(string message)
    {
        TextMeshProUGUI messageTxt = this.GetComponentInChildren<TextMeshProUGUI>();

        messageTxt.text = message;  
    }
    private IEnumerator StartDecay()
    {
        float currentTime = this.lifespan;

        while (currentTime > 0.0f)
        {
            yield return new WaitForSeconds(countdownPrecision);
            currentTime -= this.countdownPrecision;
        }

        Destroy(this.gameObject);
    }
}
