using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public GameObject destroyedVer;

    public float scatterForce = 5f;

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonDash dash = other.GetComponent<ThirdPersonDash>();

        if (dash != null && dash.IsDashing)
        {
            GameObject broken = Instantiate(destroyedVer, transform.position, transform.rotation);

            Rigidbody[] pieces = broken.GetComponentsInChildren<Rigidbody>();

            foreach(Rigidbody rb in pieces)
            {
                Vector3 pushDir = other.transform.forward + (Random.insideUnitSphere * 0.3f);
                rb.AddForce(pushDir * scatterForce, ForceMode.Impulse);
            }

            Destroy(gameObject);
        }
    }
}