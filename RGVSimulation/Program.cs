using System;
using System.Collections.Generic;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
using RGVSimulation.Controller;
namespace RGVSimulation
{
    class Program
    {
        //测试代码
        static void Main(string[] args)
        {
            //初始化轨道图
            InitialScene.initTrailerGraph();
            //初始化任务和小车
            List<RGV> RGVs = new List<RGV>();
            RGVs.Add(new RGV(1, new Tools.Vector2(10, 10), WorkState.Empty, 0, GlobalParas.edges[1], 0));
            RGVs.Add(new RGV(2, new Tools.Vector2(10, -10), WorkState.Empty, 0, GlobalParas.edges[2], 0));
            List<Task> Tasks = new List<Task>();
            Tasks.Add(new Task(1, new Vector2(-10, -15), GlobalParas.edges[4], 
                                  new Vector2(-10, -5), GlobalParas.edges[4],
                                  GlobalParas.loadTime, GlobalParas.unloadTime));
            Tasks.Add(new Task(2, new Vector2(-10, 5), GlobalParas.edges[5],
                                  new Vector2(-10, 15), GlobalParas.edges[5],
                                  GlobalParas.loadTime, GlobalParas.unloadTime));
            InitialScene.initScene(RGVs, Tasks);
            //计算最佳分配
            DispatchResult dispatchResult = new MainDispatcher().DispatchRGV();
            //输出最后结果
            OutputDispatchResult(dispatchResult);

            Console.ReadKey();
        }
        public static void OutputDispatchResult(DispatchResult dispatchResult)
        {
            Console.WriteLine("最短时间：" + dispatchResult.minTime);
            Console.WriteLine("最佳调度策略：");
            foreach (KeyValuePair<RGV, Task> kvp in dispatchResult.minMatch)
            {
                Console.WriteLine("(RGVID:" + kvp.Key.ID + ", TaskID:" + kvp.Value.ID + ")");
            }
        }
    }
}
