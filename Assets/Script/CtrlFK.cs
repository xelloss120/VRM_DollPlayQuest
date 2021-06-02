using UnityEngine;

public class CtrlFK : MonoBehaviour
{
    Transform Parent;

    void Start()
    {
        Parent = transform.parent;
    }

    void Update()
    {
        if (transform.parent != Parent)
        {
            // 元の親でなければ掴まれていると判断
            Parent.rotation = transform.rotation;
        }
        else
        {
            transform.position = Parent.position;
            transform.rotation = Parent.rotation;
        }
    }
}
