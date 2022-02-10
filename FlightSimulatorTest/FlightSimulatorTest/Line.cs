using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class Line
    {
        CustomVertex.PositionColored[] line = new CustomVertex.PositionColored[2];
        public Line(Vector3 Point1, Vector3 Point2, Color color)
        {
            line[0].X = Point1.X;
            line[0].Y = Point1.Y;
            line[0].Z = Point1.Z;
            line[0].Color = color.ToArgb();

            line[1].X = Point2.X;
            line[1].Y = Point2.Y;
            line[1].Z = Point2.Z;
            line[1].Color = color.ToArgb();
        }
        public void Render(Device Graphic)
        {
            Graphic.Transform.World = Matrix.Identity;
            Graphic.SetTexture(0, null);
            Graphic.VertexFormat = CustomVertex.PositionColored.Format;
            Graphic.DrawUserPrimitives(PrimitiveType.LineList, 1, line);
        }

        public void Render(Device Graphic, Vector3 Force, Vector3 Pos)
        {
            line[0].Position = Pos;
            line[1].X = Pos.X + Force.X;
            line[1].Y = Pos.Y + Force.Y;
            line[1].Z = Pos.Z + Force.Z;

            Render(Graphic);
        }
    }
}
