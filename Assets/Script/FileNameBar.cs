using UnityEngine;

public class FileNameBar : MonoBehaviour
{
    public TextMesh TextMesh;
    public Transform TrackingSpace;

    void Update()
    {
        transform.localScale = TrackingSpace.localScale;
    }
}
