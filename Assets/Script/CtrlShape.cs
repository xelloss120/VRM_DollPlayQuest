using UnityEngine;
using VRM;

public class CtrlShape : MonoBehaviour
{
    public VRMBlendShapeProxy Proxy;

    [SerializeField] float Adjust = 3;

    Vector3 Offset = new Vector3(0, 0.3f, 0);
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
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Joy), Mathf.Min(1, Mathf.Max(0, pos.y)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Angry), Mathf.Min(1, -Mathf.Min(0, pos.y)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Fun), Mathf.Min(1, Mathf.Max(0, pos.x)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Sorrow), Mathf.Min(1, -Mathf.Min(0, pos.x)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink_R), Mathf.Min(1, Mathf.Max(0, pos.z)));
        Proxy.AccumulateValue(BlendShapeKey.CreateFromPreset(BlendShapePreset.Blink), Mathf.Min(1, -Mathf.Min(0, pos.z)));
        Proxy.Apply();
    }
}
