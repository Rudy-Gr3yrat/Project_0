using UnityEngine;
using UnityEngine.UI;

public class DashStamina : MonoBehaviour
{
    [Header("UI")]
    public Image gaugeImage;

    [Header("Timing")]
    public float refillTime = 0.6f;

    float charge = 1f;

    void Start()
    {
        if (gaugeImage != null) gaugeImage.fillAmount = charge;
    }

    void Update()
    {
        if (charge < 1f)
        {
            charge = Mathf.Clamp01(charge + Time.deltaTime / refillTime);
            if (gaugeImage != null) gaugeImage.fillAmount = charge;
        }
    }

    public bool CanDash() => charge >= 1f;

    public void Consume()
    {
        charge = 0f;
        if (gaugeImage != null) gaugeImage.fillAmount = 0f;
    }
}