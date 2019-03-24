using System;
using RGVSimulation.Varibles;
using RGVSimulation.Trailer;
using RGVSimulation.Tools;
namespace RGVSimulation.Controller
{
    public class RGVMoveController
    {
        public RGV RGV;//车的信息
        public Task task;//车对应的任务

        public double moveSpeed;//运动时的速度
        public double safeDis;//小车之间的安全距离

        private double loadAccumuTime;//累计取货时间
        private double unloadAccumuTime;//累计卸货时间
        private ShortPath shortPath;//小车到某个点的最短路径数据

        public bool speedChangeable;//速度是否可变
        public RGVMoveController(Task task, RGV RGV, double moveSpeed,
            double safeDis)
        {
            this.RGV = RGV;
            this.task = task;
            this.moveSpeed = moveSpeed;
            this.safeDis = safeDis;
            loadAccumuTime = unloadAccumuTime = 0;
            speedChangeable = true;
        }
        //模拟update函数
        public void SimuUpdate()
        {
            ClearShortPath();
            SimuCarMovement();
            SimuAvoidCrash();
        }
        //清空shortPath
        public void ClearShortPath()
        {
            if (shortPath != null)
            {
                if (shortPath.vertexList[0].index == shortPath.vertexList[1].index)
                {
                    shortPath = null;
                }
            }
        }
        //模拟小车运动
        public void SimuCarMovement()
        {
            if (task != null)//有任务
            {
                switch (RGV.workState)
                {
                    case WorkState.Empty:
                        RGV.workState = WorkState.WayToLoad;
                        shortPath = GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex1.index,
                            task.loadEdge.vertex2.index];
                        if (speedChangeable)
                            RGV.speed = moveSpeed;
                        break;
                    case WorkState.WayToLoad:
                        gotoLoadPoint();//运动到取货点
                        break;
                    case WorkState.Loading:
                        loadingGoods();//取货
                        break;
                    case WorkState.WayToUnload:
                        gotoUnloadPoint();//运动到卸货点
                        break;
                    case WorkState.Unloading:
                        unloadingGoods();//卸货
                        break;
                    default:
                        break;
                }
            }
            else//此时车的状态一定是empty
            {
                switch (RGV.workState)
                {
                    case WorkState.Empty:
                        break;
                    case WorkState.WayToLoad:
                        RGV.workState = WorkState.Empty;
                        RGV.speed = 0;
                        shortPath = null;
                        break;
                    case WorkState.Loading:
                        break;
                    case WorkState.WayToUnload:
                        break;
                    case WorkState.Unloading:
                        break;
                    default:
                        break;
                }
                if (shortPath != null)
                {
                    int nextVertexNum = RGV.edge.vertex2.index;//得到车要运动到的下一个节点
                    Vertex nextVertex = GlobalParas.trailerGraph.vertexes[nextVertexNum];
                    if (speedChangeable)
                        RGV.speed = moveSpeed;
                    gotoVertex(nextVertex);
                }
            }
        }
        //模拟运动到取货点
        public void gotoLoadPoint()
        {
            if (speedChangeable)
                RGV.speed = moveSpeed;
            shortPath = GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex1.index,
                task.loadEdge.vertex1.index];
            //若小车已经在最后一条边上
            if (RGV.edge.vertex1.index == task.loadEdge.vertex1.index)
            {
                //判断是否已经到达取货点
                if (RGV.pos == task.loadPos)
                {
                    RGV.workState = WorkState.Loading;
                    RGV.speed = 0.0f;
                }
                else
                {
                    gotoPos(task.loadPos);
                }
            }
            else
            {
                //得到车的下一个节点
                int nextVertexNum = RGV.edge.vertex2.index;
                Vertex nextVertex = GlobalParas.trailerGraph.vertexes[nextVertexNum];
                gotoVertex(nextVertex);
            }
        }
        //模拟运动到卸货点
        public void gotoUnloadPoint()
        {
            if (speedChangeable)
                RGV.speed = moveSpeed;
            shortPath = GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex1.index,
                task.unloadEdge.vertex1.index];
            //若小车已经在最后一条边上
            if (RGV.edge.vertex1.index == task.unloadEdge.vertex1.index)
            {
                //判断是否已经到达卸货点
                if (RGV.pos == task.unloadPos)
                {
                    RGV.workState = WorkState.Unloading;
                    RGV.speed = 0.0f;
                }
                else
                {
                    gotoPos(task.unloadPos);
                }
            }
            else
            {
                //得到车的下一个节点
                int nextVertexNum = RGV.edge.vertex2.index;
                Vertex nextVertex = GlobalParas.trailerGraph.vertexes[nextVertexNum];
                gotoVertex(nextVertex);
            }
        }
        //运动到某个点
        public void gotoVertex(Vertex vertex)
        {
            if (!Vector2.IsClose(vertex.pos, RGV.pos, RGV.speed))//要考虑速度
            {
                //判断车在直线还是曲线
                if (RGV.edge is CurveEdge)//曲线 
                {
                    //模拟旋转运动
                    SimuRotateAround((RGV.edge as CurveEdge).radius, (RGV.edge as CurveEdge).center, RGV.speed * GlobalParas.frameTime);
                }
                else if (RGV.edge is StraightEdge)//直线
                {
                    //模拟直线运动
                    SimuMoveTowards(vertex.pos, RGV.speed * GlobalParas.frameTime);
                }
                else { }
            }
            else//已经到达该点
            {
                RGV.pos = vertex.pos;
                shortPath = GlobalParas.trailerGraph.shortPathsMap[vertex.index, shortPath.vertexList[shortPath.vertexList.Count - 1].index];
                if (RGV.workState == WorkState.WayToLoad && vertex.index == task.loadEdge.vertex1.index)
                {
                    RGV.edge = GlobalParas.trailerGraph.adjMatrix[vertex.index, task.loadEdge.vertex2.index];
                }
                else if (RGV.workState == WorkState.WayToUnload && vertex.index == task.unloadEdge.vertex1.index)
                {
                    RGV.edge = GlobalParas.trailerGraph.adjMatrix[vertex.index, task.unloadEdge.vertex2.index];
                }
                else
                {
                    RGV.edge = GlobalParas.trailerGraph.adjMatrix[shortPath.vertexList[0].index, shortPath.vertexList[1].index];
                }

                if (RGV.edge is CurveEdge)
                {
                    RGV.angled = (RGV.edge as CurveEdge).startDeg;
                }
                return;
            }
        }
        //运动到某个位置
        public void gotoPos(Vector2 position)
        {
            if (!Vector2.IsClose(position, RGV.pos, RGV.speed))
            {
                //判断车在直线还是曲线
                if (RGV.edge is CurveEdge)
                {
                    //模拟旋转运动
                    SimuRotateAround((RGV.edge as CurveEdge).radius, (RGV.edge as CurveEdge).center, RGV.speed * GlobalParas.frameTime);
                }
                else if (RGV.edge is StraightEdge)
                {
                    //模拟直线运动
                    SimuMoveTowards(position, RGV.speed * GlobalParas.frameTime);
                }
                else { }
            }
            else//说明已经到达该点
            {
                RGV.pos = position;
                //退出该方法
                return;
            }
        }
        //取货
        public void loadingGoods()
        {
            if (loadAccumuTime >= task.loadTime)
            {
                RGV.workState = WorkState.WayToUnload;
                speedChangeable = true;
                RGV.speed = moveSpeed;
                //现在表示从取货点到卸货点的路径
                shortPath = GlobalParas.trailerGraph.shortPathsMap[task.loadEdge.vertex1.index,
                    task.unloadEdge.vertex1.index];
                loadAccumuTime = 0;
            }
            loadAccumuTime += GlobalParas.frameTime;
        }
        //卸货
        public void unloadingGoods()
        {
            if (unloadAccumuTime >= task.unloadTime)
            {
                RGV.workState = WorkState.Empty;
                speedChangeable = true;
                RGV.speed = 0.0f;
                task = null;//清空任务
                MainDispatcher.unfinishNum--;//总任务数目-1
                shortPath = null;
                unloadAccumuTime = 0;
            }
            unloadAccumuTime += GlobalParas.frameTime;
        }
        //模拟小车的旋转运动
        public void SimuRotateAround(double curveRadius, Vector2 rotateCenter, double lineSpeed)
        {
            double angularSpeed = lineSpeed / curveRadius;//计算角速度 radian
            //deg是角度，radian是弧度
            //累计的角度（每次进入曲线轨道，都要给小车一个新的初始角度）
            RGV.angled += (MathTool<double>.Rad2Deg * angularSpeed) % 360;
            double posX = rotateCenter.x + curveRadius * MathF.Sin((float)(RGV.angled * MathTool<double>.Deg2Rad));
            double posy = rotateCenter.y + curveRadius * MathF.Cos((float)(RGV.angled * MathTool<double>.Deg2Rad));
            //更新pos
            RGV.pos = new Vector2(posX, posy);
        }
        //模拟小车的直线运动
        public void SimuMoveTowards(Vector2 endPos, double speed)
        {
            if (Vector2.IsClose(RGV.pos, endPos, RGV.speed))
            {
                RGV.pos = endPos;
                return;
            }
            Vector2 moveVector2 = Vector2.normalize(endPos - RGV.pos) * speed;
            RGV.pos += moveVector2;
        }
        //模拟碰撞检测 并 防止碰撞
        public void SimuAvoidCrash()
        {
            bool noOther = true;
            if (MainDispatcher.RMCList != null)
            {
                foreach (RGVMoveController scc in MainDispatcher.RMCList)
                {
                    if (scc.RGV.ID != RGV.ID)//是其他小车
                    {
                        noOther = false;
                        if (Vector2.Distance(scc.RGV.pos, RGV.pos) <= safeDis)//若其他小车距离本车过近,修改速度
                        {
                            SimuChangeSpeed(scc.RGV);//修改小车速度
                        }
                    }
                }
            }
            if (noOther)
            {
                SpeedRecover();
            }
        }
        //判断两车前后关系
        public bool isFront(RGV targetRGV)
        {
            //本车到目标车的路径距离，目标车到本车的路径距离
            double s2t, t2s;
            s2t = t2s = 0.0f;
            //车所在的边
            Vertex selfVertex1 = GlobalParas.trailerGraph.vertexes[RGV.edge.vertex1.index];
            Vertex selfVertex2 = GlobalParas.trailerGraph.vertexes[RGV.edge.vertex2.index];
            Vertex targetVertex1 = GlobalParas.trailerGraph.vertexes[targetRGV.edge.vertex1.index];
            Vertex targetVertex2 = GlobalParas.trailerGraph.vertexes[targetRGV.edge.vertex2.index];
            if (targetRGV.edge.Equals(RGV.edge))//2车在同一条边上
            {
                t2s = Vector2.Distance(targetRGV.pos, targetVertex2.pos);
                s2t = Vector2.Distance(RGV.pos, selfVertex2.pos);
            }
            else//不在同一条边，分别计算两车经过轨道到达彼此的距离，距离短的那个在后面
            {
                s2t += Vector2.Distance(targetRGV.pos, targetVertex2.pos);
                t2s += Vector2.Distance(RGV.pos, selfVertex2.pos);

                s2t += GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex2.index, targetRGV.edge.vertex1.index].length;
                t2s += GlobalParas.trailerGraph.shortPathsMap[targetRGV.edge.vertex2.index, RGV.edge.vertex1.index].length;

                s2t += Vector2.Distance(targetRGV.pos, targetVertex1.pos);
                t2s += Vector2.Distance(RGV.pos, selfVertex1.pos);
            }
            return s2t <= t2s ? true : false;
        }
        //速度恢复
        public void SpeedRecover()
        {
            if (RGV.workState != WorkState.Loading && RGV.workState != WorkState.Unloading)
            {
                speedChangeable = true;
            }
            //if (RGV.workState == WorkState.Empty)
            //{
            //    if (shortPath == null)
            //    {
            //        shortPath = GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex2.index, RGV.edge.vertex1.index];
            //    }
            //}
        }
        //模拟在碰撞时修改小车速度
        public void SimuChangeSpeed(RGV targetRGV)
        {
            if (speedChangeable)
            {
                bool isfront = isFront(targetRGV);
                if (isfront)
                {
                    if (shortPath == null)//只有可能在empty时发生，此时前车需要先获得路径
                    {
                        shortPath = GlobalParas.trailerGraph.shortPathsMap[RGV.edge.vertex2.index,
                            RGV.edge.vertex1.index];
                    }
                    RGV.speed = moveSpeed;
                }
                else
                {
                    RGV.speed = targetRGV.speed;
                    speedChangeable = false;
                }
            }
        }
    }
}
