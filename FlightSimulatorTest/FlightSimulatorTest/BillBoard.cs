using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class BillBoard:Object3D
    {
        public bool culled = true;

        protected VertexBuffer m_vertexbuffer;
        protected Texture m_texture;

        protected Matrix m_matRotation;

        void SetTexture(Texture Texture)
        {
            m_texture = Texture;
        }

        void CreateBuffer(float width, float Height, Device Graphic)
        {
            CustomVertex.PositionTextured[] verts = new CustomVertex.PositionTextured[4];
            verts[0].Position = new Vector3(-width / 2, 0, 0);//-Height / 2
            verts[0].Tu = 0; verts[0].Tv = 1;
            verts[1].Position = new Vector3(-width / 2, Height, 0);// / 2
            verts[1].Tu = 0; verts[1].Tv = 0;
            verts[2].Position = new Vector3(width / 2, Height, 0);// / 2
            verts[2].Tu = 1; verts[2].Tv = 0;
            verts[3].Position = new Vector3(width / 2, 0, 0);//-Height / 2
            verts[3].Tu = 1; verts[3].Tv = 1;

            m_vertexbuffer = new VertexBuffer(
                typeof(CustomVertex.PositionTextured),
                4,
                Graphic,
                Usage.None,
                CustomVertex.PositionTextured.Format,
                Pool.Managed);

            //GraphicsStream gStream;
            //gStream = m_vertexbuffer.Lock(0, 0, LockFlags.None);
            //gStream.Write(verts);
            //m_vertexbuffer.Unlock();
            m_vertexbuffer.SetData(verts, 0, LockFlags.None);
        }

        public BillBoard(string Name,float Width,float Height,Texture Texture, Device Graphic)
            :base(Name)
        {
            m_matRotation = Matrix.Identity;
            m_vPosition = Vector3.Empty;

            SetTexture(Texture);
            CreateBuffer(Width, Height, Graphic);
        }
        public BillBoard(string Name,float Width,float Height,Texture Texture, Device Graphic,Vector3 Pos)
            :base (Name)
        {
            m_matRotation = Matrix.Identity;
            m_vPosition = Pos;

            SetTexture(Texture);
            CreateBuffer(Width, Height, Graphic);
        }
        public BillBoard(string Name,float Width, float Height, string TexturePath, Device Graphic)
            :base(Name)
        {
            m_matRotation = Matrix.Identity;
            SetTexture(TextureLoader.FromFile(Graphic, TexturePath));
            CreateBuffer(Width,Height,Graphic);
        }

        public void SetRotationMatrix(Camera cam)
        {
            Vector3 dir = cam.Position - cam.Target;//m_vPosition;
            float angle = (float)Math.Atan(dir.X / dir.Z);
            if (dir.Z > 0)
                angle = (float)Math.PI + angle;
            m_matRotation = Matrix.RotationY(angle);
        }
        private void Render(Device Graphic)
        {
            if (m_vertexbuffer == null || m_texture == null || culled)
                return;
            Render(Graphic, true);
        }

        private void Render(Device Graphic,bool cull)
        {
            if (m_vertexbuffer == null || m_texture == null)
                return;
            if (culled)
                return;

            Graphic.VertexFormat = CustomVertex.PositionTextured.Format;
            Graphic.SetTexture(0, m_texture);
            Graphic.Transform.World = m_matRotation * Matrix.Translation(m_vPosition);
            Graphic.SetStreamSource(0, m_vertexbuffer, 0);
            Graphic.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);

            if(cull)
                culled = true;
        }
        public void Render(Device Graphic, Vector3 Pos,Camera cam)
        {
            m_vPosition = Pos;
            SetRotationMatrix(cam);
            Render(Graphic);
        }
        public void Render(Device Graphic, Vector3 Pos, Camera cam,bool cull)
        {
            m_vPosition = Pos;
            SetRotationMatrix(cam);
            Render(Graphic , cull);
        }

        public void Render(Device Graphic, Camera cam)
        {
            SetRotationMatrix(cam);
            Render(Graphic);
        }
        public void Render(Device Graphic, Vector3[] vects, Camera cam)
        {
            if (vects == null)
                return;
            for (int i = 0; i < vects.Length; ++i)
            {
                Render(Graphic, vects[i], cam);
            }
        }
        public override void Render(Camera cam, Matrix matWorld)
        {
            if (m_vertexbuffer == null || m_texture == null)
                return;
            //if (culled)
            //    return;
            SetRotationMatrix(cam);
            Main.MainClass.Graphic.VertexFormat = CustomVertex.PositionTextured.Format;
            Main.MainClass.Graphic.SetTexture(0, m_texture);
            Main.MainClass.Graphic.Transform.World = m_matRotation * matWorld;
            Main.MainClass.Graphic.SetStreamSource(0, m_vertexbuffer, 0);
            Main.MainClass.Graphic.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
        }

        public override void Dispose()
        {
            m_texture.Dispose();
            m_vertexbuffer.Dispose();
        }
    }
}
