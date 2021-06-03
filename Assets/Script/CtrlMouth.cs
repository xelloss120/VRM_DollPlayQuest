using UnityEngine;
using VRM;

public class CtrlMouth : MonoBehaviour
{
    public VRMBlendShapeProxy Proxy;

    [SerializeField] float Adjust = 3;

    Vector3 Offset = new Vector3(0, -0.2f, 0.2f);
    Transform Parent;

    void Start()
    {
        Parent = transform.parent;

        // 初期位置設定
        transform.position = Parent.position;
        transform.localPosition += Offset;
        transform.rotation = Parent.rotation;
    }

    void Update()
    {
        var pos = (Parent.InverseTransformPoint(transform.position) - Offset) * Adjust;
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.A), Mathf.Min(1, Mathf.Max(0, pos.y)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.I), Mathf.Min(1, -Mathf.Min(0, pos.y)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.U), Mathf.Min(1, Mathf.Max(0, pos.x)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.E), Mathf.Min(1, -Mathf.Min(0, pos.x)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.O), Mathf.Min(1, -Mathf.Min(0, pos.z)));
        Proxy.Apply();
    }
}
