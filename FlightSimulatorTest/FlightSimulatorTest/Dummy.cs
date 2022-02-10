using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    class Dummy
    {
        CustomVertex.PositionColored[] dummy;
        int[] index;

        Vector3 pos;
        Vector3 angel;
        Vector3 scale;
        Matrix matWorld;

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

        public Dummy(Color color)
        {

            dummy = new CustomVertex.PositionColored[8];
            index = new int[12 * 2];

            for (int i = 0; i < dummy.Length; ++i)
                dummy[i].Color = color.ToArgb();

            //Top Front Right
            dummy[0].X = 0.5f;
            dummy[0].Y = 0.5f;
            dummy[0].Z = 0.5f;

            //Top Back Right
            dummy[1].X = 0.5f;
            dummy[1].Y = 0.5f;
            dummy[1].Z = -0.5f;

            //Top Back Left
            dummy[2].X = -0.5f;
            dummy[2].Y = 0.5f;
            dummy[2].Z = -0.5f;

            //Top Front Left
            dummy[3].X = -0.5f;
            dummy[3].Y = 0.5f;
            dummy[3].Z = 0.5f;

            //Bottom Front Right
            dummy[4].X = 0.5f;
            dummy[4].Y = -0.5f;
            dummy[4].Z = 0.5f;

            //Bottom Back Right
            dummy[5].X = 0.5f;
            dummy[5].Y = -0.5f;
            dummy[5].Z = -0.5f;

            //Bottom Back Left
            dummy[6].X = -0.5f;
            dummy[6].Y = -0.5f;
            dummy[6].Z = -0.5f;

            //Bottom Front Left
            dummy[7].X = -0.5f;
            dummy[7].Y = -0.5f;
            dummy[7].Z = 0.5f;

            // Top Face
            index[0] = 0;
            index[1] = 1;
            index[2] = 1;
            index[3] = 2;
            index[4] = 2;
            index[5] = 3;
            index[6] = 3;
            index[7] = 0;

            //Bottom Face
            index[8] = 4;
            index[9] = 5;
            index[10] = 5;
            index[11] = 6;
            index[12] = 6;
            index[13] = 7;
            index[14] = 7;
            index[15] = 4;

            //Side Faces
            index[16] = 0;
            index[17] = 4;
            index[18] = 1;
            index[19] = 5;
            index[20] = 2;
            index[21] = 6;
            index[22] = 3;
            index[23] = 7;

            matWorld = Matrix.Identity;
            pos = new Vector3(0, 0, 0);
            angel = new Vector3(0, 0, 0);
            scale = new Vector3(1, 1, 1);
        }

        void SetMatrix()
        {
            matWorld = Matrix.Scaling(scale) * Matrix.RotationYawPitchRoll(angel.X,angel.Y,angel.Z)*Matrix.Translation(pos);
        }
        public void Render(Device Graphic)
        {
            Graphic.Clear(ClearFlags.ZBuffer, Color.Black, 1, 0);
            Graphic.SetTexture(0, null);
            Graphic.Transform.World = matWorld;
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, index.Length, index.Length / 2, index, false, dummy);
        }
        public void Render(Device Graphic,Matrix MatWorld)
        {
            Graphic.Clear(ClearFlags.ZBuffer, Color.Black, 1, 0);
            Graphic.SetTexture(0, null);
            Graphic.Transform.World = MatWorld;
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.DrawIndexedUserPrimitives(PrimitiveType.LineList, 0, index.Length, index.Length / 2, index, false, dummy);
        }
    }
}
