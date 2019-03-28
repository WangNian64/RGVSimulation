using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Trailer;
using RGVSimulation.Tools;
namespace RGVSimulation.Varibles
{
    //小车的工作状态
    public enum WorkState
    {
        Empty = 0, WayToLoad = 1, Loading = 2, WayToUnload = 3, Unloading = 4
    }
    public class RGV
    {
        public int ID;
        public Vector2 pos;
        public WorkState workState;
        public double speed;

        public Edge edge;//小车所在边的信息
        public double angled;//若小车在曲线上，该变量表示小车在曲线上的角度  

        public RGV(int ID, Vector2 pos, WorkState workState, double speed, Edge edge, double angled)
        {
            this.ID = ID;
            this.pos = pos;
            this.workState = workState;
            this.edge = edge;
            this.angled = angled;
        }
        public override string ToString()
        {
            return "ID:" + ID + ", pos:" + pos + ", workState:" + workState
                + ", speed:" + speed;
        }
        public override bool Equals(object obj)
        {
            RGV rgv = obj as RGV;
            return rgv.ID == this.ID;
        }
        public RGV Clone()
        {
            if (this == null)
            {
                return null;
            }
            else
            {
                RGV RGVCopy = new RGV(
                    this.ID,
                    this.pos,
                    this.workState,
                    this.speed,
                    this.edge,
                    this.angled
                );
                return RGVCopy;
            }
        }
    }
}
