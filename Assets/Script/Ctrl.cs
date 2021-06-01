using UnityEngine;

public class Ctrl : MonoBehaviour
{
    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject X;
    [SerializeField] GameObject Y;
    [SerializeField] GameObject Z;

    public Transform Parent;

    private void Start()
    {
        Parent = transform.parent;
    }

    public void SetColor(Color color)
    {
        Sphere.GetComponent<MeshRenderer>().material.color = color;
    }

    public void SetRendererEnabled(bool enabled)
    {
        Sphere.GetComponent<MeshRenderer>().enabled = enabled;
        X.GetComponent<MeshRenderer>().enabled = enabled;
        Y.GetComponent<MeshRenderer>().enabled = enabled;
        Z.GetComponent<MeshRenderer>().enabled = enabled;
    }
}
