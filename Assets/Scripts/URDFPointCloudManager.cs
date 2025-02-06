using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GK;

public class URDFPointCloudManager : MonoBehaviour
{
    public GameObject model;
    private List<Vector3> points;
    public GameObject pointModel;
    public GameObject fitModel;
    public Material hullMaterial;
    private Mesh hullMesh;

    // Start is called before the first frame update
    void Start()
    {
        RetrievePoints();
        GenerateConvexHull();
        Demo();
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
    }

    void GenerateConvexHull()
    {
        ConvexHullCalculator calc = new ConvexHullCalculator();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        calc.GenerateHull(points, false, ref verts, ref tris, ref normals);

        hullMesh = new Mesh();
        hullMesh.SetVertices(verts);
        hullMesh.SetTriangles(tris, 0);
        hullMesh.SetNormals(normals);
    }

    void Demo()
    {
        // Show points
        for (int i=0; i<points.Count; i+=100) {
            GameObject point = Instantiate(pointModel, transform);
            point.transform.position = points[i];
        }

        // Show hull
        GetComponent<MeshFilter>().sharedMesh = hullMesh;
        GetComponent<MeshRenderer>().material = hullMaterial;

        // Demo ICP
        FitModel();
    }

    void FitModel() {
        List<Vector3> moved = new List<Vector3>(hullMesh.vertices);
        for (int i=0; i<moved.Count; i++) {
            Quaternion rot = new Quaternion();
            rot.eulerAngles = new Vector3(20, 80, 0);
            moved[i] = rot * new Vector3(moved[i].x + 0.21f, moved[i].y + 1f, moved[i].z + 3f);
        }
        GameObject parent = new GameObject();
        for (int i=0; i<hullMesh.vertices.Count(); i++) {
            GameObject point = Instantiate(fitModel, parent.transform);
            point.transform.position = moved[i];
        }
        IterativeClosestPoint icp = new IterativeClosestPoint();
        Matrix4x4 transformation = icp.ICP(moved,
                                           new List<Vector3>(hullMesh.vertices),
                                           10,
                                           1e-5f,
                                           1.0f,
                                           true);
        MatrixHelpers.SetTransformFromMatrix(parent.transform, ref transformation);
    }
}