using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public struct Attitude
    {
        public float Yaw;
        public float Pitch;
        public float Roll;

        public Attitude(float yaw,float pitch,float roll)
        {
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
        }
    }

    public class Object3D : IDisposable,IRenderable,ICollidable,ICullable
    {
        #region Members
        protected string m_sName;
        protected Vector3 m_vPosition;
        protected Attitude m_vOrientation;
        protected Matrix m_Matrix;

        protected bool m_bCulled;
        protected float m_fRadius;

        #endregion

        #region Propertis
        public String Name
        {
            get { return m_sName; }
        }

        public virtual Vector3 Position
        {
            set { m_vPosition = value; }
            get { return m_vPosition; }
        }
        public virtual float X
        {
            set { m_vPosition.X = value; }
            get { return m_vPosition.X; }
        }
        public virtual float Y
        {
            set { m_vPosition.Y = value; }
            get { return m_vPosition.Y; }
        }
        public virtual float Z
        {
            set { m_vPosition.Z = value; }
            get { return m_vPosition.Z; }
        }

        public Attitude Orientation
        {
            set
            {
                m_vOrientation.Yaw = value.Yaw * (float)Math.PI / 180;
                m_vOrientation.Pitch = value.Pitch * (float)Math.PI / 180;
                m_vOrientation.Roll = value.Roll * (float)Math.PI / 180;
            }
            get { return m_vOrientation; }
        }
        public float Yaw
        {
            set { m_vOrientation.Yaw = value * (float)Math.PI / 180; }
            get { return m_vOrientation.Yaw * 180 / (float)Math.PI ; }
        }
        public float Pitch
        {
            set { m_vOrientation.Pitch = value * (float)Math.PI / 180; }
            get { return m_vOrientation.Pitch * 180 / (float)Math.PI; }
        }
        public float Roll
        {
            set { m_vOrientation.Roll = value * (float)Math.PI / 180; }
            get { return m_vOrientation.Roll * 180 / (float)Math.PI; }
        }

        public Matrix WorldMatrix
        {
            set { m_Matrix = value; }
            get
            {
                m_Matrix = Matrix.RotationYawPitchRoll(m_vOrientation.Yaw, m_vOrientation.Pitch, m_vOrientation.Roll);
                m_Matrix = m_Matrix * Matrix.Translation(m_vPosition);
                return m_Matrix;
            }
        }
        #endregion

        // Constructor
        public Object3D(string sName)
        {
            m_sName = sName;
            m_vPosition = new Vector3(0, 0, 0);
            m_vOrientation = new Attitude(0, 0, 0);
            m_Matrix = Matrix.Identity;
            m_bCulled = false;
            m_fRadius = 0;
        }

        #region Methodes
        #endregion

        #region Interfaces
        public virtual void Render(){}
        public virtual void Render(Camera cam, Matrix matWorld) { }
        public virtual void Update(float DeltaTime) { }
        public virtual bool IsCulled { get { return m_bCulled; } }
        public virtual bool Culled { set { m_bCulled = value; } }
        public Vector3 CenterOfMass { get { return m_vPosition; } }
        public virtual float BoundingRadius { get { return m_fRadius; } }
        public virtual bool CollidSphere(Object3D other)
        {
            return false;
        }
        public virtual bool CollidPolygon(Vector3 Point1, Vector3 Point2, Vector3 Point3)
        {
            return false;
        }
        public virtual void Dispose()
        {
        }
        #endregion
    }
}
