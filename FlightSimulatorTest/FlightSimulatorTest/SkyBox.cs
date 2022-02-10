using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    class SkyBox
    {
        CustomVertex.PositionTextured[] m_North = new CustomVertex.PositionTextured[6];
        CustomVertex.PositionTextured[] m_South = new CustomVertex.PositionTextured[6];
        CustomVertex.PositionTextured[] m_East = new CustomVertex.PositionTextured[6];
        CustomVertex.PositionTextured[] m_West = new CustomVertex.PositionTextured[6];
        CustomVertex.PositionTextured[] m_Top = new CustomVertex.PositionTextured[6];
        CustomVertex.PositionTextured[] m_Down = new CustomVertex.PositionTextured[6];

        Texture m_txrNorth, m_txrSouth, m_txrEast, m_txrWest, m_txrTop, m_txrDown;

        Device dev;
        public SkyBox(Device Graphic)
        {
            dev = Graphic;
            m_txrNorth = TextureLoader.FromFile(dev, "..\\..\\skybox\\North.bmp");
            m_txrSouth = TextureLoader.FromFile(dev, "..\\..\\skybox\\South.bmp");
            m_txrEast = TextureLoader.FromFile(dev, "..\\..\\skybox\\East.bmp");
            m_txrWest = TextureLoader.FromFile(dev, "..\\..\\skybox\\West.bmp");
            m_txrTop = TextureLoader.FromFile(dev, "..\\..\\skybox\\Top.bmp");
            m_txrDown = TextureLoader.FromFile(dev, "..\\..\\skybox\\Down.bmp");

            float size = 50;
            m_North[0].Z = m_North[3].Z = size;
            m_North[0].X = m_North[3].X = -size;
            m_North[0].Y = m_North[3].Y = 0;
            m_North[0].Tu = m_North[3].Tu = 0.01f;
            m_North[0].Tv = m_North[3].Tv = 1.0f;

            m_North[2].Z = m_North[4].Z = size;
            m_North[2].X = m_North[4].X = size;
            m_North[2].Y = m_North[4].Y = size;
            m_North[2].Tu = m_North[4].Tu = 1.0f;
            m_North[2].Tv = m_North[4].Tv = 0.01f;

            m_North[1].Z = size;
            m_North[1].X = -size;
            m_North[1].Y = size;
            m_North[1].Tu = 0.01f;
            m_North[1].Tv = 0.01f;

            m_North[5].Z = size;
            m_North[5].X = size;
            m_North[5].Y = 0;
            m_North[5].Tu = 1.0f;
            m_North[5].Tv = 1.0f;


            m_West[0].Z = m_West[3].Z = -size;
            m_West[0].X = m_West[3].X = -size;
            m_West[0].Y = m_West[3].Y = 0;
            m_West[0].Tu = m_West[3].Tu = 0.01f;
            m_West[0].Tv = m_West[3].Tv = 1.0f;

            m_West[2].Z = m_West[4].Z = size;
            m_West[2].X = m_West[4].X = -size;
            m_West[2].Y = m_West[4].Y = size;
            m_West[2].Tu = m_West[4].Tu = 1.0f;
            m_West[2].Tv = m_West[4].Tv = 0.01f;

            m_West[1].Z = -size;
            m_West[1].X = -size;
            m_West[1].Y = size;
            m_West[1].Tu = 0.01f;
            m_West[1].Tv = 0.01f;

            m_West[5].Z = size;
            m_West[5].X = -size;
            m_West[5].Y = 0;
            m_West[5].Tu = 1.0f;
            m_West[5].Tv = 1.0f;


            m_East[0].Z = m_East[3].Z = size;
            m_East[0].X = m_East[3].X = size;
            m_East[0].Y = m_East[3].Y = 0;
            m_East[0].Tu = m_East[3].Tu = 0.01f;
            m_East[0].Tv = m_East[3].Tv = 1.0f;

            m_East[2].Z = m_East[4].Z = -size;
            m_East[2].X = m_East[4].X = size;
            m_East[2].Y = m_East[4].Y = size;
            m_East[2].Tu = m_East[4].Tu = 1.0f;
            m_East[2].Tv = m_East[4].Tv = 0.01f;

            m_East[1].Z = size;
            m_East[1].X = size;
            m_East[1].Y = size;
            m_East[1].Tu = 0.01f;
            m_East[1].Tv = 0.01f;

            m_East[5].Z = -size;
            m_East[5].X = size;
            m_East[5].Y = 0;
            m_East[5].Tu = 1.0f;
            m_East[5].Tv = 1.0f;


            m_South[2].Z = m_South[4].Z = -size;
            m_South[2].X = m_South[4].X = -size;
            m_South[2].Y = m_South[4].Y = 0;
            m_South[2].Tu = m_South[4].Tu = 1.0f;
            m_South[2].Tv = m_South[4].Tv = 1.0f;

            m_South[0].Z = m_South[3].Z = -size;
            m_South[0].X = m_South[3].X = size;
            m_South[0].Y = m_South[3].Y = size;
            m_South[0].Tu = m_South[3].Tu = 0;
            m_South[0].Tv = m_South[3].Tv = 0;

            m_South[1].Z = -size;
            m_South[1].X = -size;
            m_South[1].Y = size;
            m_South[1].Tu = 1.0f;
            m_South[1].Tv = 0.01f;

            m_South[5].Z = -size;
            m_South[5].X = size;
            m_South[5].Y = 0;
            m_South[5].Tu = 0.01f;
            m_South[5].Tv = 1.0f;

            m_Top[0].X = -size;
            m_Top[0].Y = size;
            m_Top[0].Z = size;
            m_Top[0].Tu = 0.01f;
            m_Top[0].Tv = 0.01f;

            m_Top[4].X = size;
            m_Top[4].Y = size;
            m_Top[4].Z = -size;
            m_Top[4].Tu = 1.0f;
            m_Top[4].Tv = 1.0f;

            m_Top[1].X = m_Top[3].X = -size;
            m_Top[1].Y = m_Top[3].Y = size;
            m_Top[1].Z = m_Top[3].Z = -size;
            m_Top[1].Tu = m_Top[3].Tu = 0.01f;
            m_Top[1].Tv = m_Top[3].Tv = 1.0f;

            m_Top[2].X = m_Top[5].X = size;
            m_Top[2].Y = m_Top[5].Y = size;
            m_Top[2].Z = m_Top[5].Z = size;
            m_Top[2].Tu = m_Top[5].Tu = 1.0f;
            m_Top[2].Tv = m_Top[5].Tv = 0.01f;

            //
            m_Down[1].X = -size;
            m_Down[1].Y = 0;
            m_Down[1].Z = size;
            m_Down[1].Tu = 0.01f;
            m_Down[1].Tv = 0.01f;

            m_Down[5].X = size;
            m_Down[5].Y = 0;
            m_Down[5].Z = -size;
            m_Down[5].Tu = 1.0f;
            m_Down[5].Tv = 1.0f;

            m_Down[0].X = m_Down[3].X = -size;
            m_Down[0].Y = m_Down[3].Y = 0;
            m_Down[0].Z = m_Down[3].Z = -size;
            m_Down[0].Tu = m_Down[3].Tu = 0.01f;
            m_Down[0].Tv = m_Down[3].Tv = 1.0f;

            m_Down[2].X = m_Down[4].X = size;
            m_Down[2].Y = m_Down[4].Y = 0;
            m_Down[2].Z = m_Down[4].Z = size;
            m_Down[2].Tu = m_Down[4].Tu = 1.0f;
            m_Down[2].Tv = m_Down[4].Tv = 0.01f;
        }

        public void Render(Camera cam)
        {
            bool l = dev.RenderState.Lighting;
            dev.RenderState.Lighting = false;
            Matrix matWorld = Matrix.Identity;
            Matrix tmp = Matrix.Identity;
            tmp.M42 = -15;
            matWorld.Translate(cam.Position);
            matWorld = Matrix.Multiply(tmp, matWorld);

            dev.Transform.World = matWorld;

            //dev.RenderState.CullMode = Cull.None;
            dev.RenderState.ZBufferWriteEnable = false;
            //dev.RenderState.Lighting = false;

            dev.VertexFormat = CustomVertex.PositionTextured.Format;

            dev.SetTexture(0, m_txrNorth);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_North);

            dev.SetTexture(0, m_txrSouth);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_South);

            dev.SetTexture(0, m_txrEast);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_East);

            dev.SetTexture(0, m_txrWest);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_West);

            dev.SetTexture(0, m_txrTop);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_Top);

            dev.SetTexture(0, m_txrDown);
            dev.DrawUserPrimitives(PrimitiveType.TriangleList, 2, m_Down);

            dev.RenderState.CullMode = Cull.CounterClockwise;
            dev.RenderState.ZBufferWriteEnable = true;
            dev.RenderState.Lighting = l;
        }
    }
}
