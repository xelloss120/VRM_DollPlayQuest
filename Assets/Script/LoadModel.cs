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

            DestroyFileNameBar.DestroyBars();
        }
    }

    void SetCtrlFK(Animator anim, HumanBodyBones bone)
    {
        var ctrl = Instantiate(CtrlPrefab);
        ctrl.GetComponent<Ctrl>().SetColor(Color.cyan);
        ctrl.AddComponent<CtrlFK>();
        ctrl.transform.parent = anim.GetBoneTransform(bone);
    }

    void SetCtrlIK(Animator anim, HumanBodyBones bone, VRIK vrik, Transform root)
    {
        var ctrl = Instantiate(CtrlPrefab);
        ctrl.GetComponent<Ctrl>().SetColor(Color.cyan);
        ctrl.transform.position = anim.GetBoneTransform(bone).position;
        ctrl.transform.rotation = root.rotation;
        ctrl.transform.parent = root;

        var ctrlIK = ctrl.AddComponent<CtrlIK>();
        ctrlIK.VRIK = vrik;
        ctrlIK.Bone = bone;
        ctrlIK.Target = anim.GetBoneTransform(bone);
    }
}
