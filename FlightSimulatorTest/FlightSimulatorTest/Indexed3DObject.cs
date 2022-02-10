using System;
using System.IO;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Main;

namespace Framework
{
    public class IndexedNormal:Object3D
    {
        VertexBuffer m_vertexBuffer;
        IndexBuffer m_indexBuffer;
        public Material m_mtrl;

        public Vector3 M = new Vector3(0, 0, 0);

        Texture m_texture;
#if _VECTORS
        //bool m_Normals = false;
        CustomVertex.PositionColored[] Lines;
        public bool RenderNormals
        {
            set { m_Normals = value; }
            get { return m_Normals; }
        }
#endif
        int m_NumberOfVertices;
        int m_NumberOfFaces;



        private void Read(string FileName)
        {
            m_mtrl = new Material();
            m_mtrl.Diffuse = Color.White;
            m_mtrl.Ambient = Color.White;
            m_mtrl.Specular = Color.White;
            m_mtrl.Emissive = Color.Black;
            m_mtrl.SpecularSharpness = 1.0f;

            CustomVertex.PositionNormalTextured[] m_verts;
            int[] m_index;

            string txt;

            StreamReader SR = new StreamReader(FileName);

            txt = SR.ReadLine();
            while (txt == "" || txt.StartsWith("#"))
                txt = SR.ReadLine();

            // vertex buffer
            m_NumberOfVertices = Convert.ToInt32(txt);
            m_verts = new CustomVertex.PositionNormalTextured[m_NumberOfVertices];

            int verts = 0;
            while (verts < m_NumberOfVertices)
            {
                txt = SR.ReadLine();
                while (txt == "" || txt.StartsWith("#"))
                    txt = SR.ReadLine();

                string[] tocken = txt.Split(',');

                m_verts[verts].X = Convert.ToSingle(tocken[0]);
                m_verts[verts].Y = Convert.ToSingle(tocken[1]);
                m_verts[verts].Z = Convert.ToSingle(tocken[2]);
                m_verts[verts].Tu = Convert.ToSingle(tocken[3]);
                m_verts[verts].Tv = Convert.ToSingle(tocken[4]);

                verts++;
            }

            txt = SR.ReadLine();
            while (txt == "" || txt.StartsWith("#"))
                txt = SR.ReadLine();

            // Index buffer
            m_NumberOfFaces = Convert.ToInt32(txt);
            m_index = new int[m_NumberOfFaces * 3];

            int faces = 0;
            while (faces < m_NumberOfFaces * 3)
            {
                txt = SR.ReadLine();
                while (txt == "" || txt.StartsWith("#"))
                    txt = SR.ReadLine();

                string[] tocken = txt.Split(',');

                m_index[faces + 0] = Convert.ToInt32(tocken[0]);
                m_index[faces + 1] = Convert.ToInt32(tocken[1]);
                m_index[faces + 2] = Convert.ToInt32(tocken[2]);
                faces += 3;
            }

            SR.Close();

            ComputeNormals(ref m_verts, ref m_index);
#if _VECTORS
            Lines = new CustomVertex.PositionColored[m_verts.Length * 2];
            for (int i = 0; i < m_verts.Length; ++i)
            {
                Lines[i * 2 + 0].Position = m_verts[i + 0].Position;
                Lines[i * 2 + 1].Position = m_verts[i + 0].Position + m_verts[i + 0].Normal;
                Lines[i * 2 + 0].Color = Lines[i * 2 + 1].Color = Color.Red.ToArgb();
            }
#endif
            m_vertexBuffer = new VertexBuffer(
                typeof(CustomVertex.PositionNormalTextured),
                m_NumberOfVertices,
                MainClass.Graphic,
                Usage.WriteOnly,
                CustomVertex.PositionNormalTextured.Format,
                Pool.Managed);

            m_indexBuffer = new IndexBuffer(
                typeof(int),
                m_NumberOfFaces * 3,
                MainClass.Graphic,
                Usage.WriteOnly,
                Pool.Managed);

            GraphicsStream gStream = m_vertexBuffer.Lock(0, 0, LockFlags.None);
            gStream.Write(m_verts);
            m_vertexBuffer.Unlock();

            gStream = m_indexBuffer.Lock(0, 0, LockFlags.None);
            gStream.Write(m_index);
            m_indexBuffer.Unlock();
        }

