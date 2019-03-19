using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Tools;
namespace RGVSimulation.Trailer
{
    public class StraightEdge : Edge
    {
        public StraightEdge() : base()
        {

        }
        public StraightEdge(Vertex vertex1, Vertex vertex2) 
            :base(vertex1, vertex2)
        {
            this.length = getLength();
        }
        public override double getLength()
        {
            return Vector2.Distance(vertex1.pos, vertex2.pos);
        }
    }
}
