using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UniGLTF;
using VRM;
using RootMotion.FinalIK;

public class LoadModel : MonoBehaviour
{
    public enum TYPE
    {
        VRM_FK,
        VRM_IK,
        GLB
    }
    public TYPE Type;
    public string FilePath;
    public DestroyFileNameBar DestroyFileNameBar;

    public Transform TrackingSpace;
    public Transform CenterEyeAnchor;
    public GameObject CtrlPrefab;

    static GameObject GLB;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TrackingHand>() != null)
        {
            var bytes = File.ReadAllBytes(FilePath);

            if (Type == TYPE.GLB)
            {
                // GLB読み込み
                if (GLB != null)
                {
                    Destroy(GLB);
                }

                var glb = new ImporterContext();
                glb.ParseGlb(bytes);
                glb.Load();
                glb.ShowMeshes();
                glb.Root.transform.position = Vector3.zero;

                GLB = glb.Root;

                DestroyFileNameBar.DestroyBars();
                return;
            }

            // 位置コントローラ
            var root = Instantiate(CtrlPrefab);
            root.GetComponent<Ctrl>().SetColor(Color.cyan);

            // 本体読み込み
            var context = new VRMImporterContext();
            context.ParseGlb(bytes);
            context.Load();
            context.ShowMeshes();

            // 本体に位置コントローラを付ける
            context.Root.transform.position = new Vector3(0, 0.2f, 0);
            context.Root.transform.parent = root.transform;

            // 位置調整
            root.transform.position = CenterEyeAnchor.position;
            root.transform.position += CenterEyeAnchor.forward * TrackingSpace.localScale.y * 0.5f;
            root.transform.position -= new Vector3(0, 1.75f, 0);

            // 角度調整
            root.transform.LookAt(CenterEyeAnchor);
            root.transform.eulerAngles = new Vector3(0, root.transform.eulerAngles.y, 0);

            // 揺れ物の紐づけ
            var springBones = context.Root.GetComponentsInChildren<VRMSpringBone>();
            var springBonesColliders = TrackingSpace.GetComponentsInChildren<VRMSpringBoneColliderGroup>();
            foreach (var springBone in springBones)
            {
                if(springBone.ColliderGroups == null)
                {
                    // 揺れ物にコライダー設定がない場合もあるので回避
                    continue;
                }

                var list = new List<VRMSpringBoneColliderGroup>(springBone.ColliderGroups);
                foreach (var springBonesCollider in springBonesColliders)
                {
                    list.Add(springBonesCollider);
                }
                springBone.ColliderGroups = list.ToArray();
            }

            // 関節
            var anim = context.Root.GetComponent<Animator>();
            if (Type == TYPE.VRM_FK)
            {
                SetCtrlFK(anim, HumanBodyBones.Hips);
                SetCtrlFK(anim, HumanBodyBones.LeftUpperLeg);
                SetCtrlFK(anim, HumanBodyBones.RightUpperLeg);
                SetCtrlFK(anim, HumanBodyBones.LeftLowerLeg);
                SetCtrlFK(anim, HumanBodyBones.RightLowerLeg);
                SetCtrlFK(anim, HumanBodyBones.LeftFoot);
                SetCtrlFK(anim, HumanBodyBones.RightFoot);
                SetCtrlFK(anim, HumanBodyBones.Spine);
                SetCtrlFK(anim, HumanBodyBones.Chest);
                SetCtrlFK(anim, HumanBodyBones.Neck);
                SetCtrlFK(anim, HumanBodyBones.Head);
                SetCtrlFK(anim, HumanBodyBones.LeftUpperArm);
                SetCtrlFK(anim, HumanBodyBones.RightUpperArm);
                SetCtrlFK(anim, HumanBodyBones.LeftLowerArm);
                SetCtrlFK(anim, HumanBodyBones.RightLowerArm);
                SetCtrlFK(anim, HumanBodyBones.LeftHand);
                SetCtrlFK(anim, HumanBodyBones.RightHand);
            }
            if (Type == TYPE.VRM_IK)
            {
                var vrik = context.Root.AddComponent<VRIK>();

                // Target
                SetCtrlIK(anim, HumanBodyBones.Head, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.LeftHand, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.RightHand, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.LeftFoot, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.RightFoot, vrik, root.transform);

                // Swivel
                SetCtrlIK(anim, HumanBodyBones.LeftLowerArm, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.RightLowerArm, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.LeftLowerLeg, vrik, root.transform);
                SetCtrlIK(anim, HumanBodyBones.RightLowerLeg, vrik, root.transform);
            }

            // 表情
            var shape = Instantiate(CtrlPrefab);
            shape.GetComponent<Ctrl>().SetColor(Color.yellow);
            shape.AddComponent<CtrlShape>();
            shape.GetComponent<CtrlShape>().Proxy = context.Root.GetComponent<VRMBlendShapeProxy>();
            shape.transform.parent = anim.GetBoneTransform(HumanBodyBones.Head);

            // 口
            var mouth = Instantiate(CtrlPrefab);
            mouth.GetComponent<Ctrl>().SetColor(Color.yellow);
            mouth.AddComponent<CtrlMouth>();
            mouth.GetComponent<CtrlMouth>().Proxy = context.Root.GetComponent<VRMBlendShapeProxy>();
            mouth.transform.parent = anim.GetBoneTransform(HumanBodyBones.Head);

            // 視線
            var eye = Instantiate(CtrlPrefab);
            eye.GetComponent<Ctrl>().SetColor(Color.yellow);
            eye.AddComponent<CtrlEye>();
            eye.GetComponent<CtrlEye>().LookAt = context.Root.GetComponent<VRMLookAtHead>();
            eye.transform.parent = anim.GetBoneTransform(HumanBodyBones.Head);

            // 左手
            var handL = Instantiate(CtrlPrefab);
            handL.GetComponent<Ctrl>().SetColor(Color.magenta);
            handL.AddComponent<CtrlHand>();
            handL.GetComponent<CtrlHand>().Anim = anim;
            handL.GetComponent<CtrlHand>().LeftRight = CtrlHand.LR.L;
            handL.transform.parent = anim.GetBoneTransform(HumanBodyBones.LeftHand);

            // 右手
            var handR = Instantiate(CtrlPrefab);
            handR.GetComponent<Ctrl>().SetColor(Color.magenta);
            handR.AddComponent<CtrlHand>();
            handR.GetComponent<CtrlHand>().Anim = anim;
            handR.GetComponent<CtrlHand>().LeftRight = CtrlHand.LR.R;
            handR.transform.parent = anim.GetBoneTransform(HumanBodyBones.RightHand);

            DestroyFileNameBar.DestroyBars();
        }
    }

    void SetCtrlFK(Animator anim, HumanBodyBones bone)
    {
        if (anim.GetBoneTransform(bone) == null)
        {
            // 必須ではない部位の場合は割り当て設定されていない可能性があるため回避
            return;
        }

        var ctrl = Instantiate(CtrlPrefab);
        ctrl.GetComponent<Ctrl>().SetColor(Color.cyan);
        ctrl.AddComponent<CtrlFK>();
        ctrl.transform.parent = anim.GetBoneTransform(bone);
    }

    void SetCtrlIK(Animator anim, HumanBodyBones bone, VRIK vrik, Transform root)
    {
        if (anim.GetBoneTransform(bone) == null)
        {
            // 必須ではない部位の場合は割り当て設定されていない可能性があるため回避
            return;
        }

        var ctrl = Instantiate(CtrlPrefab);
        ctrl.GetComponent<Ctrl>().SetColor(Color.cyan);
        ctrl.transform.parent = root;

        var ctrlIK = ctrl.AddComponent<CtrlIK>();
        ctrlIK.VRIK = vrik;
        ctrlIK.Bone = bone;
        ctrlIK.Target = anim.GetBoneTransform(bone);
    }
}
