using System;
using System.Collections.Generic;
using System.Text;
using RGVSimulation.Varibles;
namespace RGVSimulation.Tools
{
    public class Vector2
    {
        public double x;
        public double y;
        public Vector2() : this(0, 0)
        {

        }
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        //+
        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new ArgumentNullException("Vector2不可为null!");
            }
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        //向量有关运算
        //-
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new ArgumentNullException("Vector2不可为null!");
            }
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }
        //数乘
        public static Vector2 operator *(Vector2 v, double d)
        {
            return new Vector2(v.x * d, v.y * d);
        }
        //点积
        public static double Dot(Vector2 v1, Vector2 v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new ArgumentNullException("Vector2不可为null!");
            }
            return v1.x * v2.x + v1.y * v2.y;
        }
        //两个点的中心点
        public static Vector2 CenterVector(Vector2 v1, Vector2 v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new ArgumentNullException("Vector2不可为null!");
            }
            return new Vector2((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
        }
        
        public static double Distance(Vector2 v1, Vector2 v2)
        {
            if (v1 == null || v2 == null)
            {
                throw new ArgumentNullException("Vector2不可为null!");
            }
            double d1 = Math.Pow((v1.x - v2.x), 2);
            double d2 = Math.Pow((v1.y - v2.y), 2);
            return Math.Sqrt(d1 + d2);
        }
        //单位化一个向量
        public static Vector2 normalize(Vector2 v)
        {
            if (v == null)
                throw new ArgumentException("Vector2不可为null!");
            if (v.x==0 && v.y==0)
                throw new ArgumentException("Vector2不可为零向量");
            double norm = (double)Math.Sqrt(Math.Pow(v.x, 2) + Math.Pow(v.y, 2));
            v = new Vector2(v.x/norm, v.y/norm);
            return v;
        }
        //判断两点之间是否足够接近
        public static bool IsClose(Vector2 v1, Vector2 v2, double speed)
        {
            double dist = Vector2.Distance(v1, v2);
            return dist <= speed * GlobalParas.frameTime;
        }

        public override string ToString()
        {
            return "x:" + x + ", y:" + y;
        }
    }
}
