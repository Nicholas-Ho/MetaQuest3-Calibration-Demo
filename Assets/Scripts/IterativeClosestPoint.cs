using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class IterativeClosestPoint
{
    [StructLayout(LayoutKind.Sequential)]
    private struct Vector3Cpp {
        public float x, y, z;
    }

    private Vector3Cpp Vector3ToCpp(Vector3 vec) {
        Vector3Cpp vecCpp = new Vector3Cpp();
        vecCpp.x = vec.x;
        vecCpp.y = vec.y;
        vecCpp.z = vec.z;
        return vecCpp;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Matrix4fFlattened {  // Flattened from Eigen3 Matrix4f
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] data;
    }

    [DllImport("pcl_endpoints.dll")]
    private static extern Matrix4fFlattened GetICPTransform(Vector3Cpp[] points, Vector3Cpp[] target, int pointsSize, int targetSize);

    private Matrix4x4 UnflattenMatrix(Matrix4fFlattened matF) {
        Matrix4x4 mat = new Matrix4x4();
        for (int i=0; i<4; i++) {
            for (int j=0; j<4; j++) {
                mat[i,j] = matF.data[i*4+j];
            }
        }
        return mat;
    }

    public Matrix4x4 ICP(List<Vector3> points, List<Vector3> target) {
        return ICP(points.ToArray(), target.ToArray());
    }

    public Matrix4x4 ICP(Vector3[] points, Vector3[] target) {
        int pointsSize = points.Count();
        int targetSize = target.Count();
        Vector3Cpp[] pointsCpp = new Vector3Cpp[pointsSize];
        Vector3Cpp[] targetCpp = new Vector3Cpp[targetSize];
        for (int i=0; i<pointsSize; i++) {
            pointsCpp[i] = Vector3ToCpp(points[i]);
        }
        for (int i=0; i<targetSize; i++) {
            targetCpp[i] = Vector3ToCpp(target[i]);
        }
        Debug.Log(points[0]);
        Debug.Log(target[0]);
        Matrix4fFlattened flattened = GetICPTransform(pointsCpp, targetCpp, pointsSize, targetSize);
        Debug.Log(flattened.data);
        return UnflattenMatrix(flattened);
    }
}