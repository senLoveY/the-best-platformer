using UnityEngine;

public class FixHealthBar : MonoBehaviour
{
   private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void LateUpdate()
    {
        if (transform.parent.localScale.x < 0)
        {
            transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
        }
        else
        {
            transform.localScale = initialScale;
        }
    }
}
