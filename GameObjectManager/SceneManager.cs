public class SceneManager : MonoBehaviour
{
    private Quadtree quadtree;
    private Rect cameraView;

    void Start()
    {
        quadtree = new Quadtree(0, new Rect(0, 0, 1000, 1000));
\
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            quadtree.Insert(obj);
        }
    }

    void Update()
    {
        // 更新摄像机视野
        cameraView = new Rect(Camera.main.transform.position.x - 50, Camera.main.transform.position.y - 50, 100, 100);

        // 检索视野内的对象
        List<GameObject> visibleObjects = quadtree.Retrieve(new List<GameObject>(), cameraView);

        // 处理可见对象
        foreach (var obj in visibleObjects)
        {
            Debug.Log($"Visible: {obj.name}");
        }
    }
}