using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
namespace RGVSimulation.Controller
{
    public struct DispatchResult
    {
        public Dictionary<Task, RGV> minMatch;
        public double minTime;
    }
    public class MainDispatcher
    {
        static RGV[] tempRGVs = new RGV[100];//辅助计算排列组合
        public static RGVMoveController[] RMCList;
        DispatchResult dispatchResult;

        public static int unfinishNum;
        public DispatchResult DispatchRGV()
        {
            InitialParas();
            if (TaskRGVNotEmpty())
            {
                //排列组合算法失效了，现在的数目是RGV^Task
                List<Dictionary<Task, RGV>> dispatchList = new List<Dictionary<Task, RGV>>();
                getDispatchList(ref dispatchList, GlobalParas.allocableRGVs.ToArray(), GlobalParas.allocableTasks.ToArray(),
                    GlobalParas.allocableRGVs.Count - 1, GlobalParas.allocableTasks.Count - 1);
                foreach (Dictionary<Task, RGV> matchDic in dispatchList)
                {
                    //对于已经分配了任务的RGV也要加进match
                    foreach (KeyValuePair<Task, RGV> kvp in GlobalParas.UnAllocableMatch)
                    {
                        matchDic.Add(kvp.Key, kvp.Value);//进行深拷贝
                    }
                }
                //计算最佳match和最短时间
                dispatchResult.minTime = getMinMatch(dispatchList, ref dispatchResult.minMatch);
            }
            return dispatchResult;
        }
        public void InitialParas()
        {
            dispatchResult = new DispatchResult();
        }
        public bool TaskRGVNotEmpty()
        {
            return GlobalParas.allocableTasks.Count > 0 && GlobalParas.allocableRGVs.Count > 0;
        }
        //计算出所有Task和RGV匹配的组合，允许相邻任务都是一个RGV调度
        public static void getDispatchList(ref List<Dictionary<Task, RGV>> matchList, RGV[] RGVs, Task[] tasks, int RGVNum, int taskNum)
        {
            int i, j;
            for (i = RGVNum; i >= 0; i--)
            {
                tempRGVs[taskNum] = RGVs[i];
                if (taskNum > 0)
                    getDispatchList(ref matchList, RGVs, tasks, RGVNum, taskNum - 1);
                else
                {
                    Dictionary<Task, RGV> tempMatch = new Dictionary<Task, RGV>();
                    for (j = GlobalParas.allocableTasks.Count - 1; j >= 0; j--)
                    {
                        tempMatch.Add(tasks[GlobalParas.allocableTasks.Count - 1 - j], tempRGVs[j]);
                    }
                    matchList.Add(tempMatch);
                }
            }
        }
        //得到matchList中的时间最小Match, 返回最小match对应的时间, count是每个match的车或任务的数目
        public static double getMinMatch(List<Dictionary<Task, RGV>> matchList, ref Dictionary<Task, RGV> minMatch)
        {
            double[] times = new double[matchList.Count];//任务所需时间
            int index = 0;
            double minTime = GlobalParas.INFINITY;//最短时间
            minMatch = matchList[0];//最短时间的匹配
            foreach (Dictionary<Task, RGV> match in matchList)
            {
                //进行深拷贝,因为minMatch是实际的car、task状态，不能在RGVMoveController中更改
                Dictionary<Task, RGV> tempMatch = new Dictionary<Task, RGV>();
                foreach (Task task in match.Keys)
                {
                    Task taskCopy = task.Clone();
                    RGV rgv = match[task].Clone();
                    tempMatch.Add(taskCopy, rgv);
                }
                times[index] = calcuTasksTime(tempMatch);//求所有RGV完成所有任务的时间
                if (times[index] < minTime)
                {
                    minTime = times[index];
                    minMatch = match;
                }
                index++;
            }
            return minTime;
        }
        //计算一个匹配组合完成所有任务的时间
        public static double calcuTasksTime(Dictionary<Task, RGV> match)
        {
            double completeTime = 0.0f;
            double aCompleteTime = 0.0f;
            unfinishNum = match.Count;
            int curIndex = 0;
            bool aDispatchFin = true;
            while (unfinishNum > 0)
            {
                bool sameRGV = false;
                //一波结束，分配一波新的任务
                if (aDispatchFin == true)
                {
                    //初始化RMCList
                    RMCList = new RGVMoveController[GlobalParas.allocableRGVs.Count];
                    int i = 0;
                    foreach (RGV rgv in GlobalParas.allocableRGVs)
                    {
                        RMCList[i++] = new RGVMoveController(null, rgv, GlobalParas.moveSpeed, GlobalParas.safeDis);
                    }
                    //给每个RGV分配任务
                    int j = 0;
                    while (j < GlobalParas.allocableRGVs.Count && curIndex < match.Count)
                    {
                        //判断是否是重复的RGV
                        for (int RMCIndex = 0; RMCIndex < RMCList.Length; RMCIndex++)
                        {
                            if (RMCList[RMCIndex].task!=null && RMCList[RMCIndex].RGV.ID == match.ElementAt(curIndex).Value.ID)
                            {
                                sameRGV = true;
                            }
                        }
                        //如果有相同的RGV，直接退出循环
                        if (!sameRGV)//分配
                        {
                            Task curTask = match.ElementAt(curIndex).Key;
                            RGV curRGV = match.ElementAt(curIndex).Value;
                            RMCList[j++] = new RGVMoveController(curTask, curRGV, GlobalParas.moveSpeed, GlobalParas.safeDis);
                            curIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    aDispatchFin = false;
                }
                //计算一波任务的总时间
                int aUnfinishNum = RMCList.Length;
                while (aUnfinishNum > 0)
                {
                    for (int RMCindex = 0; RMCindex < RMCList.Length; RMCindex++)
                    {
                        if (aUnfinishNum <= 0)
                        {
                            break;
                        }
                        if (RMCList[RMCindex].task == null)
                        {
                            aUnfinishNum--;
                        }
                        RMCList[RMCindex].SimuUpdate();
                    }
                    aCompleteTime += GlobalParas.frameTime;
                }
                completeTime += aCompleteTime;
            }
            return completeTime;
            //目前先无视已经分配了任务的RGV
            //现在产生的Match不是一次模拟计算完成的，而是有多次计算，因为调度的任务数目比RGV多
            //一次计算需要考虑没有任务的RGV，因为它们也会运动(因为防碰撞）
            //更新matchList的时机：每次有新任务加入时
            //从match中取任务到RMCList中
        }
        //根据RGV的ID返回全局变量allRGVs的RGV
        public RGV getRGV(int ID)
        {
            foreach (RGV rgv in GlobalParas.allRGVs)
            {
                if (rgv.ID == ID)
                    return rgv;
            }
            return null;
        }
    }
}
