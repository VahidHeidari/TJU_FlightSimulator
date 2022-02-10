using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class GroundObject: IDisposable//,IRenderable
    {
        public Object3D Model;
        public Vector3[] Poses;
        public Matrix[] matTerans;

        public void InitMatrixes(Vector3 Position)
        {
            for (int i = 0; i < Poses.Length; ++i)
            {
                matTerans[i] = Matrix.Translation(Poses[i] + Position);
            }
        }

        public void Render(Camera cam)
        {
            for (int i = 0; i < matTerans.Length; ++i)
                Model.Render(cam, matTerans[i]);
        }
        public void Dispose()
        {
            Model.Dispose();
        }
    }
}
