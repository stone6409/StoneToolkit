using System;
using System.Collections.Generic;
using System.Windows;

namespace StoneToolkit.WpfShape.Helpers
{
    /// <summary>
    /// 点集 → Bezier 段插值。
    /// <para>
    /// 采用 Catmull-Rom 样条（与 GraphWpf 的 <c>CurveShape</c> / <c>ClosedCurveShape</c> 一致）：
    /// 切线开放曲线首末取单侧差、中间取双侧平均，闭合曲线取环绕索引平均；
    /// 控制点为 <c>p0 + t0/3</c> 与 <c>p3 - t1/3</c>。
    /// </para>
    /// </summary>
    class InterpolationHelper
    {
        public class BeizerCurveSegment
        {
            public Point StartPoint { get; set; }

            public Point EndPoint { get; set; }

            public Point FirstControlPoint { get; set; }

            public Point SecondControlPoint { get; set; }
        }

        /// <summary>
        /// 生成 Bezier 段：开放曲线 n-1 段，闭合曲线 n 段（含末点→首点）。
        /// <para>不修改入参；点数不足（开放 &lt;2、闭合 &lt;3）时返回 <c>null</c>。</para>
        /// </summary>
        public static List<BeizerCurveSegment> InterpolatePointWithBeizerCurves(List<Point> points, bool isClosedCurve)
        {
            if (points == null)
            {
                return null;
            }

            int count = points.Count;

            if (isClosedCurve ? count < 3 : count < 2)
            {
                return null;
            }

            int segmentCount = isClosedCurve ? count : count - 1;
            List<BeizerCurveSegment> segments = new List<BeizerCurveSegment>(segmentCount);

            for (int index = 0; index < segmentCount; index++)
            {
                int next = (index + 1) % count;

                Point start = points[index];
                Point end = points[next];

                Vector tangent0 = GetTangent(points, index, isClosedCurve);
                Vector tangent1 = GetTangent(points, next, isClosedCurve);

                segments.Add(new BeizerCurveSegment
                {
                    StartPoint = start,
                    EndPoint = end,
                    FirstControlPoint = new Point(start.X + tangent0.X / 3.0, start.Y + tangent0.Y / 3.0),
                    SecondControlPoint = new Point(end.X - tangent1.X / 3.0, end.Y - tangent1.Y / 3.0),
                });
            }

            return segments;
        }

        private static Vector GetTangent(List<Point> points, int index, bool isClosedCurve)
        {
            int count = points.Count;

            if (isClosedCurve)
            {
                return (points[(index + 1) % count] - points[(index - 1 + count) % count]) / 2.0;
            }

            if (index == 0)
            {
                return points[1] - points[0];
            }

            if (index == count - 1)
            {
                return points[count - 1] - points[count - 2];
            }

            return (points[index + 1] - points[index - 1]) / 2.0;
        }
    }
}
