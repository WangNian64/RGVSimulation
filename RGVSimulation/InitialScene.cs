using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Trailer;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
namespace RGVSimulation
{
    public class InitialScene
    {
        public static void initTrailerGraph()
        {
            //初始化图参数
            double curveRadius = 10;
            double lineLength = 20;
            GlobalParas.vertexes = new List<Vertex>{
                new Vertex(0, new Vector2(-curveRadius, lineLength)),
                new Vertex(1, new Vector2(curveRadius, lineLength)),
                new Vertex(2, new Vector2(curveRadius, 0)),
                new Vertex(3, new Vector2(curveRadius, -lineLength)),
                new Vertex(4, new Vector2(-curveRadius, -lineLength)),
                new Vertex(5, new Vector2(-curveRadius, 0)),
            };
            GlobalParas.edges = new List<Edge>{
                new CurveEdge(GlobalParas.vertexes[0], GlobalParas.vertexes[1], -90, 90, curveRadius, new Vector2(0, 20)),
                new StraightEdge(GlobalParas.vertexes[1], GlobalParas.vertexes[2]),
                new StraightEdge(GlobalParas.vertexes[2], GlobalParas.vertexes[3]),
                new CurveEdge(GlobalParas.vertexes[3], GlobalParas.vertexes[4], 90, 270, curveRadius, new Vector2(0, -20)),
                new StraightEdge(GlobalParas.vertexes[4], GlobalParas.vertexes[5]),
                new StraightEdge(GlobalParas.vertexes[5], GlobalParas.vertexes[0]),
                new CurveEdge(GlobalParas.vertexes[2], GlobalParas.vertexes[5], 90, 270, curveRadius, new Vector2(0, 0)),
            };
            //创建轨道对应的有向图
            GlobalParas.trailerGraph = TrailerGraph.CreateTrailer(GlobalParas.edges, GlobalParas.vertexes);
        }
        public static void initScene(List<RGV> RGVs, List<Task> Tasks)
        {
            //初始化小车和任务
            foreach (RGV rgv in RGVs)
            {
                GlobalParas.allAGVs.Add(rgv);
                GlobalParas.allocableRGVs.Add(rgv);
            }
            foreach (Task task in Tasks)
            {
                GlobalParas.allTasks.Add(task);
                GlobalParas.allocableTasks.Add(task);
            }
        }
    }
}
