using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class URDFPointCloudManager : MonoBehaviour
{
    public GameObject model;
    private List<Vector3> points;
    public GameObject pointModel;
    public GameObject fitModel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RetrievePoints();
        Profiler.BeginSample("ICP");
        FitModel();
        Profiler.EndSample();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void RetrievePoints()
    {
        MeshFilter[] meshFilters = model.GetComponentsInChildren<MeshFilter>(true);
        points = new List<Vector3>();
        Quaternion modelInverseRotation = Quaternion.Inverse(model.transform.rotation);
        foreach (MeshFilter filter in meshFilters) {
            Vector3[] vertices = filter.sharedMesh.vertices;
            for (int i=0; i<vertices.Count(); i++) {
                vertices[i] = filter.gameObject.transform.rotation * vertices[i] + filter.gameObject.transform.position;  // Get coordinates
                vertices[i] = modelInverseRotation * (vertices[i] - model.transform.position);  // Remove parent transform
            }
            points.AddRange(vertices);
        }
        // JarvisMarch jarvisMarch = new JarvisMarch();
        // List<int> triangles;
        // List<int> hull = jarvisMarch.ConvexHull3D(points, out triangles);
        for (int i=0; i<points.Count; i+=50) {
            GameObject point = Instantiate(pointModel, transform);
            point.transform.position = points[i];
        }
    }

    void FitModel() {
        // Decimate for debugging
        List<Vector3> _points = new List<Vector3>();
        for (int i=0; i<points.Count; i+=200) _points.Add(points[i]);
        points = _points;

        List<Vector3> moved = new List<Vector3>(points);
        for (int i=0; i<moved.Count; i++) {
            moved[i] = new Vector3(moved[i].x + 51f, moved[i].y + 0.05f, moved[i].z);
        }
        GameObject parent = new GameObject();
        for (int i=0; i<points.Count; i+=10) {
            GameObject point = Instantiate(fitModel, parent.transform);
            point.transform.position = moved[i];
        }
        IterativeClosestPoint icp = new IterativeClosestPoint();
        Matrix4x4 transformation = icp.ICP(moved, points, 5, 1e-3f, 1.0f);
        Debug.Log(transformation);
        MatrixHelpers.SetTransformFromMatrix(parent.transform, ref transformation);
    }
}