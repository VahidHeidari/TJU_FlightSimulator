using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    public class Glass : IndexedNormal
    {
        public Glass(string Name ,string FileName, string TexturePath)
            : base(Name,FileName, TexturePath)
        {
            m_mtrl.Diffuse = Color.FromArgb(96, Color.White);
            m_mtrl.SpecularSharpness = 10;
        }
        public Glass(string Name,string FileName, Texture texture)
            : base(Name,FileName, texture)
        {
            m_mtrl.Diffuse = Color.FromArgb(96, Color.White);
            m_mtrl.SpecularSharpness = 10;
        }

        protected void BlendingEnable()
        {
            Main.MainClass.Graphic.RenderState.AlphaBlendEnable = true;
            Main.MainClass.Graphic.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.Diffuse);
            Main.MainClass.Graphic.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);
            Main.MainClass.Graphic.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            Main.MainClass.Graphic.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            Main.MainClass.Graphic.RenderState.SpecularEnable = true;

        }
        protected void BlendingDisable()
        {
            Main.MainClass.Graphic.RenderState.AlphaBlendEnable = false;
            Main.MainClass.Graphic.RenderState.SpecularEnable = false;
        }

        public override void Render()
        {
            BlendingEnable();
            base.Render();
            BlendingDisable();
        }
        public override void Render(Matrix matWorld)
        {
            BlendingEnable();
            base.Render(matWorld);
            BlendingDisable();
        }
    }
}
