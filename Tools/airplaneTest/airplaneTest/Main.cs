using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Framework;

namespace Main
{
    class MainClass : Form
    {
        public static Device Graphic;
        Microsoft.DirectX.Direct3D.Font font;
        Ground ground;
        Camera cam;
        //IndexedNormal pilot;

        bool l = false;
        bool r = false;

        bool f = false;

        bool eu = false;
        bool ed = false;

        bool au = false;
        bool ad = false;

        Airplane airplane;

        void Init()
        {
            this.Text = "Airplane Test";
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.KeyDown += new KeyEventHandler(MainClass_KeyDown);
            this.KeyUp += new KeyEventHandler(MainClass_KeyUp);
            this.MouseMove += new MouseEventHandler(MainClass_MouseMove);
            this.MouseDown += new MouseEventHandler(MainClass_MouseDown);

            PresentParameters pp = new PresentParameters();
            pp.BackBufferFormat = Format.X8R8G8B8;
            pp.AutoDepthStencilFormat = DepthFormat.D24X8;
            pp.EnableAutoDepthStencil = true;
            pp.BackBufferCount = 1;
            pp.Windowed = true;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = 800;
            pp.BackBufferHeight = 600;

            Graphic = new Device(0, DeviceType.Hardware, this, CreateFlags.HardwareVertexProcessing, pp);
            Graphic.RenderState.ZBufferEnable = true;
            Graphic.RenderState.CullMode = Cull.CounterClockwise;
            Graphic.RenderState.Lighting = false;
            Graphic.DeviceResizing += new System.ComponentModel.CancelEventHandler(Graphic_DeviceResizing);

            //Graphic.RenderState.AlphaBlendEnable = true;
            Graphic.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.Diffuse);
            Graphic.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);
            Graphic.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            Graphic.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            Graphic.RenderState.SpecularEnable = true;
            //Graphic.RenderState.FillMode = FillMode.WireFrame;
            SetupLight();
        }


        void InitGeometry()
        {


            ground = new Ground(10, 10, Color.Red.ToArgb());
            cam = new Camera();
            cam.Position = new Vector3(0, 5, -5);

            font = new Microsoft.DirectX.Direct3D.Font(Graphic, new System.Drawing.Font("lucida console", 10, FontStyle.Bold));

            airplane = new Airplane(Graphic, "..\\..\\Pathes.txt");
            airplane.SetKey0();
            //pilot = new IndexedNormal("..\\..\\SkyHAwk\\Pilot.txt", "..\\..\\SkyHawk\\pilot.jpg");
        }

        void SetupLight()
        {
            Vector3 dir = new Vector3(-1, -1, -1);
            dir.Normalize();
            Graphic.Lights[0].Type = LightType.Directional;
            Graphic.Lights[0].Diffuse = Color.White;
            Graphic.Lights[0].Ambient = Color.Black;
            Graphic.Lights[0].Specular = Color.White;
            Graphic.Lights[0].Direction = dir;
            Graphic.Lights[0].Enabled = true;

            Graphic.RenderState.ShadeMode = ShadeMode.Gouraud;
            Graphic.RenderState.Ambient = Color.FromArgb(128, 128, 128);
            Graphic.RenderState.Lighting = true;
        }

        void MainClass_MouseDown(object sender, MouseEventArgs e)
        {
            StartX = MouseX = e.X;
            StartY = MouseY = e.Y;
            StartZ = MouseZ = e.Y;
        }

        int StartX = 0, StartY = 0 ,StartZ = 0;
        int MouseX = 0, MouseY = 0 ,MouseZ = 0;
        float Sense = 0.5f;

        void MainClass_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MouseX = e.X;
                MouseY = e.Y;
                int MovementX = MouseX - StartX;
                int MovementY = StartY - MouseY;

                cam.RotateTargetYaw(Sense * MovementX);
                cam.RotateTargetPitch(Sense * MovementY);

                StartX = e.X;
                StartY = e.Y;
            }

            if (e.Button == MouseButtons.Left)
            {
                MouseX = e.X;
                MouseY = e.Y;
                int MovementX = MouseX - StartX;
                int MovementY = MouseY - StartY;

                cam.RotatePosYaw(Sense * MovementX);
                cam.RotatePosPitch(Sense * MovementY);

                StartX = e.X;
                StartY = e.Y;
            }
            if (e.Button == MouseButtons.Middle)
            {
                MouseZ = e.Y;
                int MovementZ = MouseZ - StartZ;

                cam.ZoomForward(Sense * 0.05f * MovementZ);

                StartZ = e.Y;
            }
        }

        void MainClass_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;

                case Keys.F:
                    if (Graphic.RenderState.FillMode == FillMode.Solid)
                        Graphic.RenderState.FillMode = FillMode.WireFrame;
                    else Graphic.RenderState.FillMode = FillMode.Solid;
                    break;

                case Keys.W:
                    cam.MoveForward(0.1f);
                    break;

                case Keys.S:
                    cam.MoveForward(-0.1f);
                    break;
                case Keys.A:
                    cam.MoveRight(0.1f);
                    break;

                case Keys.D:
                    cam.MoveRight(-0.1f);
                    break;

                case Keys.X:
                    l = true;
                    r = false;
                    break;
                case Keys.C:
                    l = false;
                    r = true;
                    break;
                case Keys.G:
                    f ^= true;
                    break;
                case Keys.Down:
                    eu = true;
                    ed = false;
                    break;
                case Keys.Up:
                    eu = false;
                    ed = true;
                    break;

                case Keys.Right:
                    au = true;
                    ad = false;
                    break;
                case Keys.Left:
                    au = false;
                    ad = true;
                    break;

                case Keys.Space:
                    cam.Position = new Vector3(0, 5, -5);
                    cam.Target = new Vector3(0, 0, 0);
                    break;

            }
        }
        void MainClass_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                case Keys.X:
                    l = false;
                    r = false;
                    break;

                case Keys.Up:
                case Keys.Down:
                    eu = false;
                    ed = false;
                    break;

                case Keys.Left:
                case Keys.Right:
                    au = false;
                    ad = false;
                    break;
            }
        }


        void Graphic_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        void HUD()
        {
            string str = "Targrt X:" + cam.Target.X + " Y:" + cam.Target.Y + " Z:" + cam.Target.Z +
                "\nPosition X:" + cam.Position.X + " Y:" + cam.Position.Y + " Z:" + cam.Position.Z;
            font.DrawText(null, str, new Point(10, 10), Color.White);
        }
        void Render()
        {

            Graphic.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Purple, 1F, 1);
            Graphic.BeginScene();
            HUD();
            cam.Render(Graphic);
            //ground.Render(Graphic);

            //pilot.Render();
            airplane.Render();

            Graphic.EndScene();
            Graphic.Present();
        }

        //////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////
        void Frame()
        {
            if (l)
                airplane.Rudderleft(0.1f);
            if (r)
                airplane.RudderRight(0.1f);
            if (!l && !r)
                airplane.ZeroRudder(0.1f);
            if (f)
                airplane.FlapDown(0.1f);
            else
                airplane.FlapUp(0.1f);

            if (eu)
                airplane.ElevatorUp(0.1f);
            if (ed)
                airplane.ElevatorDown(0.1f);
            if (!eu && !ed)
                airplane.ZeroElavator(0.1f);
            if (au)
                airplane.AlironLeft(0.1f);
            if (ad)
                airplane.AlironRight(0.1f);
            if (!ad && !au)
                airplane.ZeroAliron(0.1f);
        }


        [STAThread]
        public static void Main()
        {
            using (MainClass frm = new MainClass())
            {
                frm.Init();
                frm.InitGeometry();
                frm.Show();
                while (frm.Created)
                {
                    frm.Frame();
                    frm.Render();
                    Application.DoEvents();
                }
            }
        }
    }
}
