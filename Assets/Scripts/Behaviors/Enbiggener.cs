using UnityEngine;

public class Enbiggener : MonoBehaviour
{
    private RectTransform thisRectTransform;

    [SerializeField]
    private float enbiggenScale = 1.25f;
    private bool hasEnbiggened;

    [SerializeField]
    private Vector3 defaultScale;

    private void Start()
    {
        this.thisRectTransform = this.GetComponent<RectTransform>();
        this.defaultScale = this.thisRectTransform.localScale;
    }
    public void Enbiggen()
    {
        if (!this.hasEnbiggened)
        {
            this.thisRectTransform.localScale *= this.enbiggenScale;
            this.hasEnbiggened = true;
        }
    }

    public void ResetSize()
    {
        if (this.hasEnbiggened)
        {
            this.thisRectTransform.localScale = this.defaultScale;
            this.hasEnbiggened = false;
        }
    }
}
