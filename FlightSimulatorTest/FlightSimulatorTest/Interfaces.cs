using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public interface IRenderable
    {
        void Render();
        void Render(Camera cam, Matrix MatWorld);
    }
    public interface ICullable
    {
        bool Culled { set;}
        bool IsCulled { get;}
    }
    public interface ICollidable
    {
        Vector3 CenterOfMass { get;}
        float BoundingRadius { get;}
        bool CollidSphere(Object3D other);
        bool CollidPolygon(Vector3 Point1, Vector3 Point2, Vector3 Point3);
    }
    public interface IDynamic
    {
        void Update(float DeltaTime);
    }
    public interface ITerrainInfo
    {
        float TerrainHeight(Vector3 Position);
        float HeightAboveTerrain(Vector3 Position);
        Attitude GetSlope(Vector3 Position, float Heading);
    }
}
