using UnityEngine;
using VRM;

public class CtrlEye : MonoBehaviour
{
    public VRMLookAtHead LookAt;

    Vector3 Offset = new Vector3(0, 0, 0.5f);

    void Start()
    {
        // 初期位置設定
        transform.position = transform.parent.position;
        transform.localPosition += Offset;
        transform.rotation = transform.parent.rotation;

        LookAt.Target = transform;
    }
}
