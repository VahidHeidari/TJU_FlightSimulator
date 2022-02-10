using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class Camera
    {
        public Vector3 Position = new Vector3(0, 1, -1);
        public Vector3 Target = new Vector3(0, 0, 0);
        public Vector3 Up = new Vector3(0, 1, 0);

        public Matrix View = Matrix.Identity;
        public Matrix Proj = Matrix.Identity;
        public Quaternion qOrientation = Quaternion.Identity;

        public float fNearPlane = 0.1f;
        public float fFareplane = 1000;
        public float fFOV = (float)Math.PI / 4;
        public float fAspectRatio = 1.33f;

        public Camera()
        {
            View = Matrix.LookAtLH(Position, Target, Up);
            Proj = Matrix.PerspectiveFovLH(fFOV, fAspectRatio, fNearPlane, fFareplane);
        }

        public void SetProjection()
        {
            Proj = Matrix.PerspectiveFovLH(fFOV, fAspectRatio, fNearPlane, fFareplane);
        }

        public void MoveRight(float Distance)
        {
            Vector3 NewPos = new Vector3(Target.X - Position.X, Target.Y - Position.Y, Target.Z - Position.Z);
            NewPos = Vector3.Cross(NewPos, Up);
            NewPos.Normalize();
            NewPos *= Distance;
            Position += NewPos;
            Target += NewPos;
        }

        public void ZoomForward(float Distance)
        {
            Vector3 NewPos = new Vector3(Position.X - Target.X, Position.Y - Target.Y, Position.Z - Target.Z);
            if (NewPos.Length() > 1 || Distance > 0)
            {
                NewPos.Normalize();
                NewPos *= Distance;
                if ((NewPos + Position).Length() > 1)
                    Position += NewPos;
            }
        }

        public void MoveForward(float Distance)
        {
            Vector3 dir = new Vector3(Target.X - Position.X, 0, Target.Z - Position.Z);
            dir.Normalize();
            dir *= Distance;
            Position += dir;
            Target += dir;
        }

        /// <summary>
        /// Rotate Around Position
        /// </summary>
        /// <param name="Deg"></param>
        public void RotateTargetYaw(float Deg)
        {
            Vector3 NewPos = new Vector3(Target.X - Position.X, Target.Y - Position.Y, Target.Z - Position.Z);

            NewPos.TransformCoordinate(Matrix.RotationY(Geometry.DegreeToRadian(Deg)));
            NewPos += Position;
            Target = NewPos;
        }
        /// <summary>
        /// Rotate Around Position
        /// </summary>
        /// <param name="Deg">Angle of Rotation in Degree</param>
        public void RotateTargetPitch(float Deg)
        {
            Vector3 NewPos = new Vector3(Target.X - Position.X, Target.Y - Position.Y, Target.Z - Position.Z);
            Vector3 axis = Vector3.Cross(NewPos, Up);
            NewPos.TransformCoordinate(Matrix.RotationAxis(axis, Geometry.DegreeToRadian(Deg)));
            NewPos += Position;
            Target = NewPos;
        }

        /// <summary>
        /// Rotate Around target
        /// </summary>
        /// <param name="Deg"></param>
        public void RotatePosYaw(float Deg)
        {

            Vector3 NewPos = new Vector3(Position.X - Target.X, Position.Y - Target.Y, Position.Z - Target.Z);
            NewPos.TransformCoordinate(Matrix.RotationY(Geometry.DegreeToRadian(Deg)));
            NewPos += Target;
            Position = NewPos;
        }
        /// <summary>
        /// Rotate Around Target
        /// </summary>
        /// <param name="Deg"></param>
        public void RotatePosPitch(float Deg)
        {

            Vector3 NewPos = new Vector3(Position.X - Target.X, Position.Y - Target.Y, Position.Z - Target.Z);
            Vector3 axis = Vector3.Cross(NewPos,Up);
            NewPos.TransformCoordinate(Matrix.RotationAxis(axis, Geometry.DegreeToRadian(Deg)));
            NewPos += Target;
            Position = NewPos;
        }

        public void Render(Device Graphic)
        {
            View = Matrix.LookAtLH(Position, Target, Up);
            Graphic.Transform.Projection = Proj;
            Graphic.Transform.View = View;
        }

    }
}
