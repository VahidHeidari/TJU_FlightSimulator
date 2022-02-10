using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace _3dsConvertor
{
    struct Vertex
    {
        public float x, y, z;
        public float u, v;

        public override string ToString()
        {
            return x + "," + y + "," + z + ",\t\t" + u + "," + v;
        }
    }
    struct Face
    {
        public int a, b, c;

        public override string ToString()
        {
            return a + "," + b + "," + c;
        }
        public string ToString( int offset)
        {
            return (a + offset) + "," + (b + offset) + "," + (c + offset);
        }
    }
    struct Object
    {
        public Vertex[] Vertices;
        public Face[] Faces;

        public int NumOfFaces;
        public int NumOfVertices;

        public string Name;
    }
    public class Convertor
    {
        List<ushort[]> INDEXES = new List<ushort[]>();
        List<Face[]> Indexes = new List<Face[]>();
        List<Vertex[]> Vertices = new List<Vertex[]>();
        List<int> FaceOffset = new List<int>();
        int NumOfVertices = 0, NumOfIndixes = 0;

        public void ConvertToFile(string _3dsFilePath,string OutputPath)
        {
            // Unsigned Short   = 16 bit = ushort
            // Unsigned Int     = 32 bit = uint
            ushort l_chunk_id;
            uint l_chunk_lenght;

            char l_char;
            ushort l_qty=0;

            ushort l_face_flags;

            Stream Input = File.OpenRead(_3dsFilePath);
            BinaryReader BR = new BinaryReader(Input);

            Face[] Indx;
            Vertex[] vrt;

            while (Input.Position < Input.Length)
            {
                l_chunk_id = BR.ReadUInt16();
                l_chunk_lenght = BR.ReadUInt32();

                switch (l_chunk_id)
                {
                    //----------------- MAIN3DS -----------------
                    // Description: Main chunk, contains all the other chunks
                    // Chunk ID: 4d4d
                    // Chunk Lenght: 0 + sub chunks
                    //-------------------------------------------
                    case 0x4d4d:
                        break;

                    //----------------- EDIT3DS -----------------
                    // Description: 3D Editor chunk, objects layout info
                    // Chunk ID: 3d3d (hex)
                    // Chunk Lenght: 0 + sub chunks
                    //-------------------------------------------
                    case 0x3d3d:
                        break;

                    //--------------- EDIT_OBJECT ---------------
                    // Description: Object block, info for each object
                    // Chunk ID: 4000 (hex)
                    // Chunk Lenght: len(object name) + sub chunks
                    //-------------------------------------------
                    case 0x4000:
                        string Name = "";
                        do
                        {
                            l_char = BR.ReadChar();
                            Name += l_char;
                        } while (l_char != '\0');
                        break;

                    //--------------- OBJ_TRIMESH ---------------
                    // Description: Triangular mesh, contains chunks for 3d mesh info
                    // Chunk ID: 4100 (hex)
                    // Chunk Lenght: 0 + sub chunks
                    //-------------------------------------------
                    case 0x4100:
                        break;

                    //--------------- TRI_VERTEXL ---------------
                    // Description: Vertices list
                    // Chunk ID: 4110 (hex)
                    // Chunk Lenght: 1 x unsigned short (number of vertices)
                    //             + 3 x float (vertex coordinates) x (number of vertices)
                    //             + sub chunks
                    //-------------------------------------------
                    case 0x4110:
                        //(unsigned short)

                        l_qty = BR.ReadUInt16();
                        FaceOffset.Add(NumOfVertices);
                        NumOfVertices += l_qty;
                        vrt = new Vertex[l_qty];
                        for (int i = 0; i < l_qty; i++)
                        {
                            vrt[i].x = -BR.ReadSingle();
                            vrt[i].z = -BR.ReadSingle();
                            vrt[i].y = BR.ReadSingle();
                        }
                        Vertices.Add(vrt);
                        break;

                    //--------------- TRI_FACEL1 ----------------
                    // Description: Polygons (faces) list
                    // Chunk ID: 4120 (hex)
                    // Chunk Lenght: 1 x unsigned short (number of polygons)
                    //             + 3 x unsigned short (polygon points) x (number of polygons)
                    //             + sub chunks
                    //-------------------------------------------
                    case 0x4120:
                        // (unsigned short)
                        l_qty = BR.ReadUInt16();
                        NumOfIndixes += l_qty;
                        Indx = new Face[l_qty];
                        for (int i = 0; i < l_qty; i++)
                        {
                            // (unsigned short)
                            Indx[i].a = BR.ReadUInt16();
                            Indx[i].c = BR.ReadUInt16();
                            Indx[i].b = BR.ReadUInt16();
                            // (unsigned short)
                            l_face_flags = BR.ReadUInt16();
                        }
                        Indexes.Add(Indx);
                        //ushort[] INDX = new ushort[l_qty * 3];
                        //for (int i = 0; i < l_qty; i+=3)
                        //{
                        //    // (unsigned short)
                        //    INDX[i + 0] = BR.ReadUInt16();
                        //    INDX[i + 2] = BR.ReadUInt16();
                        //    INDX[i + 1] = BR.ReadUInt16();
                        //    // (unsigned short)
                        //    l_face_flags = BR.ReadUInt16();
                        //}
                        //INDEXES.Add(INDX);
                        break;

                    //------------- TRI_MAPPINGCOORS ------------
                    // Description: Vertices list
                    // Chunk ID: 4140 (hex)
                    // Chunk Lenght: 1 x unsigned short (number of mapping points)
                    //             + 2 x float (mapping coordinates) x (number of mapping points)
                    //             + sub chunks
                    //-------------------------------------------
                    case 0x4140:
                        //(unsigned short)
                        l_qty = BR.ReadUInt16();
                        for (int i = 0; i < l_qty; i++)
                        {
                            Vertices[Vertices.Count - 1][i].u = BR.ReadSingle(); //(float)
                            Vertices[Vertices.Count - 1][i].v = -BR.ReadSingle();//(float)
                            Vertices[Vertices.Count - 1][i].v = (int)Vertices[Vertices.Count - 1][i].v + 1 + Vertices[Vertices.Count - 1][i].v;
                        }
                        break;

                    //----------- Skip unknow chunks ------------
                    //We need to skip all the chunks that currently we don't use
                    //We use the chunk lenght information to set the file pointer
                    //to the same level next chunk
                    //-------------------------------------------
                    default:
                        //fseek(l_file, l_chunk_lenght-6, SEEK_CUR);
                        Input.Seek(l_chunk_lenght - 6, SeekOrigin.Current);
                        break;
                }
            }
            BR.Close();
            Input.Close();

            StringBuilder txt = new StringBuilder();
            txt.Append("# Number of vertices\r\n");
            txt.Append(NumOfVertices + "\r\n");
            for (int i = 0; i < Vertices.Count; ++i)
            {
                for (int j = 0; j < Vertices[i].Length; ++j)
                    txt.Append(Vertices[i][j].ToString() + "\r\n");
            }
            txt.Append("\r\n#====================\r\n" +
                       "# Number of Faces\r\n");
            txt.Append(NumOfIndixes + "\r\n");
            for (int i = 0; i < Indexes.Count; ++i)
            {
                for (int j = 0; j < Indexes[i].Length; ++j)
                    txt.Append(Indexes[i][j].ToString(FaceOffset[i]) + "\r\n");
            }
            //for (int i = 0; i < INDEXES.Count; ++i)
            //{
            //    for (int j = 0; j < INDEXES[i].Length; j += 3)
            //    {
            //        txt.Append(
            //            (INDEXES[i][j + 0] + FaceOffset[i]) + ","+
            //            (INDEXES[i][j + 1] + FaceOffset[i]) + "," +
            //            (INDEXES[i][j + 2] + FaceOffset[i]) + "\r\n");
            //    }
            //}
            File.WriteAllText(OutputPath, txt.ToString());
        }
    }
}
