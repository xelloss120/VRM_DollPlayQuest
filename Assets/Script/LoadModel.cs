using System.IO;
using UnityEngine;
using UniGLTF;
using VRM;

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

            DestroyFileNameBar.DestroyBars();
        }
    }
}
