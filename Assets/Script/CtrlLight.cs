using UnityEngine;

public class CtrlLight : MonoBehaviour
{
    [SerializeField] Light Light;
    [SerializeField] Transform R;
    [SerializeField] Transform G;
    [SerializeField] Transform B;
    [SerializeField] Transform I;
    [SerializeField] float Adjust = 5;

    void Update()
    {
        var r = Vector3.Distance(R.position, transform.position) * Adjust;
        var g = Vector3.Distance(G.position, transform.position) * Adjust;
        var b = Vector3.Distance(B.position, transform.position) * Adjust;
        Light.color = new Color(r, g, b);

        var i = Vector3.Distance(I.position, transform.position) * Adjust;
        Light.intensity = i;
    }
}
