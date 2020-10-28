using RESUPUB.EngineObjects;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESUPUB.Helpers
{
    static class TheMaths
    {
        public static float GetDistanceToPoint(Vector3 pointA, Vector3 pointB)
        {
            return (float)Math.Abs((pointA - pointB).Length());
        }
        public static float DistanceToOtherEntityInMetres(Local me, Entity other)
        {
            return GetDistanceToPoint(me.Position, other.Position) * 0.01905f;
        }

        public static Vector3 CalcAngle(SharpDX.Vector3 src, SharpDX.Vector3 dst)
        {
            Vector3 ret = new Vector3();
            Vector3 vDelta = src - dst;
            //vDelta = NormalizeAngles(vDelta);
            /* 	float Length2d( void ) const
	            {
		            return sqrtf( x * x + y * y );
	            } 
            */

            float fHyp = (float)Math.Sqrt((vDelta.X * vDelta.X) + (vDelta.Y * vDelta.Y));

            ret.X = RadToDeg((float)Math.Atan(vDelta.Z / fHyp));
            ret.Y = RadToDeg((float)Math.Atan(vDelta.Y / vDelta.X));

            if (vDelta.X >= 0.0f)
                ret.Y += 180.0f;
            return ret;
        }
        public static Vector3 NormalizeAngles(Vector3 v)
        {
            for (int i = 0; i < 3; i++)
            {
                if (v[i] < -180.0f) v[i] += 360.0f;
                if (v[i] > 180.0f) v[i] -= 360.0f;
            }
            return v;
        }

        public static float MakeDot(Vector3 i, Vector3 o)
        {
            return (i.X * o.X + i.Y * o.Y + i.Z * o.Z);
        }

        public static Vector2[] WorldToScreen(Matrix4x4 vMatrix, Size2 screenSize, params Vector3[] points)
        {
            Vector2[] worlds = new Vector2[points.Length];
            for (int i = 0; i < worlds.Length; i++)
                worlds[i] = WorldToScreen(vMatrix, screenSize, points[i]);
            return worlds;

        }
        public static Vector2 WorldToScreen(Matrix4x4 viewMatrix, Size2 screenSize, Vector3 point)
        {
            Vector2 returnVector = Vector2.Zero;

            float w = viewMatrix.M41 * point.X + viewMatrix.M42 * point.Y + viewMatrix.M43 * point.Z + viewMatrix.M44;
            if (w >= 0.01f)
            {
                float inverseWidth = 1f / w;
                returnVector.Y =
                    (screenSize.Height / 2f) -
                    (0.5f * (
                    (viewMatrix.M21 * point.X + viewMatrix.M22 * point.Y + viewMatrix.M23 * point.Z + viewMatrix.M24)
                    * inverseWidth)
                    * screenSize.Height + 0.5f);
                returnVector.X =
                    (screenSize.Width / 2f) +
                    (0.5f * (
                    (viewMatrix.M11 * point.X + viewMatrix.M12 * point.Y + viewMatrix.M13 * point.Z + viewMatrix.M14)
                    * inverseWidth)
                    * screenSize.Width + 0.5f);
            }
            return returnVector;
        }

        public static Vector3 ClampAngle(Vector3 qaAng)
        {

            if (qaAng.X > 89.0f && qaAng.X <= 180.0f)
                qaAng.X = 89.0f;

            while (qaAng.X > 180.0f)
                qaAng.X = qaAng.X - 360.0f;

            if (qaAng.X < -89.0f)
                qaAng.X = -89.0f;

            while (qaAng.Y > 180.0f)
                qaAng.Y = qaAng.Y - 360.0f;

            while (qaAng.Y < -180.0f)
                qaAng.Y = qaAng.Y + 360.0f;

            return qaAng;
        }

        public static float RadToDeg(float deg) { return (float)(deg * (180f / Math.PI)); }

        public static bool PointInCircle(Vector2 point, Vector2 circleCenter, float radius)
        {
            return Math.Sqrt(((circleCenter.X - point.X) * (circleCenter.X - point.X)) + ((circleCenter.Y - point.Y) * (circleCenter.Y - point.Y))) < radius;
        }

        public static bool WorldToScreen(Matrix4x4 _ViewMatrix, Vector3 _Position, out Vector3 _Result, float Width, float Height, float Left, float Top)
        {
            _Result = new Vector3(0, 0, 0);

            _Result.X = (_ViewMatrix.M11 * _Position.X) + (_ViewMatrix.M12 * _Position.Y) + (_ViewMatrix.M13 * _Position.Z) + _ViewMatrix.M14;
            _Result.Y = (_ViewMatrix.M21 * _Position.X) + (_ViewMatrix.M22 * _Position.Y) + (_ViewMatrix.M23 * _Position.Z) + _ViewMatrix.M24;
            _Result.Z = (_ViewMatrix.M41 * _Position.X) + (_ViewMatrix.M42 * _Position.Y) + (_ViewMatrix.M43 * _Position.Z) + _ViewMatrix.M44;

            if (_Result.Z < 0.01f)
                return false;

            float invw = 1.0f / _Result.Z;
            _Result.X *= invw;
            _Result.Y *= invw;

            float x = Width / 2;
            float y = Height / 2;

            x += 0.5f * _Result.X * Width + 0.5f;
            y -= 0.5f * _Result.Y * Height + 0.5f;

            _Result.X = x + Left;
            _Result.Y = y + Top;
            //Console.WriteLine("Last - X: " + _Result.X.ToString() + " - " + _Result.Y.ToString());
            return true;
        }
    }
}
