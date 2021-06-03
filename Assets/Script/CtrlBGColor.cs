using UnityEngine;

public class CtrlBGColor : MonoBehaviour
{
    [SerializeField] Camera Camera;
    [SerializeField] Transform R;
    [SerializeField] Transform G;
    [SerializeField] Transform B;
    [SerializeField] float Adjust = 5;

    void Update()
    {
        var r = Vector3.Distance(R.position, transform.position) * Adjust;
        var g = Vector3.Distance(G.position, transform.position) * Adjust;
        var b = Vector3.Distance(B.position, transform.position) * Adjust;
        Camera.backgroundColor = new Color(r, g, b);
    }
}
