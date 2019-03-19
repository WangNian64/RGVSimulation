using System;
using System.Collections.Generic;
using System.Text;

namespace RGVSimulation.Trailer
{
    public abstract class Edge
    {
        public Vertex vertex1;//边的点1
        public Vertex vertex2;//边的点2
        public double length;//边的权值（长度，曲线的弧长）

        public Edge()
        {
            this.vertex1 = new Vertex();
            this.vertex2 = new Vertex();
            this.length = 0;
        }
        public Edge(Vertex vertex1, Vertex vertex2)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.length = getLength();
        }

        public abstract double getLength();
    }
}
