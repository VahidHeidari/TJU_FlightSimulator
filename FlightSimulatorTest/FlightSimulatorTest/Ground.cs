using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    class Ground
    {
        CustomVertex.PositionColored[] ground;
        public Ground(float Width, float Height, int Color)
        {
            ground = new CustomVertex.PositionColored[4];

            ground[0].X = -Width / 2;
            ground[0].Y = 0;
            ground[0].Z = Height / 2;


            ground[1].X = Width / 2;
            ground[1].Y = 0;
            ground[1].Z = Height / 2;


            ground[2].X = -Width / 2;
            ground[2].Y = 0;
            ground[2].Z = -Height / 2;


            ground[3].X = Width / 2;
            ground[3].Y = 0;
            ground[3].Z = -Height / 2;
            ground[3].Color = ground[2].Color = ground[1].Color = ground[0].Color = Color;
        }
        public Ground()
        {
            ground = new CustomVertex.PositionColored[4];

            ground[0].X = -5;
            ground[0].Y = 0;
            ground[0].Z = 5;


            ground[1].X = 5;
            ground[1].Y = 0;
            ground[1].Z = 5;


            ground[2].X = -5;
            ground[2].Y = 0;
            ground[2].Z = -5;


            ground[3].X = 5;
            ground[3].Y = 0;
            ground[3].Z = -5;
            ground[3].Color = ground[2].Color = ground[1].Color = ground[0].Color = Color.Orange.ToArgb();
        }

        public void Render(Device Graphic)
        {
            Graphic.SetTexture(0, null);
            Graphic.Transform.World = Matrix.Identity;
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, ground);
        }

        public void Render(Device Graphic, Matrix World)
        {
            Graphic.Transform.World = World;
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, ground);
        }
    }
}
