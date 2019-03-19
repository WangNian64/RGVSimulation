using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Varibles;
namespace RGVSimulation.Trailer
{
    //两点之间的最短路径数据
    public class ShortPath
    {
        public double length;
        public List<Vertex> vertexList;
        public ShortPath()
        {
            this.length = GlobalParas.INFINITY;
            this.vertexList = new List<Vertex>(TrailerGraph.MAX_VERTEX_NUMBER);
        }
        public ShortPath(double length, List<Vertex> vertexList)
        {
            this.vertexList = vertexList;
            this.length = length;
        }
        public void PrintShortPaths()
        {
            Console.WriteLine("ShortPath:");
            for (int i = 0; i < vertexList.Count; i++)
            {
                Console.WriteLine(vertexList[i].index + ", ");
            }
        }
    }
}
