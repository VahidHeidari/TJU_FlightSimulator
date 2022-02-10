using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    class SphereDummy
    {
        CustomVertex.PositionColored[] dummy;
        int[] index;

        int numOfCircles = 12;
        Vector3 pos;
        Vector3 angel;
        Vector3 scale;
        float radius = 1;
        Matrix matWorld;

        public float Radius
        {
            set
            {
                scale.X = value;
                scale.Y = value;
                scale.Z = value;
                SetMatrix();
            }
            get { return radius; }
        }
        public Vector3 Position
        {
            set
            {
                pos = value;
                SetMatrix();
            }
            get { return pos; }
        }
        public float X
        {
            set
            {
                pos.X = value;
                SetMatrix();
            }
            get { return pos.X; }
        }
        public float Y
        {
            set
            {
                pos.Y = value;
                SetMatrix();
            }
            get { return pos.Y; }
        }
        public float Z
        {
            set
            {
                pos.Z = value;
                SetMatrix();
            }
            get { return pos.Z; }
        }

        public Vector3 Angle
        {
            set
            {
                angel = value;
                angel.X = Geometry.DegreeToRadian(angel.X);
                angel.Y = Geometry.DegreeToRadian(angel.Y);
                angel.Z = Geometry.DegreeToRadian(angel.Z);
                SetMatrix();
            }
            get
            {
                Vector3 angle;

                angle.X = Geometry.RadianToDegree(angel.X);
                angle.Y = Geometry.RadianToDegree(angel.Y);
                angle.Z = Geometry.RadianToDegree(angel.Z);

                return angle;
            }
        }
        public float Pitch
        {
            set
            {
                angel.X = Geometry.DegreeToRadian(value);
                SetMatrix();
            }
            get { return Geometry.RadianToDegree(angel.X); }
        }
        public float Yaw
        {
            set
            {
                angel.Y = Geometry.DegreeToRadian(value);
                SetMatrix();
            }
            get { return Geometry.RadianToDegree(angel.Y); }
        }
        public float Roll
        {
            set
            {
                angel.Z = Geometry.DegreeToRadian(value);
                SetMatrix();
            }
            get { return Geometry.RadianToDegree(angel.Z); }
        }

        public Vector3 Scale
        {
            set
            {
                scale = value;
                SetMatrix();
            }
            get { return scale; }
        }
        public float ScaleX
        {
            set
            {
                scale.X = value;
                SetMatrix();
            }
            get { return scale.X; }
        }
        public float ScaleY
        {
            set
            {
                scale.Y = value;
                SetMatrix();
            }
            get { return scale.Y; }
        }
        public float ScaleZ
        {
            set
            {
                scale.Z = value;
                SetMatrix();
            }
            get { return scale.Z; }
        }

        public SphereDummy(Color color)
        {
            dummy = new CustomVertex.PositionColored[12];
            index = new int[14 * 2];
            for (int i = 0; i < 12; ++i)
            {
                dummy[i].Position = new Vector3(0, 0.2f * (float)Math.Cos(i * (Math.PI / 12 * 2)), 0.2f * (float)Math.Sin(i * Math.PI / 12 * 2));
                dummy[i].Color = color.ToArgb();

                index[i * 2 + 0] = i;
                index[i * 2 + 1] = i + 1;
            }
            index[23] = 0;
            index[24] = 0; index[25] = 6;
            index[26] = 3; index[27] = 9;
        }

        public SphereDummy(Color color,int NumOfCircles)
        {
            numOfCircles = NumOfCircles;
            dummy = new CustomVertex.PositionColored[numOfCircles];
            index = new int[(numOfCircles + 2) * 2];
            for (int i = 0; i < numOfCircles; ++i)
            {
                dummy[i].Position = new Vector3(0, 0.2f * (float)Math.Cos(i * (Math.PI / numOfCircles * 2)), 0.2f * (float)Math.Sin(i * Math.PI / numOfCircles * 2));
                dummy[i].Color = color.ToArgb();

                index[i * 2 + 0] = i;
                index[i * 2 + 1] = i + 1;
            }
            index[numOfCircles - 5] = 0;
            index[numOfCircles - 4] = 0; index[numOfCircles - 3] = numOfCircles / 2;
            index[numOfCircles - 2] = numOfCircles / 4; index[numOfCircles - 1] = numOfCircles * 3 / 4;
        }

        void SetMatrix()
        {
            matWorld = Matrix.Scaling(scale) * Matrix.RotationYawPitchRoll(angel.X, angel.Y, angel.Z) * Matrix.Translation(pos);
        }

        public void Render(Device Graphic)
        {
            Graphic.SetTexture(0, null);
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.Transform.World = matWorld;
            Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, dummy.Length, index.Length / 2, index, false, dummy);

            Graphic.Transform.World = Matrix.RotationYawPitchRoll(3.14f / 2, 0, 0) * matWorld;
            Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, dummy.Length, index.Length / 2, index, false, dummy);

            Graphic.Transform.World = Matrix.RotationYawPitchRoll(0, 0, 3.14f / 2) * matWorld;
            Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, dummy.Length, index.Length / 2, index, false, dummy);

            for (int i = 1; i < numOfCircles; ++i)
            {
                Graphic.Transform.World = Matrix.RotationY(2 * 3.14f * i / 12) * matWorld;
                Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, dummy.Length, index.Length / 2 - 4, index, false, dummy);
            }
        }
    }
}
