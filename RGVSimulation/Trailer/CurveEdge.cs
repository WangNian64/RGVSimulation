using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Tools;
namespace RGVSimulation.Trailer
{
    public class CurveEdge : Edge
    {
        public double startDeg;
        public double endDeg;
        public double radius;
        public Vector2 center;
        public CurveEdge() : base()
        {
            this.startDeg = this.endDeg = 0;
            this.radius = 0;
            this.center = new Vector2();
        }
        public CurveEdge(Vertex vertex1, Vertex vertex2,
            double startDeg, double endDeg, double radius, Vector2 center) : base(vertex1, vertex2)
        {
            this.startDeg = startDeg;
            this.endDeg = endDeg;
            this.radius = radius;
            this.center = center;
            this.length = getLength();
        }
        public override double getLength()
        {
            if (startDeg > endDeg)
            {
                throw new ArgumentException("参数设置错误");
            }
            return MathTool<double>.Deg2Rad * Math.Clamp((endDeg - startDeg), 0, 360)
                * 2 * Math.PI * radius;
        }
    }
}
