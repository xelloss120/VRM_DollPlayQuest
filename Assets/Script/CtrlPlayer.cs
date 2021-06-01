using UnityEngine;

public class CtrlPlayer : MonoBehaviour
{
    public enum Touch
    {
        PositionF,
        PositionB,
        PositionL,
        PositionR,
        PositionU,
        PositionD,
        RotationL,
        RotationR,
        ScaleU,
        ScaleD
    }
    public Touch Is;
}
