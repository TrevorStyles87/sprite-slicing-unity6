using System;
using System.Collections.Generic;
using UnityEngine;

namespace Slicing.Core
{
    public enum PointSide
    {
        Left,
        Right,
        On
    }

    public struct SliceResult
    {
        public List<Vector2> LeftPolygon;
        public List<Vector2> RightPolygon;
    }

    public struct Line2D
    {
        public Vector2 A;
        public Vector2 B;

        public Line2D(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }

        public Vector2 Direction => B - A;
    }

    public static class GeometryMath
    {
        public const float DefaultTolerance = 0.0001f;

        public static float ComputeArea(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return 0f;

            float area = 0f;
            for (int i = 0; i < polygon.Count; i++)
            {
                var curr = polygon[i];
                var next = polygon[(i + 1) % polygon.Count];
                area += curr.x * next.y - next.x * curr.y;
            }

            return Mathf.Abs(area) * 0.5f;
        }

        public static Vector2 ComputeCentroid(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return Vector2.zero;

            float area = 0f;
            Vector2 centroid = Vector2.zero;

            for (int i = 0; i < polygon.Count; i++)
            {
                var curr = polygon[i];
                var next = polygon[(i + 1) % polygon.Count];
                float cross = curr.x * next.y - next.x * curr.y;
                area += cross;
                centroid += (curr + next) * cross;
            }

            area *= 0.5f;
            if (Mathf.Abs(area) < 0.000001f)
                return Vector2.zero;

            centroid *= 1f / (6f * area);
            return centroid;
        }

        public static Bounds ComputeBounds(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count == 0)
                return new Bounds();

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var point in polygon)
            {
                minX = Mathf.Min(minX, point.x);
                maxX = Mathf.Max(maxX, point.x);
                minY = Mathf.Min(minY, point.y);
                maxY = Mathf.Max(maxY, point.y);
            }

            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            Vector3 size = new Vector3(maxX - minX, maxY - minY, 0f);
            return new Bounds(center, size);
        }

        public static bool IsClockwise(List<Vector2> polygon)
        {
            return SignedArea(polygon) < 0f;
        }

        public static float SignedArea(List<Vector2> polygon)
        {
            float area = 0f;
            for (int i = 0; i < polygon.Count; i++)
            {
                var curr = polygon[i];
                var next = polygon[(i + 1) % polygon.Count];
                area += curr.x * next.y - next.x * curr.y;
            }

            return area * 0.5f;
        }

        public static PointSide ClassifyPoint(Vector2 point, Vector2 a, Vector2 b)
        {
            float cross = (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x);
            if (Mathf.Abs(cross) <= DefaultTolerance)
                return PointSide.On;
            return cross > 0f ? PointSide.Left : PointSide.Right;
        }

        public static Vector2 Intersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            float denominator = (a1.x - a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x - b2.x);
            if (Mathf.Abs(denominator) < 0.000001f)
                return Vector2.zero;

            float x = ((a1.x * a2.y - a1.y * a2.x) * (b1.x - b2.x) - (a1.x - a2.x) * (b1.x * b2.y - b1.y * b2.x)) / denominator;
            float y = ((a1.x * a2.y - a1.y * a2.x) * (b1.y - b2.y) - (a1.y - a2.y) * (b1.x * b2.y - b1.y * b2.x)) / denominator;
            return new Vector2(x, y);
        }

        public static List<Vector2> ClipAgainstLine(List<Vector2> polygon, Vector2 lineStart, Vector2 lineEnd)
        {
            if (polygon == null || polygon.Count < 3)
                return new List<Vector2>();

            List<Vector2> output = new List<Vector2>(polygon.Count);
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 current = polygon[i];
                Vector2 previous = polygon[(i - 1 + polygon.Count) % polygon.Count];

                PointSide currentSide = ClassifyPoint(current, lineStart, lineEnd);
                PointSide previousSide = ClassifyPoint(previous, lineStart, lineEnd);

                if (currentSide == PointSide.Left)
                {
                    if (previousSide == PointSide.Right)
                    {
                        output.Add(Intersect(previous, current, lineStart, lineEnd));
                    }

                    output.Add(current);
                }
                else if (currentSide == PointSide.Right)
                {
                    if (previousSide == PointSide.Left)
                    {
                        output.Add(Intersect(previous, current, lineStart, lineEnd));
                    }
                }
            }

            return output;
        }

        public static SliceResult SplitPolygonAlongLine(List<Vector2> polygon, Vector2 lineStart, Vector2 lineEnd)
        {
            var left = new List<Vector2>();
            var right = new List<Vector2>();

            if (polygon == null || polygon.Count < 3)
                return new SliceResult { LeftPolygon = left, RightPolygon = right };

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 current = polygon[i];
                Vector2 previous = polygon[(i - 1 + polygon.Count) % polygon.Count];
                PointSide sideCurrent = ClassifyPoint(current, lineStart, lineEnd);
                PointSide sidePrevious = ClassifyPoint(previous, lineStart, lineEnd);

                if (sideCurrent == PointSide.Left || sideCurrent == PointSide.On)
                    left.Add(current);
                else
                    right.Add(current);

                if (sideCurrent == PointSide.Left && sidePrevious == PointSide.Right)
                {
                    var intersection = Intersect(previous, current, lineStart, lineEnd);
                    left.Add(intersection);
                    right.Add(intersection);
                }
                else if (sideCurrent == PointSide.Right && sidePrevious == PointSide.Left)
                {
                    var intersection = Intersect(previous, current, lineStart, lineEnd);
                    left.Add(intersection);
                    right.Add(intersection);
                }
            }

            if (left.Count < 3)
                left.Clear();
            if (right.Count < 3)
                right.Clear();

            return new SliceResult { LeftPolygon = left, RightPolygon = right };
        }

        public static List<Vector2> TriangulateEarClipping(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return new List<Vector2>();

            var vertices = new List<Vector2>(polygon);
            var triangles = new List<Vector2>();

            while (vertices.Count >= 3)
            {
                bool earFound = false;
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector2 prev = vertices[(i - 1 + vertices.Count) % vertices.Count];
                    Vector2 curr = vertices[i];
                    Vector2 next = vertices[(i + 1) % vertices.Count];

                    float area = Mathf.Abs((next.x - prev.x) * (curr.y - prev.y) - (next.y - prev.y) * (curr.x - prev.x));
                    if (area <= 0.000001f)
                    {
                        vertices.RemoveAt(i);
                        earFound = true;
                        break;
                    }

                    bool isConvex = (next - curr).x * (curr - prev).y - (next - curr).y * (curr - prev).x >= 0f;
                    if (isConvex)
                    {
                        triangles.Add(prev);
                        triangles.Add(curr);
                        triangles.Add(next);
                        vertices.RemoveAt(i);
                        earFound = true;
                        break;
                    }
                }

                if (!earFound)
                    break;
            }

            return triangles;
        }

        public static bool HasValidWinding(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count < 3)
                return false;

            return Mathf.Abs(SignedArea(polygon)) > 0.0001f;
        }

        public static float GetToleranceFromBounds(List<Vector2> polygon)
        {
            if (polygon == null || polygon.Count == 0)
                return DefaultTolerance;

            Bounds bounds = ComputeBounds(polygon);
            float maxDimension = Mathf.Max(bounds.size.x, bounds.size.y, 1f);
            return Mathf.Max(DefaultTolerance, maxDimension * 0.0005f);
        }
    }
}
