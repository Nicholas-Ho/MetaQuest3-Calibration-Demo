using System.Collections.Generic;
using UnityEngine;

public class JarvisMarch
{
    private struct Edge {
        public int pt1 { get; }
        public int pt2 { get; }
    
        public Edge(int i1, int i2) {
            pt1 = i1;
            pt2 = i2;
        }

        public Edge(List<int> indices) {
            if (indices.Count != 2) Debug.LogWarning("Indices given to edge is not two.");
            pt1 = indices.Count > 0 ? indices[0] : 0;
            pt2 = indices.Count > 1 ? indices[1] : 0;
        }

        public bool Equals(Edge other) {
            if (pt1 == other.pt1 && pt2 == other.pt2) return true;
            if (pt1 == other.pt2 && pt2 == other.pt1) return true;
            return false;
        }

        public override int GetHashCode() {
            return pt1 * pt2 + pt1 + pt2;
        }
    }

    public List<int> ConvexHull2D(List<Vector3> points3d)
    {
        List<Vector2> points2d = new List<Vector2>();
        foreach (Vector3 point3d in points3d) {
            points2d.Add(new Vector2(point3d.x, point3d.y));
        }
        return ConvexHull2D(points2d);
    }

    public List<int> ConvexHull2D(List<Vector2> points)
    {
        List<int> hullIndices = new List<int>();
        if (points.Count == 1) hullIndices.Add(0);
        if (points.Count <= 1) return hullIndices;

        // Get point with smallest x value to start
        int curr = 0;
        float minX = points[0].x;
        for (int i=1; i<points.Count; i++) {
            if (points[i].x < minX) {
                curr = i;
                minX = points[i].x;
            }
        }
        hullIndices.Add(curr);

        // Iterate
        int next = curr != 0 ? 0 : 1;  // Not curr
        Vector2 prevailingVec, candidateVec;
        while (next != hullIndices[0]) {
            next = 0;
            prevailingVec.x = points[0].x - points[curr].x;
            prevailingVec.y = points[0].y - points[curr].y;
            for (int i=1; i<points.Count; i++) {
                candidateVec.x = points[i].x - points[curr].x;
                candidateVec.y = points[i].y - points[curr].y;
                if ((prevailingVec.x * candidateVec.y) - (prevailingVec.y * candidateVec.x) > 0) {  // cross-product
                    next = i;
                    prevailingVec = candidateVec;
                }
            }
            hullIndices.Add(next);
            curr = next;
        }

        return hullIndices;
    }

    public List<int> ConvexHull2DFirstEdge(List<Vector3> points3d)  // Only one iteration. Returns an edge.
    {
        List<Vector2> points2d = new List<Vector2>();
        foreach (Vector3 point3d in points3d) {
            points2d.Add(new Vector2(point3d.x, point3d.y));
        }
        return ConvexHull2DFirstEdge(points2d);
    }

    public List<int> ConvexHull2DFirstEdge(List<Vector2> points)  // Only one iteration. Returns an edge.
    {
        List<int> hullIndices = new List<int>();
        if (points.Count == 1) hullIndices.Add(0);
        if (points.Count <= 1) return hullIndices;

        // Get point with smallest x value to start
        int curr = 0;
        float minX = points[0].x;
        for (int i=1; i<points.Count; i++) {
            if (points[i].x < minX) {
                curr = i;
                minX = points[i].x;
            }
        }
        hullIndices.Add(curr);

        // Iterate
        int next = 0;  // Not curr
        Vector2 prevailingVec, candidateVec;

        prevailingVec.x = points[0].x - points[curr].x;
        prevailingVec.y = points[0].y - points[curr].y;
        for (int i=1; i<points.Count; i++) {
            candidateVec.x = points[i].x - points[curr].x;
            candidateVec.y = points[i].y - points[curr].y;
            if ((prevailingVec.x * candidateVec.y) - (prevailingVec.y * candidateVec.x) > 0) {  // cross-product
                next = i;
                prevailingVec = candidateVec;
            }
        }
        hullIndices.Add(next);

        return hullIndices;
    }

    public List<int> ConvexHull3D(List<Vector3> points)
    {
        List<int> triangles;
        return ConvexHull3D(points, out triangles);
    }
    
    public List<int> ConvexHull3D(List<Vector3> points, out List<int> triangles)
    {
        List<int> hullIndices = ConvexHull2DFirstEdge(points);
        Stack<Edge> edges = new Stack<Edge>();
        edges.Push(new Edge(hullIndices));
        triangles = new List<int>();

        Edge currEdge;
        int next;
        HashSet<Edge> visited = new HashSet<Edge>();
        while (edges.Count > 0) {
            currEdge = edges.Pop();
            if (visited.Contains(currEdge)) continue;
            next = PivotOnEdge(currEdge, ref points);
            hullIndices.Add(next);
            triangles.Add(currEdge.pt2);
            triangles.Add(currEdge.pt1);
            triangles.Add(next);
            edges.Push(new Edge(currEdge.pt1, next));
            edges.Push(new Edge(currEdge.pt2, next));
            visited.Add(currEdge);
        }

        return hullIndices;
    }

    private int PivotOnEdge(Edge edge, ref List<Vector3> points)
    {
        int next = 0;
        while (next == edge.pt1 || next == edge.pt2) next++;  // Not on edge
        double area = 0.5 * Vector3.Cross(points[edge.pt1] - points[next], points[edge.pt2] - points[next]).magnitude;
        double volume, candidateArea;
        for (int i=next+1; i<points.Count; i++) {
            if (edge.pt1 == i || edge.pt2 == i) continue;
            volume = Vector3.Dot(points[i] - points[next], Vector3.Cross(points[edge.pt1] - points[next], points[edge.pt2] - points[next])) / 6;
            if (volume < 0) {
                next = i;
            } else if (volume == 0) {
                candidateArea = 0.5 * Vector3.Cross(points[edge.pt1] - points[i], points[edge.pt2] - points[i]).magnitude;
                if (candidateArea < area) {
                    next = i;
                    area = candidateArea;
                }
            }
        }
        return next;
    }
}