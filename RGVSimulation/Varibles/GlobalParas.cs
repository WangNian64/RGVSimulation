using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Trailer;
namespace RGVSimulation.Varibles
{
    public struct DispatchResult
    {
        public Dictionary<Task, RGV> minMatch;
        public double minTime;
    }
    public struct TaskRGVPair
    {
        public List<Task> tasks;
        public List<RGV> RGVs;
    }
    public class GlobalParas
    {
        public static double INFINITY = Double.MaxValue;
        //图相关参数
        public static List<Edge> edges = new List<Edge>();
        public static List<Vertex> vertexes = new List<Vertex>();

        public static TrailerGraph trailerGraph;//轨道对应的有向图
        //任务相关参数
        public static List<Task> allTasks = new List<Task>();//总任务列表
        public static List<RGV> allRGVs = new List<RGV>();//所有小车
        public static List<RGV> allocableRGVs = new List<RGV>();//所有可分配的车的信息数组
        public static List<Task> allocableTasks = new List<Task>();//所有可分配的任务
        public static Dictionary<Task, RGV> UnAllocableMatch = new Dictionary<Task, RGV>();//存储当前所有已经不可再分配的Car-Task对

        public static double safeDis = 2.0f;//安全距离
        public static double moveSpeed = 5.0f;//规定所有RGV的移动速度，加速的话可以不一样
        public static double frameTime = 0.02f;//每帧时间，用于模拟计算任务完成总时间
        public static double loadTime = 1.0f;
        public static double unloadTime = 1.0f;
    }
}
