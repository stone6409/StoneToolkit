using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StoneToolkit.WpfShape.Helpers
{
    public static class GeometryHelper
    {
        public static double NormalizeAngle(double degree)
        {
            if ((degree < 0.0) || (degree > 360.0))
            {
                degree = degree % 360.0;
                if (degree < 0.0)
                {
                    degree += 360.0;
                }
            }
            return degree;
        }

        public static Point GetArcPoint(double degree)
        {
            double angle = Math.PI * degree / 180.0;

            //Start 270 degrees, clock zero
            return new Point(0.5 + (0.5 * Math.Cos(angle)), 0.5 + (0.5 * Math.Sin(angle)));
        }

        public static Point GetArcPoint(double degree, Rect bound)
        {
            Point arcPoint = GetArcPoint(degree);
            return RelativeToAbsolutePoint(bound, arcPoint);
        }

        public static Point RelativeToAbsolutePoint(Rect bound, Point relative)
        {
            return new Point(bound.X + (relative.X * bound.Width), bound.Y + (relative.Y * bound.Height));
        }

        public static double Lerp(double x, double y, double alpha)
        {
            return ((x * (1.0 - alpha)) + (y * alpha));
        }

        public static Point Center(this Rect rect)
        {
            return new Point(rect.X + (rect.Width / 2.0), rect.Y + (rect.Height / 2.0));
        }

        public static bool SetIfDifferent(this DependencyObject dependencyObject,
            DependencyProperty dependencyProperty, object value)
        {
            if (!object.Equals(dependencyObject.GetValue(dependencyProperty), value))
            {
                dependencyObject.SetValue(dependencyProperty, value);
                return true;
            }
            return false;
        }
    }
}
