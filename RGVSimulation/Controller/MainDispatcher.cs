using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RGVSimulation.Varibles;
using RGVSimulation.Tools;
namespace RGVSimulation.Controller
{
    public class MainDispatcher
    {
        static RGV[] tempRGVs = new RGV[100];//辅助计算排列组合
        public static RGVMoveController[] RMCList;
        DispatchResult dispatchResult;

        public static int unfinishTaskNum;//未完成的任务数目
        public static int aUnfinishTaskNum;//一次分配未完成的任务数目
        public DispatchResult DispatchRGV()
        {
            InitialParas();
            if (TaskRGVNotEmpty())
            {
                //先得到Task的全排列
                List<Task[]> taskPermuList = new List<Task[]>();
                MathTool<Task>.GetPermutation(ref taskPermuList, GlobalParas.allocableTasks.ToArray(), 0, GlobalParas.allocableTasks.ToArray().Length);

                List<Dictionary<Task, RGV>> dispatchList = new List<Dictionary<Task, RGV>>();
                foreach (Task[] tasks in taskPermuList)
                {
                    getDispatchList(ref dispatchList, GlobalParas.allocableRGVs.ToArray(), tasks,
                    GlobalParas.allocableRGVs.Count - 1, tasks.Length - 1);
                }
                //对于已经分配了任务的RGV也要加进match(这里加的时机不对，因为任务是有先后级别的，后加不一定对)
                foreach (Dictionary<Task, RGV> match in dispatchList)
                {
                    foreach (KeyValuePair<Task, RGV> kvp in GlobalParas.UnAllocableMatch)
                    {
                        match.Add(kvp.Key.Clone(), kvp.Value.Clone());//进行深拷贝
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
        public void getDispatchList(ref List<Dictionary<Task, RGV>> matchList, RGV[] RGVs, Task[] tasks, int RGVNum, int taskNum)
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
                    for (j = tasks.Length - 1; j >= 0; j--)
                    {
                        tempMatch.Add(tasks[tasks.Length - 1 - j], tempRGVs[j]);
                    }
                    matchList.Add(tempMatch);
                }
            }
        }
        //得到matchList中的时间最小Match, 返回最小match对应的时间, count是每个match的车或任务的数目
        public double getMinMatch(List<Dictionary<Task, RGV>> matchList, ref Dictionary<Task, RGV> minMatch)
        {
            double[] times = new double[matchList.Count];//任务所需时间
            int index = 0;
            double minTime = GlobalParas.INFINITY;//最短时间
            minMatch = matchList[0];//最短时间的匹配
            foreach (Dictionary<Task, RGV> match in matchList)
            {
                //对match进行Clone
                Dictionary<Task, RGV> matchDeepCopy = new Dictionary<Task, RGV>();
                foreach (Task task in match.Keys)
                {
                    Task taskCopy = task.Clone();
                    RGV rgv = match[task].Clone();
                    matchDeepCopy.Add(taskCopy, rgv);
                }
                //由于Clone函数，导致后面的相同RGV有不同的状态
                times[index] = calcuTasksTime(matchDeepCopy);//求所有RGV完成所有任务的时间
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
        public double calcuTasksTime(Dictionary<Task, RGV> match)
        {
            double completeTime = 0.0f;
            double aCompleteTime = 0.0f;
            unfinishTaskNum = match.Count;
            aUnfinishTaskNum = 0;
            int curIndex = 0;
            bool aDispatchFin = true;

            int count1 = 0;
            //对alloRGV进行深拷贝
            List<RGV> alloRGVsDeepCopy = new List<RGV>();
            foreach (RGV rgv in GlobalParas.allocableRGVs)
            {
                alloRGVsDeepCopy.Add(rgv.Clone());
            }
            while (unfinishTaskNum > 0)
            {
                bool sameRGV = false;
                //一波结束，分配一波新的任务
                if (aDispatchFin == true)
                {
                    //初始化RMCList
                    RMCList = new RGVMoveController[alloRGVsDeepCopy.Count];
                    int i = 0;
                    //这里的RGV其实应该是AllRGVs！先不考虑
                    foreach (RGV rgv in alloRGVsDeepCopy)//调度所有的RGV，即使有些没有被分配任务
                    {
                        RMCList[i++] = new RGVMoveController(null, rgv, GlobalParas.moveSpeed, GlobalParas.safeDis);
                    }
                    //给每个RGV分配任务
                    int RGVIndex = 0;
                    while (RGVIndex < alloRGVsDeepCopy.Count && curIndex < match.Count)
                    {
                        //判断是否是重复的RGV
                        for (int RMCIndex = 0; RMCIndex < RMCList.Length; RMCIndex++)
                        {
                            if (RMCList[RMCIndex].task!=null && RMCList[RMCIndex].RGV.ID == match.ElementAt(curIndex).Value.ID)
                            {
                                sameRGV = true;
                            }
                        }
                        //分配
                        if (!sameRGV)
                        {
                            Task curTask = match.ElementAt(curIndex).Key;
                            RGV curRGV = match.ElementAt(curIndex).Value;
                            foreach (RGVMoveController RMC in RMCList)
                            {
                                if (RMC.RGV.ID == curRGV.ID)
                                {
                                    RMC.task = curTask;
                                }
                            }
                            RGVIndex++;
                            curIndex++;
                            aUnfinishTaskNum++;
                        }
                        else//如果有相同的RGV，直接退出循环（一个RGV同时只能调度一个任务）
                        {
                            break;
                        }
                    }
                    aDispatchFin = false;
                }
                //计算一波任务的总时间
                while (aUnfinishTaskNum > 0)
                {
                    for (int RMCindex = 0; RMCindex < RMCList.Length; RMCindex++)
                    {
                        RMCList[RMCindex].SimuUpdate();
                        if (aUnfinishTaskNum <= 0)
                        {
                            aDispatchFin = true;
                            break;
                        }
                    }
                    aCompleteTime += GlobalParas.frameTime;
                }
                completeTime += aCompleteTime;
                aCompleteTime = 0.0f;
            }
            return completeTime;
            //目前先无视已经分配了任务的RGV
            //现在产生的Match不是一次模拟计算完成的，而是有多次计算，因为调度的任务数目比RGV多
            //一次计算需要考虑没有任务的RGV，因为它们也会运动(因为防碰撞）
            //更新matchList的时机：每次有新任务加入时
            //从match中取任务到RMCList中
        }
        public RGV getRGV(int ID)
        {
            foreach (RGV rgv in GlobalParas.allocableRGVs)
            {
                if (rgv.ID == ID)
                    return rgv;
            }
            return null;
        }
    }
}
