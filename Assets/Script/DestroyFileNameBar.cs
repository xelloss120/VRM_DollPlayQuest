using System.Collections.Generic;
using UnityEngine;

public class DestroyFileNameBar : MonoBehaviour
{
    public List<GameObject> Bars = new List<GameObject>();

    public void DestroyBars()
    {
        foreach (var bar in Bars)
        {
            Destroy(bar);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrackingHand>() != null)
        {
            DestroyBars();
        }
    }
}
