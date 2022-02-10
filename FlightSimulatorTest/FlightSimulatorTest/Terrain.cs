using System;
using System.Drawing;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class Terrain : IDisposable,IRenderable,ITerrainInfo
    {
        #region Constants
        enum EntryLocations
        {
            HeigthMapPath    = 0,
            TexturePath      = 1,
            GroundObjectPath = 2,
        }
        #endregion

        #region Members
        public static IndexBuffer Indices;
        protected static Material m_matrl;
        public int SizeX;
        public int SizeY;
        public float Spacing;
        public float ElevationFactor;
        public int SecNumX;
        public int SecNumY;
        TerrainSection[,] Sections;
        string[,,] TerrainLoader;

        public static Object3D[] GroundObjects;

        #endregion

        // Constructor
        public Terrain(string LoaderPath ,string GroundObjsPath)
        {
            m_matrl = new Material();
            m_matrl.Diffuse = Color.White;
            m_matrl.Ambient = Color.FromArgb(64, 64, 64);

            TerrainLoader = LoadData(LoaderPath);

            LoadGraoundObjects(GroundObjsPath);

            CreatIndexBuffer();
            InitSections();
        }

        #region Properties
        public static Material Material
        {
            set { m_matrl = value; }
            get { return m_matrl; }
        }
        #endregion

        #region Methodes
        public float[,] ReadHeightMap(string FileName)
        {
            Bitmap heightmapImage = new Bitmap(FileName);
            float[,] Data = new float[heightmapImage.Width, heightmapImage.Height];

            for (int y = 0; y < heightmapImage.Height; ++y)
            {
                for (int x = 0; x < heightmapImage.Width; ++x)
                {
                    Data[x, y] = heightmapImage.GetPixel(x, y).ToArgb() & 0xFF;
                }
            }
            return Data;
        }
        void CreatIndexBuffer()
        {
            int numOfFaces = SizeX * SizeY * 2;
            int[] indices = new int[numOfFaces * 3];
            int Offset = 0;
            for (int y = 0; y < SizeY - 1; y++)
            {
                for (int x = 0; x < SizeX - 1; ++x)
                {
                    indices[Offset + 0] = SizeX * y + x;                // 0
                    indices[Offset + 1] = (SizeX * (y + 1)) + x + 1;    // 1
                    indices[Offset + 2] = SizeX * y + (x + 1);          // sizex + 1

                    indices[Offset + 3] = SizeX * y + x;                // 0
                    indices[Offset + 4] = (SizeX * (y + 1)) + x;        // sizex + 1
                    indices[Offset + 5] = (SizeX * (y + 1)) + x + 1;    // sixex + 2

                    Offset += 6;
                }
            }
            Indices = new IndexBuffer(
                typeof(int),
                numOfFaces * 3,
                Main.MainClass.Graphic,
                Usage.WriteOnly,
                Pool.Managed);
            Indices.SetData(indices, 0, LockFlags.None);
        }
        string[,,] LoadData(string LoaderPath)
        {
            string[] data = File.ReadAllText(LoaderPath).Replace("\r\n", "\n").Split('\n');
            SecNumX = Convert.ToInt32(data[0]);
            SecNumY = Convert.ToInt32(data[1]);
            SizeX = Convert.ToInt32(data[2]);
            SizeY = Convert.ToInt32(data[3]);
            Spacing = Convert.ToSingle(data[4]);
            ElevationFactor = Convert.ToSingle(data[5]);

            int HeaderOffset = 6;
            int PathEntryLines = 3; // hieghtmap path ,texture path ,gndobj path
            string[,,] Pathes = new string[SecNumX, SecNumY,PathEntryLines];
            for (int y = 0; y < SecNumY; ++y)
            {
                for (int x = 0; x < SecNumX; ++x)
                {
                    int index = x * PathEntryLines;
                    index += y * SecNumX * PathEntryLines;
                    index += HeaderOffset;

                    for (int i = 0; i < PathEntryLines; ++i)
                    {
                        Pathes[x, y, i] = data[index + i];
                    }
                }
            }
            return Pathes;
        }
        void InitSections()
        {
            Sections = new TerrainSection[SecNumX, SecNumY];
            for (int y = 0; y < SecNumY; ++y)
                for (int x = 0; x < SecNumX; ++x)
                {
                    Sections[x,y] = new TerrainSection(
                        "Terrain" + x+','+y,
                        TerrainLoader[x,y,(int)EntryLocations.GroundObjectPath],
                        SizeX,
                        SizeY,
                        Spacing,
                        ElevationFactor,
                        ReadHeightMap(TerrainLoader[x,y,(int)EntryLocations.HeigthMapPath]),
                        TextureLoader.FromFile(Main.MainClass.Graphic,TerrainLoader[x,y,(int)EntryLocations.TexturePath]));

                    float X = x * (SizeX - 1) * Spacing;
                    float Z = y * (SizeY - 1) * Spacing;
                    Sections[x, y].Position = new Vector3(X, 0, Z);
                }
        }

        void LoadGraoundObjects(string GndObjPath)
        {
            int NumOfIndexed;
            int NumOfBillBoard;
            string[] Lines;
            Lines = File.ReadAllText(GndObjPath).Replace("\r\n", "\n").Split('\n');
            NumOfIndexed = Convert.ToInt32(Lines[0]);
            NumOfBillBoard = Convert.ToInt32(Lines[1]);
            GroundObjects = new Object3D[NumOfIndexed + NumOfBillBoard];

            for (int i = 0; i < NumOfIndexed;++i )
            {
                GroundObjects[i] = new IndexedNormal(Lines[2 + (i * 3)], Lines[3 + (i * 3)], Lines[4 + (3 * i)]);
            }
            int offset = 2 + 3 * NumOfIndexed;
            for (int i = 0; i < NumOfBillBoard; ++i)
            {
                GroundObjects[i + NumOfIndexed] = new BillBoard(Lines[offset + (i * 4)],
                    Convert.ToSingle(Lines[offset + 1 + (i * 4)]),
                    Convert.ToSingle(Lines[offset + 2 + (i * 4)]),
                    Lines[offset + 3 + (i * 4)],
                    Main.MainClass.Graphic);
            }
        }

        public static int GetGndObjIndexByName(string Name)
        {
            for (int i = 0; i < GroundObjects.Length; ++i)
                if (String.Compare(GroundObjects[i].Name, Name, true) == 0)
                    return i;
            return -1;
        }

        #endregion

        #region Interfaces
        public Matrix GetMatrix(Vector3 Pos, float h)
        {
            int x = (int)(Pos.X / (Spacing * (SizeX - 1)));
            int y = (int)(Pos.Z / (Spacing * (SizeY - 1)));
            return Sections[x, y].GetMatrix(Pos, h);
        }
        public float TerrainHeight(Vector3 Position)
        {
            int x = (int)(Position.X / (Spacing * (SizeX - 1)));
            int y = (int)(Position.Z / (Spacing * (SizeY - 1)));
            if (x < 0 || y < 0 ||
                x > SecNumX || y > SecNumY)
                return 0;

            return Sections[x, y].TerrainHeight(Position.X, Position.Z);
        }

        public float HeightAboveTerrain(Vector3 Position)
        {
            return Position.Y - TerrainHeight(Position);
        }
        public Attitude GetSlope(Vector3 Position, float Heading)
        {
            int x = (int)(Position.X / (Spacing * (SizeX - 1)));
            int y = (int)(Position.Z / (Spacing * (SizeY - 1)));
            return Sections[x, y].GetSlope(Position, Heading,true);//, true
        }
        public void Render()
        {
            for (int y = 0; y < SecNumY; ++y)
                for (int x = 0; x < SecNumX; ++x)
                    Sections[x, y].Render();
        }
        public void Render(Vector3 Position)
        {
            int x = (int)(Position.X / (Spacing * (SizeX - 1)));
            int y = (int)(Position.Z / (Spacing * (SizeY - 1)));

            //x %= SecNumX;
            //y %= SecNumY;

            if (x < 0)
                x = 0;
            if (x >= SecNumX)
                x = SecNumX - 1;
            if (y < 0)
                y = 0;
            if (y >= SecNumY)
                y = SecNumY - 1;

            Sections[x, y].Render();
        }
        public void Render(Camera cam)
        {
            for (int y = 0; y < SecNumY; ++y)
                for (int x = 0; x < SecNumX; ++x)
                    Sections[x, y].Render(cam);
        }
        public void Render(Camera cam, Matrix matWorld)
        {
            Render(cam);
        }
        public void Dispose()
        {
        }
        #endregion

    }

    class TerrainSection : Object3D ,IDisposable
    {

        #region Members
        protected VertexBuffer m_vertexbuffer;
        protected Texture m_texture;
        protected float[,] m_heightMap; // height data
        protected Vector3[,,] m_faceNormals; // normal data
        protected int m_sizeX; // number of x vertices
        protected int m_sizeY; // number of y verices
        protected float m_spacing; // space between celles
        protected float m_elevationFactor;
        protected RectangleF m_rect;
        protected float m_width;
        protected float m_height;

        protected GroundObject[] m_GroundObjects;
        #endregion

        //debug
#if _VECTORS
        CustomVertex.PositionColored[] Lines;
        public bool m_Normals = false;
#endif

        #region Proprties
        public override Vector3 Position
        {
            get
            {
                return base.Position;

            }
            set
            {
                base.Position = value;
                SetMatrix();
                InitBounds();
            }
        }
        public override float X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
                SetMatrix();
                InitBounds();
            }
        }
        public override float Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
                SetMatrix();
                InitBounds();
            }
        }
        public override float Z
        {
            get
            {
                return base.Z;
            }
            set
            {
                base.Z = value;
                SetMatrix();
                InitBounds();
            }
        }
        #endregion

        // Constructor
        public TerrainSection(string Name, string GndObjPath, int SizeX, int SizeY, float Spacing, float ElevationFactor, float[,] HeightMap, Texture texture)
            : base(Name)
        {
            m_sizeX = SizeX;
            m_sizeY = SizeY;
            m_heightMap = HeightMap;
            m_texture = texture;
            m_spacing = Spacing;
            m_elevationFactor = ElevationFactor;

            InitBounds();

            // Initialize Vertices
            CustomVertex.PositionNormalTextured[] vertices = new CustomVertex.PositionNormalTextured[m_sizeX * m_sizeY];
            float tu, tv;
            tu = 1 / (float)(SizeX - 1);
            tv = 1 / (float)(SizeY-1);
            for (int height = 0; height < m_sizeY; ++height)
            {
                for (int width = 0; width < m_sizeX; ++width)
                {
                    int index = m_sizeY * height + width;
                    vertices[index].X = Spacing * width;
                    vertices[index].Y = ElevationFactor * m_heightMap[width, height];
                    vertices[index].Z = Spacing * height;
                    vertices[index].Tu = width * tu;
                    vertices[index].Tv = 1 - height * tv;
                }
            }

            // Initialize Normals
            m_faceNormals = new Vector3[m_sizeX, m_sizeY,2];
            for (int y = 0; y < SizeY - 1; ++y)
            {
                for (int x = 0; x < SizeX - 1; ++x)
                {
                    Vector3 v0 = new Vector3(vertices[SizeX * y + x].X,vertices[SizeX * y + x].Y,vertices[SizeX * y + x].Z);
                    Vector3 v1 = new Vector3(vertices[SizeX * y + x + 1].X,vertices[SizeX * y + x + 1].Y,vertices[SizeX * y + x + 1].Z);
                    Vector3 v2 = new Vector3(vertices[SizeX * (y + 1) + x].X,vertices[SizeX * (y+1) + x].Y,vertices[SizeX * (y+1) + x].Z);
                    Vector3 v3 = new Vector3(vertices[SizeX * (y + 1) + x + 1].X,vertices[SizeX * (y + 1) + x + 1].Y,vertices[SizeX * (y + 1) + x + 1].Z);

                    m_faceNormals[x, y, 0] = Vector3.Cross((v3 - v0), (v1 - v0));
                    m_faceNormals[x, y, 1] = Vector3.Cross((v2 - v0), (v3 - v0));

                    m_faceNormals[x, y, 0].Normalize();
                    m_faceNormals[x, y, 1].Normalize();
                }
            }
            int[] ind;
            CreatIndex(SizeX, SizeY, out ind);
            ComputeNormals(ref vertices, ref ind);
            //CreatLines(vertices);

            // Create vertexbuffer
            m_vertexbuffer = new VertexBuffer(
                typeof(CustomVertex.PositionNormalTextured),
                SizeX * SizeY,
                Main.MainClass.Graphic,
                Usage.WriteOnly,
                CustomVertex.PositionNormalTextured.Format,
                Pool.Managed);
            m_vertexbuffer.SetData(vertices, 0, LockFlags.None);

            LoadGroundObjects(GndObjPath);
        }

        #region Methodes
        public void InitBounds()
        {
            m_width = m_spacing * (m_sizeX - 1);
            m_height = m_spacing * (m_sizeY - 1);

            m_rect = new RectangleF(X, Z, m_width, m_height);
            m_fRadius = (float)Math.Sqrt((m_width * m_width / 4) + (m_height * m_height / 4));
        }
        public void SetMatrix()
        {
            m_Matrix = Matrix.RotationYawPitchRoll(m_vOrientation.Yaw, m_vOrientation.Pitch, m_vOrientation.Roll);
            m_Matrix = m_Matrix * Matrix.Translation(m_vPosition);

            if (m_GroundObjects != null)
                for (int i = 0; i < m_GroundObjects.Length; ++i)
                    m_GroundObjects[i].InitMatrixes(m_vPosition);

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
        public  void CreatIndex(int SizeX,int SizeY,out int[] ind)
        {
            int numOfFaces = SizeX * SizeY * 2;
            ind = new int[numOfFaces * 3];
            int Offset = 0;
            for (int y = 0; y < SizeY - 1; y++)
            {
                for (int x = 0; x < SizeX - 1; ++x)
                {
                    ind[Offset + 0] = SizeX * y + x;
                    ind[Offset + 1] = (SizeX * (y + 1)) + x + 1;
                    ind[Offset + 2] = SizeX * y + (x + 1);

                    ind[Offset + 3] = SizeX * y + x;
                    ind[Offset + 4] = (SizeX * (y + 1)) + x;
                    ind[Offset + 5] = (SizeX * (y + 1)) + x + 1;

                    Offset +=6;
                }
            }
        }
#if _VECTORS
        public void CreatLines(CustomVertex.PositionNormalTextured[] m_verts)
        {

            Lines = new CustomVertex.PositionColored[m_verts.Length * 2];
            for (int i = 0; i < m_verts.Length; ++i)
            {
                Lines[i * 2 + 0].Position = m_verts[i + 0].Position;
                Lines[i * 2 + 1].Position = m_verts[i + 0].Position + m_verts[i + 0].Normal * 5;
                Lines[i * 2 + 0].Color = Lines[i * 2 + 1].Color = Color.Red.ToArgb();
            }
        }
#endif
        public float TerrainHeight(float X, float Z)
        {
            float Height = -m_elevationFactor * 300;
            if (m_rect.Contains(X, Z))
            {
                int cellX = (int)((X - this.X) / m_spacing);
                int cellZ = (int)((Z - this.Z) / m_spacing);

                float dx = X - cellX * m_spacing - this.X;
                float dz = Z - cellZ * m_spacing - this.Z;

                Vector3 v0 = new Vector3(cellX * m_spacing, m_heightMap[cellX, cellZ] * m_elevationFactor, cellZ * m_spacing);

                //Face number1
                if (dx <= dz)
                {
                    Height = v0.Y + (m_faceNormals[cellX, cellZ, 1].X * dx + m_faceNormals[cellX, cellZ, 1].Z * dz) / -m_faceNormals[cellX, cellZ, 1].Y;
                }
                else // Face number0
                {
                    Height = v0.Y + (m_faceNormals[cellX, cellZ, 0].X * dx + m_faceNormals[cellX, cellZ, 0].Z * dz) / -m_faceNormals[cellX, cellZ, 0].Y;
                }

            }
            return Height;
        }

        public Attitude GetSlope(Vector3 Position, float Heading)//
        {
            Vector3 u = new Vector3((float)Math.Cos(Geometry.DegreeToRadian(Heading)), 0, (float)Math.Sin(Geometry.DegreeToRadian(Heading)));
            Vector3 up = new Vector3(u.Z, 0, -u.X);

            u.Normalize();
            up.Normalize();

            // Find Cell
            int cellX = (int)((Position.X - this.X) / m_spacing);
            int cellZ = (int)((Position.Z - this.Z) / m_spacing);

            float dx = Position.X - cellX * m_spacing - this.X;
            float dz = Position.Z - cellZ * m_spacing - this.Z;

            float dotu;
            float dotup;

            Vector3 Normal;
            //face number1
            if (dx <= dz)
            {
                Normal = m_faceNormals[cellX, cellZ, 1];
            }
            else//face number0
            {
                Normal = m_faceNormals[cellX, cellZ, 0];
            }
            //Normal.TransformCoordinate(Matrix.RotationY(Geometry.DegreeToRadian(Yaw)));
            Normal.Normalize();
            dotu = Vector3.Dot(Normal, u);
            dotup = Vector3.Dot(Normal, up);

            float roll = Geometry.RadianToDegree((float)Math.Acos(dotu));
            float pitch = Geometry.RadianToDegree((float)Math.Acos(dotup));

            if (pitch >= 90)
                pitch = pitch - 90;
            else
                pitch = (90 - pitch);

            if (roll >= 90)
                roll = roll - 90;
            else
                roll = (90 - roll);

            if (Normal.X > 0)
                roll = -roll;

            if (Normal.Z > 0)
                pitch = -pitch;

            return new Attitude(Heading, pitch, roll);
        }

        public Attitude GetSlope(Vector3 Pos, float Heading, bool t)
        {

            Pos .Y  = TerrainHeight(Pos.X,Pos.Z);
            Vector3 u = new Vector3((float)Math.Cos(Geometry.DegreeToRadian(Heading)),
                0,
                (float)Math.Sin(Geometry.DegreeToRadian(Heading)));
            Vector3 up = new Vector3(u.Z,0,-u.X);

            Vector3 pu = new Vector3(Pos.X + u.X,
                Height(GetNormal(Pos),GetPoint(Pos),new Vector3(Pos.X + u.X,0, Pos.Z + u.Z)),
                Pos.Z + u.Z);
            Vector3 pup = new Vector3(Pos.X + up.X,
                Height(GetNormal(Pos),GetPoint(Pos),new Vector3(Pos.X + up.X,0, Pos.Z + up.Z)),
                Pos.Z + up.Z);

            pu = pu - Pos;
            pup = pup - Pos;

            pu = Vector3.Cross(pu, Vector3.Cross(pu, u));    // pu x (pu x u)
            pup = Vector3.Cross(pup, Vector3.Cross(pup, up));// pup x (pup x up)

            pu.Normalize();
            pup.Normalize();

            float roll = -Geometry.RadianToDegree((float)Math.Asin(Vector3.Dot(u, pu)));
            float pitch = -Geometry.RadianToDegree((float)Math.Asin(Vector3.Dot(up, pup)));

            Attitude att = new Attitude(Heading, pitch, roll);
            return att;
        }

        public Matrix GetMatrix(Vector3 Pos , float h)
        {
            Matrix mat;
            Vector3 N = GetNormal(Pos);
            Quaternion q = Quaternion.RotationAxis(N,h);
            q.Normalize();

            mat = Matrix.RotationQuaternion(q);
            return mat;
        }

        void LoadGroundObjects(string GndObjPath)
        {
            // there isn't any Ground object in this section
            if (GndObjPath == "NULL")
                return;

            int NumOfObjects;
            string Name;
            int NumOfPos;
            int offset = 1;
            Vector3 Pos;

            string[] Lines = File.ReadAllText(GndObjPath).Replace("\r\n", "\n").Split('\n');
            NumOfObjects = Convert.ToInt32(Lines[0]);
            m_GroundObjects = new GroundObject[NumOfObjects];

            for (int i = 0; i < NumOfObjects; ++i)
            {
                Name = Lines[offset]; // Read Name
                NumOfPos = Convert.ToInt32(Lines[offset + 1]); // Read NumOfPoses
                m_GroundObjects[i] = new GroundObject();
                m_GroundObjects[i].Poses = new Vector3[NumOfPos];
                m_GroundObjects[i].matTerans = new Matrix[NumOfPos];

                offset += 2;
                for (int j = 0; j < NumOfPos; ++j)
                {
                    string[] vect = Lines[offset + j].Split(',');
                    Pos.X = Convert.ToSingle(vect[0]);
                    Pos.Z = Convert.ToSingle(vect[1]);
                    Pos.Y = TerrainHeight(Pos.X, Pos.Z);
                    m_GroundObjects[i].Poses[j] = Pos;
                    m_GroundObjects[i].Model = Terrain.GroundObjects[Terrain.GetGndObjIndexByName(Name)];
                }
                m_GroundObjects[i].InitMatrixes(m_vPosition);
                offset += NumOfPos;
            }
        }

        double Distance(Vector3 P1, Vector3 P2)
        {
            return Math.Sqrt(
                ((P1.X - P2.X) * (P1.X - P2.X)) +
                ((P1.Y - P2.Y) * (P1.Y - P2.Y)) +
                ((P1.Z - P2.Z) * (P1.Z - P2.Z)));
        }
        float Height(Vector3 N, Vector3 V0, Vector3 P)
        {
            float height;
            height = (N.X * (P.X - V0.X) + N.Z * (P.Z - V0.Z)) / -N.Y + V0.Y;
            return height;
        }
        Vector3 GetNormal(Vector3 Position)
        {
            Vector3 Normal;
            int cellX = (int)((Position.X - this.X) / m_spacing);
            int cellZ = (int)((Position.Z - this.Z) / m_spacing);
            float dx = Position.X - cellX * m_spacing - this.X;
            float dz = Position.Z - cellZ * m_spacing - this.Z;
            if (dx <= dz)
            {
                Normal = m_faceNormals[cellX, cellZ, 1];
            }
            else // Face number0
            {
                Normal = m_faceNormals[cellX, cellZ, 0];
            }
            return Normal;
        }
        Vector3 GetPoint(Vector3 Position)
        {
            int cellX = (int)((Position.X - this.X) / m_spacing);
            int cellZ = (int)((Position.Z - this.Z) / m_spacing);
            Vector3 v0 = new Vector3(cellX * m_spacing,
                m_heightMap[cellX, cellZ] * m_elevationFactor,
                cellZ * m_spacing);
            return v0;
        }

        void BlendingEnable(Device Graphic)
        {
            Graphic.RenderState.Lighting = false;
            Graphic.RenderState.AlphaBlendEnable = true;
            Graphic.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.TextureColor);
            Graphic.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);
            Graphic.RenderState.SourceBlend = Blend.SourceAlpha;
            Graphic.RenderState.DestinationBlend = Blend.InvSourceAlpha;
            Graphic.RenderState.AlphaBlendOperation = BlendOperation.Add;
            Graphic.RenderState.AlphaTestEnable = true;
            Graphic.RenderState.ReferenceAlpha = 0x08;
            Graphic.RenderState.AlphaFunction = Compare.GreaterEqual;
        }
        void BlendingDisable(Device Graphic)
        {
            Graphic.RenderState.AlphaBlendEnable = false;
            Graphic.RenderState.Lighting = true;
        }
        #endregion

        #region Interfaces
        public override void Dispose()
        {
            m_texture.Dispose();
            m_vertexbuffer.Dispose();
        }
        public override void Render()
        {
            Main.MainClass.Graphic.Transform.World = m_Matrix;
            Main.MainClass.Graphic.Material = Terrain.Material;
            Main.MainClass.Graphic.VertexFormat = CustomVertex.PositionNormalTextured.Format;
            Main.MainClass.Graphic.SetTexture(0, m_texture);
            Main.MainClass.Graphic.Indices = Terrain.Indices;
            Main.MainClass.Graphic.SetStreamSource(0, m_vertexbuffer, 0);
            Main.MainClass.Graphic.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_sizeX * m_sizeY, 0, (m_sizeX - 1) * (m_sizeY - 1) * 2);

#if _VECTORS
            if (m_Normals)
            {
                Main.MainClass.Graphic.RenderState.Lighting = false;
                Main.MainClass.Graphic.SetTexture(0, null);
                Main.MainClass.Graphic.VertexFormat = CustomVertex.PositionColored.Format;
                Main.MainClass.Graphic.DrawUserPrimitives(PrimitiveType.LineList, Lines.Length / 2, Lines);
                Main.MainClass.Graphic.RenderState.Lighting = true;
            }
#endif
        }
        public void Render(Camera cam)
        {
            Render();
            if (m_GroundObjects != null)
                for (int i = 0; i < m_GroundObjects.Length; ++i)
                    m_GroundObjects[i].Render(cam);
        }
        #endregion
    }
}
