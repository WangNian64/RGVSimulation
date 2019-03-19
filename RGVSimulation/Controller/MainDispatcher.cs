using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGVSimulation.Trailer;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
namespace RGVSimulation.Controller
{
    public struct DispatchResult
    {
        public Dictionary<RGV, Task> minMatch;
        public double minTime;
    }
    public class MainDispatcher
    {
        ////实际分配的车和任务
        //private List<RGV> trueRGVs;
        //private List<Task> trueTasks;

        //所有的RGVMoveController
        public static RGVMoveController[] RMCList;
        List<RGV> trueRGVs;
        List<Task> trueTasks;
        DispatchResult dispatchResult;

        public DispatchResult DispatchRGV()
        {
            InitialParas();
            if (TrailerNotEmpty())
            {
                //Task>=RGV, 从任务队列中取出小车数目的任务，再进行全排列
                if (GlobalParas.allocableTasks.Count >= GlobalParas.allocableRGVs.Count)
                {
                    List<Dictionary<RGV, Task>> matchList = new List<Dictionary<RGV, Task>>(); //匹配Dic
                    for (int i = 0; i < GlobalParas.allocableRGVs.Count; i++)
                    {
                        trueTasks.Add(GlobalParas.allocableTasks.ElementAt(i));
                    }
                    trueRGVs = GlobalParas.allocableRGVs;
                    //得到对应的匹配组合（car和task一样多）
                    getMatchList(ref matchList, trueRGVs.ToArray(), trueTasks, 0, trueRGVs.Count);

                    //matchList的每一项都应该包括UnAllocableMatch
                    //每次重新分配，不仅计算所有可分配的Car-Task，也要计算不可分配的Car-Task
                    foreach (Dictionary<RGV, Task> matchDic in matchList)
                    {
                        foreach (KeyValuePair<RGV, Task> kvp in GlobalParas.UnAllocableMatch)
                        {
                            matchDic.Add(kvp.Key, kvp.Value);//进行深拷贝
                        }
                    }
                    dispatchResult.minTime = getMinMatch(matchList, ref dispatchResult.minMatch);
                }
                else//Task<RGV，会有空闲的小车，此时需要进行（多选少），再进行全排列
                {
                    //所有任务都要分配
                    foreach (Task t in GlobalParas.allocableTasks)
                    {
                        trueTasks.Add(t);
                    }
                    List<RGV[]> carLists = MathTool<RGV>.GetCombination(GlobalParas.allocableRGVs.ToArray(), trueTasks.Count);

                    dispatchResult.minTime = GlobalParas.INFINITY;
                    foreach (RGV[] carList in carLists)
                    {
                        List<Dictionary<RGV, Task>> aMatchList = new List<Dictionary<RGV, Task>>();
                        getMatchList(ref aMatchList, carList, trueTasks, 0, carList.Length);
                        //matchList的每一项都应该包括unAllocableMatch
                        foreach (Dictionary<RGV, Task> matchDic in aMatchList)
                        {
                            foreach (KeyValuePair<RGV, Task> kvp in GlobalParas.UnAllocableMatch)
                            {
                                matchDic.Add(kvp.Key, kvp.Value);//进行深拷贝
                            }
                        }
                        //计算matchList中的最小匹配minMatch
                        Dictionary<RGV, Task> aMinMatch = new Dictionary<RGV, Task>();
                        double aMinTime = getMinMatch(aMatchList, ref aMinMatch);
                        //aMinTime只是局部最小，还要比较多个matchList的最小值
                        if (aMinTime < dispatchResult.minTime)
                        {
                            dispatchResult.minTime = aMinTime;
                            dispatchResult.minMatch = aMinMatch;
                        }
                    }
                }
            }
            return dispatchResult;
        }
        //// Update is called once per frame
        //void FixedUpdate()
        //{
        //    trueRGVs = new List<RGV>();
        //    trueTasks = new List<Task>();
        //    //重新分配任务，先清空已经分配的任务
        //    foreach (RGV car in GlobalParas.RGVList)
        //    {
        //        CarController cc = GameObject.Find(car.carName).gameObject.GetComponent<CarController>();
        //        if (cc.carMessage.workState == WorkState.Empty || cc.carMessage.workState == WorkState.WayToLoad)
        //        {
        //            cc.task = null;
        //        }
        //    }
        //    if (GlobalParas.allocableTasks.Count > 0 && GlobalParas.allocableRGVs.Count > 0)
        //    {
        //        Dictionary<RGV, Task> minMatch = new Dictionary<RGV, Task>();        //matchList中的最小匹配minMatch
        //                                                                             //Task>=RGV, 从任务队列中取出小车数目的任务，再进行全排列
        //        if (GlobalParas.allocableTasks.Count >= GlobalParas.allocableRGVs.Count)
        //        {
        //            List<Dictionary<RGV, Task>> matchList = new List<Dictionary<RGV, Task>>(); //匹配Dic
        //            for (int i = 0; i < GlobalParas.allocableRGVs.Count; i++)
        //            {
        //                trueTasks.Add(GlobalParas.allocableTasks.ElementAt(i));
        //            }
        //            trueRGVs = GlobalParas.allocableRGVs;
        //            //得到对应的匹配组合（car和task一样多）
        //            getMatchList(ref matchList, trueRGVs.ToArray(), trueTasks, 0, trueRGVs.Count);

