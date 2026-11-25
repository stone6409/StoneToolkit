using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SimpleShape
{
    public abstract class Shape
    {
        protected System.Windows.Shapes.Shape _shape;

        protected Shape()
        {

        }

        public System.Windows.Shapes.Shape InternalShape
        {
            get { return _shape; }
        }

        public double Top
        {
            get
            {
                return (double)InternalShape.GetValue(Canvas.TopProperty);
            }
            set
            {
                InternalShape.SetValue(Canvas.TopProperty, value);
            }
        }

        public double Left
        {
            get
            {
                return (double)InternalShape.GetValue(Canvas.LeftProperty);
            }
            set
            {
                InternalShape.SetValue(Canvas.LeftProperty, value);
            }
        }

        public double Width
        {
            get
            {
                return InternalShape.Width;
            }
            set
            {
                InternalShape.Width = value;
            }
        }

        public double Height
        {
            get
            {
                return InternalShape.Height;
            }
            set
            {
                InternalShape.Height = value;
            }
        }

        public Brush Stroke
        {
            get
            {
                return InternalShape.Stroke;
            }
            set
            {
                InternalShape.Stroke = value;
            }
        }

        public Brush Fill
        {
            get
            {
                return InternalShape.Fill;
            }
            set
            {
                InternalShape.Fill = value;
            }
        }
    }
}
