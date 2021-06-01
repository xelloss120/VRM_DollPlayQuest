using UnityEngine;

public class CtrlModel : MonoBehaviour
{
    public enum Touch
    {
        VRM_FK,
        VRM_IK,
        GLB,
        Delete,
        Hide
    }
    public Touch Is;
}