        //            //matchList的每一项都应该包括unAllocableMatch
        //            foreach (Dictionary<RGV, Task> matchDic in matchList)
        //            {
        //                foreach (KeyValuePair<RGV, Task> kvp in GlobalParas.UnAllocableMatch)
        //                {
        //                    matchDic.Add(kvp.Key, kvp.Value);//进行深拷贝
        //                }
        //            }
        //            double minTime = getMinMatch(matchList, ref minMatch);
        //        }
        //        else//Task<RGV，会有空闲的小车，此时需要进行（多选少），再进行全排列
        //        {
        //            //所有任务都要分配
        //            foreach (Task t in GlobalParas.allocableTasks)
        //            {
        //                trueTasks.Add(t);
        //            }
        //            List<RGV[]> carLists = MathTool<RGV>.GetCombination(GlobalParas.allocableRGVs.ToArray(), trueTasks.Count);

        //            double minTime = GlobalParas.INFINITY;
        //            foreach (RGV[] carList in carLists)
        //            {
        //                List<Dictionary<RGV, Task>> aMatchList = new List<Dictionary<RGV, Task>>();
        //                getMatchList(ref aMatchList, carList, trueTasks, 0, carList.Length);
        //                //matchList的每一项都应该包括unAllocableMatch
        //                foreach (Dictionary<RGV, Task> matchDic in aMatchList)
        //                {
        //                    foreach (KeyValuePair<RGV, Task> kvp in GlobalParas.UnAllocableMatch)
        //                    {
        //                        matchDic.Add(kvp.Key, kvp.Value);//进行深拷贝
        //                    }
        //                }
        //                //计算matchList中的最小匹配minMatch
        //                Dictionary<RGV, Task> aMinMatch = new Dictionary<RGV, Task>();
        //                double aMinTime = getMinMatch(aMatchList, ref aMinMatch);
        //                //aMinTime只是局部最小，还要比较多个matchList的最小值
        //                if (aMinTime < minTime)
        //                {
        //                    minTime = aMinTime;
        //                    minMatch = aMinMatch;
        //                }
        //            }
        //        }
        //        //给各个小车分发任务
        //        DistributeTasks(minMatch);
        //    }
        //}
        public void InitialParas()
        {
            trueRGVs = new List<RGV>();
            trueTasks = new List<Task>();
            dispatchResult = new DispatchResult();
        }
        public bool TrailerNotEmpty()
        {
            return GlobalParas.allocableTasks.Count > 0 && GlobalParas.allocableRGVs.Count > 0;
        }
        #region //计算x个车和x个任务的所有排列
        public static void getMatchList(ref List<Dictionary<RGV, Task>> matchList, RGV[] carList, List<Task> taskList, int begin, int end)
        {
            if (begin == end)
            {
                //添加一个匹配
                Dictionary<RGV, Task> match = new Dictionary<RGV, Task>();
                for (int i = 0; i < carList.Length; i++)
                {
                    match.Add(carList[i], taskList[i]);
                }
                matchList.Add(match);
            }
            for (int i = begin; i < end; i++)
            {
                if (IsSwap(taskList, begin, i))
                {
                    Swap(taskList, begin, i);
                    getMatchList(ref matchList, carList, taskList, begin + 1, end);
                    Swap(taskList, begin, i);
                }
            }
        }
        //判断是否重复,重复的话要交换
        public static bool IsSwap(List<Task> tasks, int nBegin, int nEnd)
        {
            for (int i = nBegin; i < nEnd; i++)
                if (tasks[i] == tasks[nEnd])
                    return false;
            return true;
        }
        //交换数组中指定元素
        static void Swap(List<Task> tasks, int x, int y)
        {
            Task t = tasks[x];
            tasks[x] = tasks[y];
            tasks[y] = t;
        }
        #endregion

