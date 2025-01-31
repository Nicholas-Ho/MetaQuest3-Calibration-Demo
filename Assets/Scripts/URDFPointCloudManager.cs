using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

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
        List<Vector3> moved = new List<Vector3>(points);
        for (int i=0; i<moved.Count; i++) {
            moved[i] = new Vector3(moved[i].x + 1, moved[i].y + 5, moved[i].z);
        }
        GameObject parent = new GameObject();
        for (int i=0; i<points.Count; i+=10) {
            GameObject point = Instantiate(fitModel, parent.transform);
            point.transform.position = moved[i];
        }
        IterativeClosestPoint icp = new IterativeClosestPoint();
        Matrix4x4 transformation = icp.ICP(moved, points);
        Debug.Log(transformation);
        MatrixHelpers.SetTransformFromMatrix(parent.transform, ref transformation);
    }
}