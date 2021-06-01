using System.IO;
using UnityEngine;

public class TrackingHand : MonoBehaviour
{
    [SerializeField] OVRHand OVRHand;
    [SerializeField] SkinnedMeshRenderer HandMesh;
    [SerializeField] GameObject Controller;
    [SerializeField] Transform TrackingSpace;
    [SerializeField] Transform CenterEyeAnchor;

    [SerializeField] float MoveSpeed = 0.01f;
    [SerializeField] float ScalingSpeed = 0.05f;
    [SerializeField] Vector3 RotateSpeed = new Vector3(0, 1, 0);

    [SerializeField] GameObject FileNameBarPrefab;
    [SerializeField] GameObject CtrlPrefab;

    Transform Child = null;
    Transform Parent = null;
    bool CtrlMeshEnabled = true;

#if UNITY_EDITOR
    string ModelDataPath = "Model";
#else
    string ModelDataPath = Application.persistentDataPath;
#endif

    void Update()
    {
        // 手のメッシュ表示をトラッキングと同期
        HandMesh.enabled = OVRHand.IsTracked;

        if (OVRHand.IsTracked)
        {
            // キツネサインで操作盤表示
            var view =
                OVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > 0.7f &&
                OVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > 0.7f;
            Controller.SetActive(view);

            // 離す
            if (OVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) < 0.1f && Child != null)
            {
                Child.parent = Parent;
                Child = null;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 掴む
        if (OVRHand.IsTracked &&
            OVRHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > 0.1f &&
            Child == null && other.GetComponent<Ctrl>() != null)
        {
            Parent = other.transform.parent;
            Child = other.transform;
            Child.parent = OVRHand.transform;
        }

        // 左手操作盤
        var ctrlPlayer = other.GetComponent<CtrlPlayer>();
        if (ctrlPlayer != null)
        {
            switch (ctrlPlayer.Is)
            {
                case CtrlPlayer.Touch.PositionF:
                    TrackingSpace.position += CenterEyeAnchor.forward * MoveSpeed;
                    break;
                case CtrlPlayer.Touch.PositionB:
                    TrackingSpace.position -= CenterEyeAnchor.forward * MoveSpeed;
                    break;
                case CtrlPlayer.Touch.PositionL:
                    TrackingSpace.position -= CenterEyeAnchor.right * MoveSpeed;
                    break;
                case CtrlPlayer.Touch.PositionR:
                    TrackingSpace.position += CenterEyeAnchor.right * MoveSpeed;
                    break;
                case CtrlPlayer.Touch.PositionU:
                    TrackingSpace.position += Vector3.up * MoveSpeed; // 上は真上
                    break;
                case CtrlPlayer.Touch.PositionD:
                    TrackingSpace.position -= Vector3.up * MoveSpeed; // 下は真下
                    break;
                case CtrlPlayer.Touch.RotationL:
                    TrackingSpace.Rotate(-RotateSpeed);
                    break;
                case CtrlPlayer.Touch.RotationR:
                    TrackingSpace.Rotate(RotateSpeed);
                    break;
                case CtrlPlayer.Touch.ScaleU:
                    TrackingSpace.localScale -= Vector3.one * ScalingSpeed;
                    break;
                case CtrlPlayer.Touch.ScaleD:
                    TrackingSpace.localScale += Vector3.one * ScalingSpeed;
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 右手操作盤
        var ctrlModel = other.GetComponent<CtrlModel>();
        if (ctrlModel != null)
        {
            switch (ctrlModel.Is)
            {
                case CtrlModel.Touch.VRM_FK:
                    ViewFiles("vrm", LoadModel.TYPE.VRM_FK);
                    break;
                case CtrlModel.Touch.VRM_IK:
                    ViewFiles("vrm", LoadModel.TYPE.VRM_IK);
                    break;
                case CtrlModel.Touch.GLB:
                    ViewFiles("glb", LoadModel.TYPE.GLB);
                    break;
                case CtrlModel.Touch.Delete:
                    if (Child != null)
                    {
                        var ctrl = Child.GetComponent<Ctrl>();
                        if (ctrl.Parent != null)
                        {
                            Destroy(ctrl.Parent.root.gameObject);
                        }
                        Destroy(ctrl.gameObject);
                    }
                    break;
                case CtrlModel.Touch.Hide:
                    CtrlMeshEnabled = !CtrlMeshEnabled;
                    var objects = FindObjectsOfType(typeof(GameObject));
                    foreach (var obj in objects)
                    {
                        var go = (GameObject)obj;
                        if (go.GetComponent<Ctrl>() != null)
                        {
                            go.GetComponent<Ctrl>().SetRendererEnabled(CtrlMeshEnabled);
                        }
                    }
                    break;
            }
        }
    }

    void ViewFiles(string ext, LoadModel.TYPE type)
    {
        var offsetDepth = CenterEyeAnchor.forward * TrackingSpace.localScale.y * 0.1f;
        var offsetHeight = Vector3.up * TrackingSpace.localScale.y * 0.05f;
        var offset = offsetDepth + offsetHeight;

        // Closeのバー
        var close = Instantiate(FileNameBarPrefab);
        close.transform.position = transform.position + offset;
        close.transform.LookAt(CenterEyeAnchor);

        close.GetComponent<FileNameBar>().TextMesh.text = "Close";
        close.GetComponent<FileNameBar>().TrackingSpace = TrackingSpace;
        close.AddComponent<DestroyFileNameBar>();

        // ファイル名のバー
        var files = Directory.GetFiles(ModelDataPath, "*." + ext);
        foreach (var file in files)
        {
            offset += offsetHeight;

            var fileNameBar = Instantiate(FileNameBarPrefab);
            fileNameBar.transform.position = transform.position + offset;
            fileNameBar.transform.LookAt(CenterEyeAnchor);

            fileNameBar.GetComponent<FileNameBar>().TextMesh.text = Path.GetFileName(file).Replace("." + ext, "");
            fileNameBar.GetComponent<FileNameBar>().TrackingSpace = TrackingSpace;

            fileNameBar.AddComponent<LoadModel>();
            fileNameBar.GetComponent<LoadModel>().Type = type;
            fileNameBar.GetComponent<LoadModel>().FilePath = file;
            fileNameBar.GetComponent<LoadModel>().DestroyFileNameBar = close.GetComponent<DestroyFileNameBar>();
            fileNameBar.GetComponent<LoadModel>().TrackingSpace = TrackingSpace;
            fileNameBar.GetComponent<LoadModel>().CenterEyeAnchor = CenterEyeAnchor;
            fileNameBar.GetComponent<LoadModel>().CtrlPrefab = CtrlPrefab;

            close.GetComponent<DestroyFileNameBar>().Bars.Add(fileNameBar);
        }
    }
}
