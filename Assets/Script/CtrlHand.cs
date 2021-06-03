using System.Collections.Generic;
using UnityEngine;

public class CtrlHand : MonoBehaviour
{
    public Animator Anim;

    public enum LR
    {
        L,
        R
    }
    public LR LeftRight;

    [SerializeField] float Adjust = 5;

    List<Transform> Thumb = new List<Transform>();
    List<Transform> Finger = new List<Transform>();

    Vector3 AngleF;
    Vector3 AngleT;
    Vector3 PeaceI;
    Vector3 PeaceM;

    Vector3 Offset = new Vector3(0.2f, 0, 0);
    Transform Parent;

    void Start()
    {
        Parent = transform.parent;

        transform.rotation = Parent.rotation;

        int start = 0;
        int end = 0;

        // 指の操作対象と曲げ角度の準備
        if (LeftRight == LR.L)
        {
            transform.position = Parent.position;
            transform.localPosition -= Offset;

            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.LeftThumbDistal));

            start = (int)HumanBodyBones.LeftIndexProximal;
            end = (int)HumanBodyBones.LeftLittleDistal;

            AngleF = new Vector3(0, 0, 50);
            AngleT = new Vector3(10, -10, 0);
            PeaceI = new Vector3(0, 10, 0);
            PeaceM = new Vector3(0, -10, 0);
        }
        if (LeftRight == LR.R)
        {
            transform.position = Parent.position;
            transform.localPosition += Offset;

            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.RightThumbProximal));
            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate));
            Thumb.Add(Anim.GetBoneTransform(HumanBodyBones.RightThumbDistal));

            start = (int)HumanBodyBones.RightIndexProximal;
            end = (int)HumanBodyBones.RightLittleDistal;

            AngleF = new Vector3(0, 0, -50);
            AngleT = new Vector3(-10, 10, 0);
            PeaceI = new Vector3(0, -10, 0);
            PeaceM = new Vector3(0, 10, 0);
        }

        // 親指以外の準備
        for (int i = start; i <= end; i++)
        {
            Finger.Add(Anim.GetBoneTransform((HumanBodyBones)i));
        }
    }

    void Update()
    {
        var pos = Parent.InverseTransformPoint(transform.position) * Adjust;
        if (pos.y < 0)
        {
            // ぐー
            foreach (var t in Finger)
            {
                t.localEulerAngles = AngleF * -pos.y;
            }
            Thumb[0].localEulerAngles = AngleT * -pos.y;
            Thumb[1].localEulerAngles = AngleT * -pos.y;
            Thumb[2].localEulerAngles = AngleT * -pos.y;
        }
        else
        {
            // ちょき
            foreach (var t in Finger)
            {
                t.localEulerAngles = AngleF * pos.y;
            }
            Thumb[0].localEulerAngles = AngleT * pos.y;
            Thumb[1].localEulerAngles = AngleT * pos.y;
            Thumb[2].localEulerAngles = AngleT * pos.y;

            Finger[0].localEulerAngles = Vector3.zero + PeaceI * pos.y;
            Finger[1].localEulerAngles = Vector3.zero;
            Finger[2].localEulerAngles = Vector3.zero;
            Finger[3].localEulerAngles = Vector3.zero + PeaceM * pos.y;
            Finger[4].localEulerAngles = Vector3.zero;
            Finger[5].localEulerAngles = Vector3.zero;
        }
    }
}
