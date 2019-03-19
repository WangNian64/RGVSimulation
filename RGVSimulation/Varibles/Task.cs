using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Trailer;
using RGVSimulation.Tools;
namespace RGVSimulation.Varibles
{
    public class Task
    {
        public int ID;
        public Vector2 loadPos;
        public Vector2 unloadPos;
        public Edge loadEdge;
        public Edge unloadEdge;
        public double loadTime = 1.0f;
        public double unloadTime = 1.0f;
        public Task(int ID, Vector2 loadPos, Edge loadEdge, 
            Vector2 unloadPos, Edge unloadEdge, double loadTime, double unloadTime)
        {
            this.ID = ID;
            this.loadPos = loadPos;
            this.unloadPos = unloadPos;
            this.loadEdge = loadEdge;
            this.unloadEdge = unloadEdge;
            this.loadTime = loadTime;
            this.unloadTime = unloadTime;
        }
        public Task Clone()
        {
            if (this == null)
                return null;
            else
            {
                Task task = new Task(
                    this.ID,
                    this.loadPos,
                    this.loadEdge,
                    this.unloadPos,
                    this.unloadEdge,
                    this.loadTime,
                    this.unloadTime
                );
                return task;
            }
        }
    }
}
