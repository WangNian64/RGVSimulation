using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Varibles;
namespace RGVSimulation
{
    //随机任务和RGV产生器，用于产生训练集
    public class TrainingSetCreator
    {
        //得到随机的Task和RGV池
        public static List<Task> randomTaskPool;
        public static List<RGV> randomRGVPool;
        //设置Task和RGV数目的上下限
        public static int RGV_MinNum;
        public static int RGV_MaxNum;
        public static int Task_MinNum;
        public static int Taks_MaxNum;
        public static void InitialRandomTaskPool()
        {

        }
        public static void InitialRandomRGVPool()
        {

        }
        //从初始化的任务和RGV池中得到随机的Task和RGV
        public static TaskRGVPair GetRandomTaskRGVDic(int size)
        {
            return new TaskRGVPair();
        }
        public static List<Dictionary<TaskRGVPair, DispatchResult>> GetResultList()
        {
            return null;
        }
        public static void DealResultList()
        {

        }
        //训练集的
        public static void GetTrainingSet(int sampleSize)
        {
            //输出包括：所有的task数组、RGV数组、最佳调度结果，三者构成一个sample
            InitialRandomTaskPool();
            InitialRandomRGVPool();
            //得到Task-RGV集合
            GetRandomTaskRGVDic(sampleSize);
            //得到所有Task-RGV最佳配对的结果
            List<Dictionary<TaskRGVPair, DispatchResult>> allDispatchList = GetResultList();
            //对结果进行处理，去除不要的信息
            DealResultList();
        }
    }
}