        #region //n中取出m个的全部组合
        public List<List<RGV>> GetCombination(RGV[] cars, int m)
        {
            if (cars.Length < m)
            {
                return null;
            }
            int[] temp = new int[m];
            List<RGV[]> carLists = new List<RGV[]>();
            GetCombination(ref carLists, cars, cars.Length, m, temp, m);
            //转换格式
            List<List<RGV>> carLists1 = new List<List<RGV>>();
            for (int i = 0; i < carLists.Count; i++)
            {
                carLists1.Add(carLists[i].ToList());
            }
            return carLists1;
        }
        private void GetCombination(ref List<RGV[]> carLists, RGV[] cars, int n, int m, int[] b, int M)
        {
            for (int i = n; i >= m; i--)
            {
                b[m - 1] = i - 1;
                if (m > 1)
                {
                    GetCombination(ref carLists, cars, i - 1, m - 1, b, M);
                }
                else
                {
                    if (carLists == null)
                    {
                        carLists = new List<RGV[]>();
                    }
                    RGV[] temp = new RGV[M];
                    for (int j = 0; j < b.Length; j++)
                    {
                        temp[j] = cars[b[j]];
                    }
                    carLists.Add(temp);
                }
            }
        }
        #endregion

        //计算一个匹配组合完成所有任务的时间
        public static double calcuTasksTime(Dictionary<RGV, Task> match)
        {
            double completeTime = 0.0f;
            double unFinishNum = match.Count;
            RMCList = new RGVMoveController[match.Count];//每次更新RMCList
            int i = 0;
            foreach (KeyValuePair<RGV, Task> kvp in match)
            {
                RMCList[i] = new RGVMoveController(kvp.Key, kvp.Value, GlobalParas.moveSpeed, GlobalParas.safeDis);
                i++;
            }
            while (true)
            {
                foreach (RGVMoveController scc in RMCList)
                {
                    if (unFinishNum <= 0)
                    {
                        return completeTime;
                    }
                    if (scc.task == null)
                    {
                        unFinishNum--;
                    }
                    scc.SimuUpdate();
                }
                completeTime += GlobalParas.frameTime;
            }
        }
        //得到matchList中的时间最小Match, 返回最小match对应的时间, count是每个match的车或任务的数目
        public static double getMinMatch(List<Dictionary<RGV, Task>> matchList, ref Dictionary<RGV, Task> minMatch)
        {
            double[] times = new double[matchList.Count];//任务所需时间
            int index = 0;
            double minTime = GlobalParas.INFINITY;//最短时间
            minMatch = matchList[0];//最短时间的匹配
            foreach (Dictionary<RGV, Task> match in matchList)
            {
                //进行深拷贝,因为minMatch是实际的car、task状态，不能在RGVMoveController中更改
                Dictionary<RGV, Task> tempMatch = new Dictionary<RGV, Task>();
                foreach (RGV rgv in match.Keys)
                {
                    RGV rgvCopy = rgv.Clone();
                    Task task = match[rgv].Clone();
                    tempMatch.Add(rgv, task);
                }
                times[index] = calcuTasksTime(tempMatch);//求所有小车完成所有任务的时间
                if (times[index] < minTime)
                {
                    minTime = times[index];
                    minMatch = match;
                }
                index++;
            }
            return minTime;
        }
    }
}
