using UnityEngine;
using RootMotion.FinalIK;

public class CtrlIK : MonoBehaviour
{
    public VRIK VRIK;
    public HumanBodyBones Bone;
    public Transform Target;

    Transform Parent;

    void Start()
    {
        Parent = transform.parent;

        switch (Bone)
        {
            case HumanBodyBones.Head:
                VRIK.solver.spine.headTarget = transform;
                Target = null;
                break;
            case HumanBodyBones.LeftHand:
                VRIK.solver.leftArm.target = transform;
                Target = null;
                break;
            case HumanBodyBones.RightHand:
                VRIK.solver.rightArm.target = transform;
                Target = null;
                break;
            case HumanBodyBones.LeftFoot:
                VRIK.solver.leftLeg.target = transform;
                VRIK.solver.leftLeg.positionWeight = 1;
                VRIK.solver.leftLeg.rotationWeight = 1;
                Target = null;
                break;
            case HumanBodyBones.RightFoot:
                VRIK.solver.rightLeg.target = transform;
                VRIK.solver.rightLeg.positionWeight = 1;
                VRIK.solver.rightLeg.rotationWeight = 1;
                Target = null;
                break;
        }
        // Target = nullするのは外でTargetとSwivelで処理を分けないため
    }

    void Update()
    {
        if (Target == null)
        {
            // VRIKのTarget用なので何もしない
            return;
        }
        else if (transform.parent == Parent)
        {
            // VRIKのSwivel用なので位置だけ設定（掴まれていない時）
            transform.position = Target.position;
        }

        // 親からの相対的な回転を求めてVRIKのSwivelに設定
        var rotation = Quaternion.Inverse(Parent.rotation) * transform.rotation;
        switch(Bone)
        {
            case HumanBodyBones.LeftLowerArm:
                VRIK.solver.leftArm.swivelOffset = rotation.eulerAngles.x;
                break;
            case HumanBodyBones.RightLowerArm:
                VRIK.solver.rightArm.swivelOffset = rotation.eulerAngles.x;
                break;
            case HumanBodyBones.LeftLowerLeg:
                VRIK.solver.leftLeg.swivelOffset = rotation.eulerAngles.y;
                break;
            case HumanBodyBones.RightLowerLeg:
                VRIK.solver.rightLeg.swivelOffset = rotation.eulerAngles.y;
                break;
        }
    }
}
