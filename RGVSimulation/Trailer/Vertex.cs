using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Tools;
namespace RGVSimulation.Trailer
{
    public class Vertex
    {
        public int index;//顶点索引
        public Vector2 pos;//顶点的坐标
        public Vertex() : this(0, new Vector2())
        {

        }
        public Vertex(int index, Vector2 pos)
        {
            this.index = index;
            this.pos = pos;
        }
    }
}
