using System;
using System.Collections;
using System.Collections.Generic;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
namespace RGVSimulation.Trailer
{
    //轨道有向图的拓扑结构信息
    public class TrailerGraph
    {
        //图中所能包含的点上限
        public const int MAX_VERTEX_NUMBER = 1000;
        //顶点数组
        public Vertex[] vertexes;
        //邻接矩阵
        public Edge[,] adjMatrix;
        //点数目
        public int vertexNum = 0;
        //最短路径
        public ShortPath[,] shortPathsMap;//所有点的最短路径矩阵
        public double[,] distMap;//最短路径长度矩阵
        public int[,] vertexIndexMap;//最短路径节点矩阵

        //初始化num个点的图
        public TrailerGraph(int vertexNum = MAX_VERTEX_NUMBER)
        {
            this.vertexNum = vertexNum;
            adjMatrix = new Edge[vertexNum, vertexNum];
            vertexes = new Vertex[vertexNum];
            distMap = new double[vertexNum, vertexNum];
            vertexIndexMap = new int[vertexNum, vertexNum];
        }
        //向图中添加节点
        public void AddVertex(int index, Vector2 pos)
        {
            vertexes[vertexNum] = new Vertex(index, pos);
            vertexNum++;
        }
        //向图中添加有向边
        public void AddEdge(Edge edge)
        {
            if (edge is StraightEdge)
            {
                StraightEdge straightEdge = edge as StraightEdge;
                adjMatrix[straightEdge.vertex1.index, straightEdge.vertex2.index] = straightEdge;
            }
            else if (edge is CurveEdge)
            {
                CurveEdge curveEdge = edge as CurveEdge;
                adjMatrix[curveEdge.vertex1.index, curveEdge.vertex2.index] = curveEdge;
            }
            else
            {
                throw new ArgumentException("参数类型错误");
            }
        }
        //生成一个轨道图
        public static TrailerGraph CreateTrailer(List<Edge> edges, List<Vertex> vertexes)
        {
            TrailerGraph trailerGraph = new TrailerGraph(vertexes.Count);
            trailerGraph.vertexNum = vertexes.Count;
            //给点赋值（坐标）
            for (int i = 0; i < vertexes.Count; i++)
            {
                trailerGraph.vertexes[i] = new Vertex(vertexes[i].index, vertexes[i].pos);
            }
            //给对角线的边赋值(默认是直线)
            for (int i = 0; i < vertexes.Count; i++)
            {
                Edge edge = new StraightEdge(vertexes[i], vertexes[i]);
                trailerGraph.AddEdge(edge);
            }
            for (int i = 0; i < edges.Count; i++)
            {
                trailerGraph.AddEdge(edges[i]);
            }
            //得到最短路径相关信息
            getShortPathData(trailerGraph);
            return trailerGraph;
        }


        public static void getShortPathData(TrailerGraph trailerGraph)
        {
            trailerGraph.distMap = new double[trailerGraph.vertexNum, trailerGraph.vertexNum];
            trailerGraph.vertexIndexMap = new int[trailerGraph.vertexNum, trailerGraph.vertexNum];
            trailerGraph.shortPathsMap = new ShortPath[trailerGraph.vertexNum, trailerGraph.vertexNum];
            for (int i = 0; i < trailerGraph.vertexNum; i++)
            {
                for (int j = 0; j < trailerGraph.vertexNum; j++)
                {
                    trailerGraph.shortPathsMap[i, j] = new ShortPath();
                }
            }
            //根据图来初始化最短路径
            trailerGraph.floyd();
            trailerGraph.getAllVertexList();
        }
        //计算所有点到所有其他点的最短路径
        public void floyd()
        {
            int i, j, k;
            //初始化路径数组
            for (i = 0; i < vertexNum; i++)
            {
                for (j = 0; j < vertexNum; j++)
                {
                    if (adjMatrix[i, j] == null)
                    {
                        distMap[i, j] = GlobalParas.INFINITY;
                    } 
                    else
                    {
                        distMap[i, j] = adjMatrix[i, j].length;
                    }
                    vertexIndexMap[i, j] = -1;
                }
            }
            //计算最短路径
            for (k = 0; k < vertexNum; k++)
            {
                for (i = 0; i < vertexNum; i++)
                {
                    for (j = 0; j < vertexNum; j++)
                    {
                        if (distMap[i, j] > distMap[i, k] + distMap[k, j])
                        {
                            distMap[i, j] = distMap[i, k] + distMap[k, j];
                            vertexIndexMap[i, j] = k;
                        }
                    }
                }
            }
        }
        //计算每个节点到其他所有点的路径列表
        public void getAllVertexList()
        {
            int i, j;
            for (i = 0; i < vertexNum; i++)
            {
                for (j = 0; j < vertexNum; j++)
                {
                    if (distMap[i, j] == GlobalParas.INFINITY)
                    {
                        if (i != j)
                        {
                            shortPathsMap[i, j] = null;
                        }
                    }
                    else
                    {
                        shortPathsMap[i, j].length = distMap[i, j];
                        shortPathsMap[i, j].vertexList.Add(vertexes[i]);
                        getVertexList(shortPathsMap[i, j], i, j);
                        shortPathsMap[i, j].vertexList.Add(vertexes[j]);
                    }
                }
            }
        }
        //计算一个点到其他所有点的路径列表
        public void getVertexList(ShortPath pd, int i, int j)
        {
            int k;
            k = vertexIndexMap[i, j];
            if (k == -1)
            {
                return;
            }
            getVertexList(pd, i, k);
            pd.vertexList.Add(vertexes[k]);
            getVertexList(pd, k, j);
        }
    }
}

