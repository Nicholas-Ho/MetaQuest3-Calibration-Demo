using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
        RetrievePoints();
        FitModel();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        for (int i=0; i<points.Count; i+=10) {
            GameObject point = Instantiate(pointModel, transform);
            point.transform.position = points[i];
        }
    }

    void FitModel() {
        // Decimate for development
        List<Vector3> _points = new List<Vector3>();
        for (int i=0; i<points.Count; i+=10) _points.Add(points[i]);
        points = _points;

        List<Vector3> moved = new List<Vector3>(points);
        for (int i=0; i<moved.Count; i++) {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(20, 80, 0);
            moved[i] = rot * new Vector3(moved[i].x + 0.1f, moved[i].y + 1f, moved[i].z + 3f);
        }
        GameObject parent = new GameObject();
        for (int i=0; i<points.Count; i++) {
            GameObject point = Instantiate(fitModel, parent.transform);
            point.transform.position = moved[i];
        }
        IterativeClosestPoint icp = new IterativeClosestPoint();
        Matrix4x4 transformation = icp.ICP(moved, points, 50, 1e-5f, 1.0f, true);
        MatrixHelpers.SetTransformFromMatrix(parent.transform, ref transformation);
    }
}