using System.Collections;
using UnityEngine;

public class Pulser : MonoBehaviour
{
    private Vector3 defaultScale;
    private bool _triggerStop = false;
    private Vector3 pulser = new Vector3(1.0f, 1.0f, 1.0f);

    private bool hasInit = false;

    [SerializeField]
    [Tooltip("If true, this Image will pulse without a trigger, else this.StartPulsing() needs to be called.")]
    private bool automaticallyStarts = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float maxScaleMultiplier = 1.1f;
    private Vector3 maxScale;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    private float pulseRate = 1.0f;

    private bool isGrowing = true; // Toggle between growing and shrinking

    private void Start()
    {
        if (this.automaticallyStarts)
            this.StartPulsing();
    }
    public void StartPulsing()
    {
        if (!hasInit)
        {
            this.defaultScale = this.transform.localScale;
            this.maxScale = this.transform.localScale * maxScaleMultiplier;
            this.pulseRate *= (maxScaleMultiplier - 1.0f) * 10.0f;
            this.hasInit = true;
        }

        this._triggerStop = false;
        StartCoroutine(this.Pulse());
    }

    private IEnumerator Pulse()
    {
        while (!_triggerStop)
        {
            if (isGrowing)
            {
                // Increase scale
                this.transform.localScale += pulseRate * Time.deltaTime * this.pulser;

                if (this.transform.localScale.x >= maxScale.x)
                {
                    this.transform.localScale = maxScale;
                    isGrowing = false; // Switch to shrinking
                }
            }
            else
            {
                // Decrease scale
                this.transform.localScale -= pulseRate * Time.deltaTime * this.pulser;

                if (this.transform.localScale.x <= defaultScale.x)
                {
                    this.transform.localScale = defaultScale;
                    isGrowing = true; // Switch to growing
                }
            }

            yield return null;
        }
    }

    public void StopPulsing()
    {
        this._triggerStop = true;
        this.transform.localScale = this.defaultScale;
    }
}