        public IndexedNormal(String Name, string FileName, string TexturePath)
            :base (Name)
        {
            m_texture = TextureLoader.FromFile(MainClass.Graphic, TexturePath);

            Read(FileName);
        }
        public IndexedNormal(string Name, string FileName, Texture texture)
            :base (Name)
        {
            m_texture = texture;

            Read(FileName);
        }

        void ComputeNormals(ref CustomVertex.PositionNormalTextured[] Verts, ref int[] IndeXes)
        {
            for (int i = 0; i < Verts.Length; ++i) // vertex
            {
                Vector3 Normal = new Vector3(0, 0, 0);
                for (int j = 0; j < IndeXes.Length; j += 3) // face
                {
                    if (IndeXes[j + 0] == i ||
                        IndeXes[j + 1] == i ||
                        IndeXes[j + 2] == i)
                    {
                        Vector3 a = Verts[IndeXes[j + 1]].Position - Verts[IndeXes[j + 0]].Position;
                        Vector3 b = Verts[IndeXes[j + 2]].Position - Verts[IndeXes[j + 0]].Position;
                        Vector3 NP = Vector3.Cross(a, b);
                        Normal += NP;
                    }
                }
                Normal.Normalize();
                Verts[i].Normal = Normal;
            }
        }



        public override void Render()
        {
            MainClass.Graphic.Material = m_mtrl;
            MainClass.Graphic.Transform.World = Matrix.RotationYawPitchRoll(Yaw, Pitch, Roll) * Matrix.Translation(Position);
            MainClass.Graphic.SetTexture(0, m_texture);
            MainClass.Graphic.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            MainClass.Graphic.Indices = m_indexBuffer;
            MainClass.Graphic.SetStreamSource(0, m_vertexBuffer, 0);
            MainClass.Graphic.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                m_NumberOfVertices,
                0,
                m_NumberOfFaces);
#if _VECTORS
            if (m_Normals)
            {
                MainClass.Graphic.RenderState.Lighting = false;
                MainClass.Graphic.SetTexture(0, null);
                MainClass.Graphic.VertexFormat = CustomVertex.PositionColored.Format;
                MainClass.Graphic.DrawUserPrimitives(PrimitiveType.LineList, Lines.Length / 2, Lines);
                MainClass.Graphic.RenderState.Lighting = true;
            }
#endif

        }
        public override void Render(Camera cam, Matrix matWorld)
        {
            MainClass.Graphic.Material = m_mtrl;
            MainClass.Graphic.Transform.World = matWorld;
            MainClass.Graphic.SetTexture(0, m_texture);
            MainClass.Graphic.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            MainClass.Graphic.Indices = m_indexBuffer;
            MainClass.Graphic.SetStreamSource(0, m_vertexBuffer, 0);
            MainClass.Graphic.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                m_NumberOfVertices,
                0,
                m_NumberOfFaces);
        }
        public virtual void Render(Matrix matWorld)
        {
            MainClass.Graphic.Material = m_mtrl;
            MainClass.Graphic.Transform.World = matWorld;
            MainClass.Graphic.SetTexture(0, m_texture);
            MainClass.Graphic.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            MainClass.Graphic.Indices = m_indexBuffer;
            MainClass.Graphic.SetStreamSource(0, m_vertexBuffer, 0);
            MainClass.Graphic.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                m_NumberOfVertices,
                0,
                m_NumberOfFaces);
        }
        public override void Dispose()
        {
            m_texture.Dispose();
            m_vertexBuffer.Dispose();
            m_indexBuffer.Dispose();
        }
    }
}
