//#define _VECTORS

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
        Camera cam;
        Terrain terrain;
        int framecntr = 0;
        int FPS = 0;
        Timer timerFPS;
        IndexedNormal pilot;
        Airplane C172;
        Framework.Line lForce, lLift, lVelBody, lvel;
        Vector3[] vcts;
        SkyBox skybox;

        //SphereDummy sphere;
        bool b_Paused = false;

        bool l = false;
        bool r = false;

        bool f = false;

        bool eu = false;
        bool ed = false;

        bool au = false;
        bool ad = false;

        int CamNumber = 0;

        string RootDirectory = Application.StartupPath;

        void Init()
        {
            this.Text = "Flight Simulator";
            this.Icon = new Icon(RootDirectory + "\\..\\..\\Icon1.ico");
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.KeyDown += new KeyEventHandler(MainClass_KeyDown);
            this.KeyUp += new KeyEventHandler(MainClass_KeyUp);
            this.MouseMove += new MouseEventHandler(MainClass_MouseMove);
            this.MouseDown += new MouseEventHandler(MainClass_MouseDown);
            this.StartPosition = FormStartPosition.Manual;

            PresentParameters pp = new PresentParameters();
            pp.BackBufferFormat = Manager.Adapters[0].CurrentDisplayMode.Format;
            //pp.BackBufferFormat = Format.X8R8G8B8;
            pp.AutoDepthStencilFormat = DepthFormat.D24X8;
            pp.EnableAutoDepthStencil = true;
            pp.BackBufferCount = 1;
            pp.Windowed = true;//false
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = 800;
            pp.BackBufferHeight = 600;
            //pp.MultiSample = MultiSampleType.FourSamples;

            int qty, res;

            /**/if(!
            Manager.CheckDeviceMultiSampleType(0, DeviceType.Hardware, pp.BackBufferFormat, true, pp.MultiSample, out res, out qty))//;
            {
                MessageBox.Show("Not Supported MultiSampleType\n" + "result:" + res + "\nQualitylevel:" + qty);
                pp.MultiSample = MultiSampleType.None;
                pp.MultiSampleQuality = 0;
            }
            else
            {
                //MessageBox.Show("Supported MultiSampleType." + "result:" + res + "\nQualitylevel:" + qty);
            }
            //pp.MultiSampleQuality = qty - 1;

            Graphic = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, pp);
            Graphic.RenderState.ZBufferEnable = true;
            Graphic.RenderState.CullMode = Cull.CounterClockwise;
            Graphic.RenderState.Lighting = false;
            Graphic.RenderState.FogColor = Color.FromArgb(70, 70, 70);
            Graphic.RenderState.FogDensity = 1000;
            Graphic.RenderState.FogEnable = true;
            Graphic.RenderState.FogStart = 0;
            Graphic.RenderState.FogEnd = 10000;
            Graphic.RenderState.FogVertexMode = FogMode.Linear;
            Graphic.DeviceResizing += new System.ComponentModel.CancelEventHandler(Graphic_DeviceResizing);
            Graphic.SamplerState[0].MagFilter = TextureFilter.Linear;
            Graphic.SamplerState[0].MinFilter = TextureFilter.Linear;

            //Graphic.RenderState.FillMode = FillMode.WireFrame;
            SetupLights();
            timerFPS = new Timer();
            timerFPS.Interval = 1000;
            timerFPS.Tick += new EventHandler(timerFPS_Tick);
            timerFPS.Start();
        }

        void timerFPS_Tick(object sender, EventArgs e)
        {
            FPS = framecntr;
            framecntr = 0;
        }

        private void SetupLights()
        {
            Vector3 dir = new Vector3(-0.5f, -1, -1);
            dir.Normalize();
            Graphic.Lights[0].Type = LightType.Directional;
            Graphic.Lights[0].Diffuse = Color.White;
            Graphic.Lights[0].Ambient = Color.FromArgb(64, 64, 64);
            Graphic.Lights[0].Specular = Color.White;
            Graphic.Lights[0].Direction = dir;
            Graphic.Lights[0].Enabled = true;

            Graphic.RenderState.Ambient = Color.Black;
            Graphic.RenderState.Lighting = true;
        }

        void InitGeometry()
        {
            C172 = new Airplane(Graphic,RootDirectory + "\\..\\..\\Pathes.txt");
            C172.SetKey0();

            terrain = new Terrain(RootDirectory + "\\..\\..\\terrain.txt", "..\\..\\GroundObjctsPath.txt");
            skybox = new SkyBox(Graphic);
            vcts = new Vector3[8];
            lvel = new Framework.Line(Vector3.Empty, Vector3.Empty, Color.Gray);
            lVelBody = new Framework.Line(Vector3.Empty, Vector3.Empty, Color.White);
            lForce = new Framework.Line(Vector3.Empty, Vector3.Empty, Color.Black);
            lLift = new Framework.Line(Vector3.Empty, Vector3.Empty, Color.Blue);

            cam = new Camera();
            cam.Position = new Vector3(0, 5, -5);
            cam.Position = Airplane.vPosition;
            cam.Position.Z -= 4;
            cam.fFareplane = 10000;
            cam.SetProjection();
            font = new Microsoft.DirectX.Direct3D.Font(Graphic, new System.Drawing.Font("lucida console", 10, FontStyle.Bold));

            Airplane = new RigidBody();
            InitializeAirplane();
            CalcAirplaneMassProperties();
            pilot = new IndexedNormal("Pilot",RootDirectory + "\\..\\..\\SkyHAwk\\Pilot.txt", RootDirectory + "\\..\\..\\SkyHawk\\pilot.jpg");

            HUDVerts[0].Position = new Vector4(5, 5, 0, 1);
            HUDVerts[1].Position = new Vector4(200, 5, 0, 1);
            HUDVerts[2].Position = new Vector4(5, 150, 0, 1);
            HUDVerts[3].Position = new Vector4(200, 150, 0, 1);

            HUDVerts[0].Color =
                HUDVerts[1].Color =
                HUDVerts[2].Color =
                HUDVerts[3].Color = Color.FromArgb(128, Color.White).ToArgb();
            //sphere = new SphereDummy(Color.Yellow);
            //sphere.Radius = 3;
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
            if (e.Button == MouseButtons.Right ||
                e.Button == MouseButtons.Left)
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

                case Keys.W:
                    if (Graphic.RenderState.FillMode == FillMode.Solid)
                        Graphic.RenderState.FillMode = FillMode.WireFrame;
                    else Graphic.RenderState.FillMode = FillMode.Solid;
                    break;

                case Keys.Add:
                    IncThrust(); break;
                case Keys.Subtract:
                    DecThrust(); break;


                    //////////////////////////////
                case Keys.X:
                    RightRudder();
                    l = true;
                    r = false;
                    break;
                case Keys.C:
                    LeftRudder();
                    l = false;
                    r = true;
                    break;
                case Keys.F:
                    f ^= true;
                    break;

                case Keys.Down:
                    PitchUp();
                    eu = true;
                    ed = false;
                    break;
                case Keys.Up:
                    PitchDown();
                    eu = false;
                    ed = true;
                    break;

                case Keys.Right:
                    RollLeft();
                    au = true;
                    ad = false;
                    break;
                case Keys.Left:
                    RollRight();
                    au = false;
                    ad = true;
                    break;

                case Keys.F1:
                    CamNumber = 0; break;
                case Keys.F2:
                    CamNumber = 1; break;
                case Keys.F3:
                    CamNumber = 2; break;
                case Keys.F4:
                    CamNumber = 3; break;

                case Keys.P:
                    b_Paused = !b_Paused;
                    break;
            }
        }
        void MainClass_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.C:
                case Keys.X:
                    ZeroRudder();
                    l = false;
                    r = false;
                    break;

                case Keys.Up:
                case Keys.Down:
                    ZeroElevator();
                    eu = false;
                    ed = false;
                    break;

                case Keys.Left:
                case Keys.Right:
                    ZeroAilerons();
                    au = false;
                    ad = false;
                    break;
            }
        }

        void Graphic_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        CustomVertex.TransformedColored[] HUDVerts = new CustomVertex.TransformedColored[4];
        void HUD()
        {
            string str = "FPS: " + FPS +
                //"\n\nTargrt X:" + (int)cam.Target.X + " Y:" + cam.Target.Y + " Z:" + (int)cam.Target.Z +
                //"\nPosition X:" + (int)cam.Position.X + " Y:" + (int)cam.Position.Y + " Z:" + (int)cam.Position.Z +
                "\n\nAllt:" + (int)cam.Target.Y +
                "\nSpeed:" + (int)SpeedConvertor.Ft_sec2Knot(Airplane.fSpeed) +
                "\nThrust:%" + (int)(ThrustForce / _MAXTHRUST * 100) +
                "\nG:" + Math.Round(Airplane.vAcceleration.Length() / -g, 1) +
                "\n\nCollision: " + CheckCollision(ref Airplane);
            if (Flaps) str += "\nFlaps";
            if (Stalling) str += "\nStalling";

            Graphic.SetTextureStageState(0, TextureStageStates.AlphaArgument1, (int)TextureArgument.Diffuse);
            Graphic.SetTextureStageState(0, TextureStageStates.AlphaOperation, (int)TextureOperation.SelectArg1);
            Graphic.SetRenderState(RenderStates.SourceBlend, (int)Blend.SourceAlpha);
            Graphic.SetRenderState(RenderStates.DestinationBlend, (int)Blend.InvSourceAlpha);
            Graphic.RenderState.AlphaBlendEnable = true;
            Graphic.VertexFormat = CustomVertex.TransformedColored.Format;
            Graphic.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, HUDVerts);

            font.DrawText(null, str, new Point(10, 10), Color.White);

            System.Drawing.Font sysFont = new System.Drawing.Font("lucida console", 10, FontStyle.Bold);
            if(b_Paused)
                font.DrawText(null, "--Paused--", new Point(350, 300), Color.Blue);
        }
        void Render()
        {

            Graphic.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Purple, 1F, 1);
            Graphic.BeginScene();

            Graphic.RenderState.Lighting = false;
            cam.Render(Graphic);
            skybox.Render(cam);
            //sphere.Position = Airplane.vPosition;
            //sphere.Render(Graphic);
            Graphic.RenderState.Lighting = true;

            terrain.Render(cam);
            //Blur();
            pilot.Render(matWorld);
            C172.Render(matWorld);

#if _VECTORS
            for (int i = 0; i < 8; ++i)
                lLift.Render(Graphic, vcts[i] * 0.01f, Element[i].vCGCoords * 0.1f);
            lForce.Render(Graphic, vcts[7] * 0.01f, Vector3.Empty);
            lVelBody.Render(Graphic, Airplane.vVelocityBody * 0.01f, Vector3.Empty);
            lvel.Render(Graphic, Airplane.vVelocity * 0.01f, Vector3.Empty);
#endif

            HUD();
            Graphic.EndScene();
            Graphic.Present();

            framecntr++;
        }
        void Blur()
        {
            Sections propeller = C172.GetSectionByName("Propeller");
            for (int i = 0; i < 5; ++i)
            {
                propeller.SetLocalMatrix(Matrix.RotationZ((Environment.TickCount + 1 * i) * 3.14f ) * Matrix.Translation(propeller.GetKeyFrameByName("Rest").Pos));
                propeller.Render(matWorld);
            }
        }

        //////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////
        PointMass[] Element;
        RigidBody Airplane;
        Vector3 Thrust;
        float ThrustForce = 0;
        bool Stalling;
        bool Flaps;
        const float rho = 2.37E-3f;
        const float g = -32.174f;
        Vector3 vPosition;
        Quaternion qOrientation;
        Matrix matWorld;

        float Swet = 160;
        float S = 120 * 0.092903f;
        float L = 12.5f * 0.3048f;
        float L_D = 12.5f / 5.0f;

        void InitializeAirplane()
        {
            float iRoll, iPitch, iYaw;

            // Set initial position
            Airplane.vPosition = new Vector3(15000, 500, 5000);

            // Set initial velocity
            Airplane.vVelocity = new Vector3(0,0,100);
            Airplane.fSpeed = 100;

            // Set initial angular velocity
            Airplane.vAngularVelocity = new Vector3(0, 0, 0);

            // Set the initial thrust, forces, anfd moments
            Airplane.vForces = new Vector3(0, 0, 500);
            ThrustForce = 500;

            Airplane.vMoments = new Vector3(0, 0, 0);

            // Zero the velocity in body space coordinates
            Airplane.vVelocityBody = new Vector3(0, 0, 0);

            // Set these to false at first,
            //you can control later using the keyboard
            Stalling = false;
            Flaps = false;

            // Set the initial orientation
            iRoll = 0;
            iPitch = 0;
            iYaw = -3.14f/2.0f;
            Airplane.qOrientation = Quaternion.RotationYawPitchRoll(iYaw, iPitch, iRoll);
            Airplane.vVelocity.TransformCoordinate(Matrix.RotationQuaternion(Airplane.qOrientation));
            // Now go ahead and calculate the plane's mass properties
            CalcAirplaneMassProperties();
        }
        private void CalcAirplaneMassProperties()
        {
            float mass;
            Vector3 vMoment;
            Vector3 CG;
            float Ixx, Iyy, Izz, Ixy, Ixz, Iyz;
            float ind, dih;

            // Initialize the elements here
            // Initially, the coordinates of each element are referenced from
            // a design coordinates system located at the very tail end of the plane,
            // its baseline, and its centerline. Later, these coordinates will be
            // adjusted so that each element is referenced to the combined center of
            // gravity of the airplane.
            Element = new PointMass[8];

            Element[0].fMass = 6.56f;
            Element[0].vDCoords = new Vector3(12, 2.5f, 14.5f);
            Element[0].vLocalInertia = new Vector3(10.5f, 24, 13.92f);
            Element[0].fIncidence = -3.5f;
            Element[0].fDihedral = 0.0f;
            Element[0].fArea = 31.2f;
            Element[0].iFlap = 0;

            Element[1].fMass = 7.31f;
            Element[1].vDCoords = new Vector3(5.5f, 2.5f, 14.5f);
            Element[1].vLocalInertia = new Vector3(12.22f, 33.67f, 21.95f);
            Element[1].fIncidence = -3.5f;
            Element[1].fDihedral = 0.0f;
            Element[1].fArea = 36.4f;
            Element[1].iFlap = 0;

            Element[2].fMass = 7.31f;
            Element[2].vDCoords = new Vector3(-5.5f, 2.5f, 14.5f);
            Element[2].vLocalInertia = new Vector3(12.22f, 33.67f, 21.95f);
            Element[2].fIncidence = -3.5f;
            Element[2].fDihedral = 0.0f;
            Element[2].fArea = 36.4f;
            Element[2].iFlap = 0;

            Element[3].fMass = 6.56f;
            Element[3].vDCoords = new Vector3(-12, 2.5f, 14.5f);
            Element[3].vLocalInertia = new Vector3(10.5f, 24, 13.92f);
            Element[3].fIncidence = -3.5f;
            Element[3].fDihedral = 0.0f;
            Element[3].fArea = 31.2f;
            Element[3].iFlap = 0;

            Element[4].fMass = 2.62f;
            Element[4].vDCoords = new Vector3(2.5f, 3, 3.03f);
            Element[4].vLocalInertia = new Vector3(0.385f, 1.206f, 0.837f);
            Element[4].fIncidence = 0.0f;
            Element[4].fDihedral = 0.0f;
            Element[4].fArea = 10.8f;
            Element[4].iFlap = 0;

            Element[5].fMass = 2.62f;
            Element[5].vDCoords = new Vector3(-2.5f, 3f, 3.03f);
            Element[5].vLocalInertia = new Vector3(0.385f, 1.206f, 0.837f);
            Element[5].fIncidence = 0.0f;
            Element[5].fDihedral = 0.0f;
            Element[5].fArea = 10.8f;
            Element[5].iFlap = 0;

            Element[6].fMass = 2.93f;
            Element[6].vDCoords = new Vector3(0, 5, 2.25f);
            Element[6].vLocalInertia = new Vector3(1.942f, 0.718f, 1.262f);
            Element[6].fIncidence = 0.0f;
            Element[6].fDihedral = 90.0f;
            Element[6].fArea = 12.0f;
            Element[6].iFlap = 0;

            Element[7].fMass = 31.8f;
            Element[7].vDCoords = new Vector3(0, 1.5f, 15.25f);
            Element[7].vLocalInertia = new Vector3(861.9f, 861.9f, 66.3f);
            Element[7].fIncidence = 0.0f;
            Element[7].fDihedral = 0.0f;
            Element[7].fArea = 84.0f;
            Element[7].iFlap = 0;

            // Calculate the vector normal (perpendicular) to each lifting surface.
            // This is required when calculating the relative air velocity for
            // lift and drag calculations.
            for (int i = 0; i < 8; ++i)
            {
                ind = Geometry.DegreeToRadian(Element[i].fIncidence);
                dih = Geometry.DegreeToRadian(Element[i].fDihedral);
                Element[i].vNormal = new Vector3
                    ((float)(Math.Cos(ind) * Math.Sin(dih)),
                    (float)(Math.Cos(ind) * Math.Cos(dih)),
                    (float)Math.Sin(ind));
                Element[i].vNormal.Normalize();
            }

            // Calculate total mass
            mass = 0;
            for (int i = 0; i < 8; ++i)
                mass += Element[i].fMass;

            // Calculate combnied center of gravity location
            vMoment = new Vector3(0, 0, 0);
            for (int i = 0; i < 8; ++i)
                vMoment += Element[i].fMass * Element[i].vDCoords;
            CG = vMoment * (1.0f / mass);

            // Calculate coordinates of each element with respect to the combined CG
            for (int i = 0; i < 8; ++i)
                Element[i].vCGCoords = Element[i].vDCoords - CG;

            // Now calculate the moments and products of inertia for the
            // combined elements
            // (This inertia matrix (tensor) is in body coordinates)
            Ixx = 0; Iyy = 0; Izz = 0;
            Ixy = 0; Ixz = 0; Iyz = 0;
            for (int i = 0; i < 8; ++i)
            {
                Ixx += Element[i].vLocalInertia.X + Element[i].fMass *
                    (Element[i].vCGCoords.Y * Element[i].vCGCoords.Y +
                    Element[i].vCGCoords.Z * Element[i].vCGCoords.Z);
                Iyy += Element[i].vLocalInertia.Y + Element[i].fMass *
                    (Element[i].vCGCoords.Z * Element[i].vCGCoords.Z +
                    Element[i].vCGCoords.X * Element[i].vCGCoords.X);
                Izz += Element[i].vLocalInertia.Z + Element[i].fMass *
                    (Element[i].vCGCoords.X * Element[i].vCGCoords.X +
                    Element[i].vCGCoords.Y * Element[i].vCGCoords.Y);
                Ixy += Element[i].fMass * (Element[i].vCGCoords.X *
                    Element[i].vCGCoords.Y);
                Ixz += Element[i].fMass * (Element[i].vCGCoords.X *
                    Element[i].vCGCoords.Z);
                Iyz += Element[i].fMass * (Element[i].vCGCoords.Y *
                    Element[i].vCGCoords.Z);
            }

            // Finally, set up the airplane's mass and its inertia matrix and take the
            // inverse of the inertia matrix
            Airplane.fMass = mass;
            Airplane.fMassInverse = 1 / mass;
            Airplane.mInertia = Matrix.Identity;
            Airplane.mInertia.M11 = Ixx;
            Airplane.mInertia.M12 = -Ixy;
            Airplane.mInertia.M13 = -Ixz;
            Airplane.mInertia.M21 = -Ixy;
            Airplane.mInertia.M22 = Iyy;
            Airplane.mInertia.M23 = -Iyz;
            Airplane.mInertia.M31 = -Ixz;
            Airplane.mInertia.M32 = -Iyz;
            Airplane.mInertia.M33 = Izz;

            Airplane.mInertiaInverse = Matrix.Invert(Airplane.mInertia);
        }

        float LiftCoefficient(float angle, int flaps)
        {
            float[] clf0 = new float[] { -0.54f, -0.2f, 0.2f, 0.57f, 0.92f, 1.21f, 1.43f, 1.4f, 1.0f };
            float[] clfd = new float[] { 0.0f, 0.45f, 0.85f, 1.02f, 1.39f, 1.65f, 1.75f, 1.38f, 1.17f };
            float[] clfu = new float[] { -0.74f, -0.4f, 0.0f, 0.27f, 0.63f, 0.92f, 1.03f, 1.1f, 0.78f };
            float[] a = new float[] { -8.0f, -4.0f, 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };
            float cl = 0;
            for (int i = 0; i < 8; ++i)
            {
                if (a[i] <= angle && a[i + 1] > angle)
                {
                    switch (flaps)
                    {
                        case 0:// flaps not deflected
                            cl = clf0[i] - (a[i] - angle) * (clf0[i] - clf0[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                        case -1:// flaps down
                            cl = clfd[i] - (a[i] - angle) * (clfd[i] - clfd[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                        case 1:// flaps up
                            cl = clfu[i] - (a[i] - angle) * (clfu[i] - clfu[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                    }
                    break;
                }
            }
            return cl;
        }
        float DragCoefficint(float angle, int flaps)
        {
            float[] cdf0 = new float[] { 0.01f, 0.0074f, 0.004f, 0.009f, 0.013f, 0.023f, 0.05f, 0.12f, 0.21f };
            float[] cdfd = new float[] { 0.0065f, 0.0043f, 0.0055f, 0.0153f, 0.0221f, 0.0391f, 0.1f, 0.195f, 0.3f };
            float[] cdfu = new float[] { 0.005f, 0.0043f, 0.0055f, 0.02601f, 0.03757f, 0.06647f, 0.13f, 0.18f, 0.25f };
            float[] a = new float[] { -8.0f, -4.0f, 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };

            float cd = 0.5f;
            for (int i = 0; i < 8; ++i)
            {
                if(a[i] <= angle && a[i+1] > angle)
                {
                    switch (flaps)
                    {
                        case 0://flaps not defected
                            cd = cdf0[i] - (a[i] - angle) * (cdf0[i] - cdf0[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                        case -1:// flaps down
                            cd = cdfd[i] - (a[i] - angle) * (cdfd[i] - cdfd[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                        case 1:// flaps up
                            cd = cdfu[i] - (a[i] - angle) * (cdfu[i] - cdfu[i + 1]) /
                                (a[i] - a[i + 1]);
                            break;
                    }
                    break;
                }
            }
            return cd;
        }

        float RudderLiftCoefficient(float angle)
        {
            float[] clf0 = new float[] { 0.16f, 0.456f, 0.736f, 0.968f, 1.144f, 1.12f, 0.8f };
            float[] a = new float[] { 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };
            float cl = 0;
            float aa = Math.Abs(angle);
            for (int i = 0; i < 6; ++i)
            {
                if (a[i] <= aa && a[i + 1] > aa)
                {
                    cl = clf0[i] - (a[i] - aa) * (clf0[i] - clf0[i + 1]) /
                        (a[i] - a[i + 1]);
                    if (angle < 0) cl = -cl;
                    break;
                }
            }
            return cl;
        }
        float RudderDragCoefficient(float angle)
        {
            float[] cdf0 = new float[] { 0.0032f, 0.0072f, 0.0104f, 0.0184f, 0.04f, 0.096f, 0.168f };
            float[] a = new float[] { 0.0f, 4.0f, 8.0f, 12.0f, 16.0f, 20.0f, 24.0f };
            float cd = 0.5f;
            float aa = Math.Abs(angle);
            for (int i = 0; i < 6; ++i)
            {
                if (a[i] <= aa && a[i + 1] > aa)
                {
                    cd = cdf0[i] - (a[i] - aa) * (cdf0[i] - cdf0[i + 1]) /
                        (a[i] - a[i + 1]);
                    break;
                }
            }
            return cd;
        }


        void CalcAirplaneLoads(ref RigidBody AirPlane)
        {
            Vector3 Fb, Mb;

            // reset forces and moments:
            AirPlane.vForces = new Vector3(0, 0, 0);
            AirPlane.vMoments = new Vector3(0, 0, 0);
            Fb = new Vector3(0, 0, 0);
            Mb = new Vector3(0, 0, 0);

            // Define the thrust vector,wich acts through the plane's CG
            Thrust = new Vector3(0, 0, ThrustForce);

            // Calculate forces and moments in body space:
            Vector3 vLocalVelocity;
            float fLocalSpeed;
            Vector3 vDragVector = new Vector3(0, 0, 0);
            Vector3 vliftVector;
            float fAttackAngle;
            float tmp;
            Vector3 vResultant;
            Vector3 vtmp;

            Stalling = false;

            for (int i = 0; i < 7; ++i) // loop through the seven lifting elements
                                        // skipping the fuselage
            {
                if (i == 6) // The tail/rudder id a special case, since it can rotate;
                {           // therfore, you have to recalculate the normal vector
                    float ind, dih;
                    ind = Geometry.DegreeToRadian(Element[i].fIncidence); // incidence angle
                    dih = Geometry.DegreeToRadian(Element[i].fDihedral);//dihedral angle
                    Element[i].vNormal = new Vector3
                    ((float)(Math.Cos(ind) * Math.Sin(dih)),
                    (float)(Math.Cos(ind) * Math.Cos(dih)),
                    (float)Math.Sin(ind));
                    Element[i].vNormal.Normalize();
                }

                // Calculate local velocity at element
                // The local velocity includes the velocity due to linear
                // motion of the airplane,
                // plus the velocity at each element due to the
                // rotation of the airplane

                // Here's the rotational part
                vtmp = Vector3.Cross(AirPlane.vAngularVelocity, Element[i].vCGCoords);

                vLocalVelocity = AirPlane.vVelocityBody + vtmp;

                // Calculate local air speed
                fLocalSpeed = vLocalVelocity.Length();

                // Find the direction in wich drag will act.
                // Drag always acts inline with the relative
                // velocity but in the opposing direction
                if (fLocalSpeed > 1.0f)
                    vDragVector = -vLocalVelocity * (1.0f / fLocalSpeed);

                // Find the direction in which lift will act.
                // Lift is always perpendicular to the drag vector
                vliftVector = Vector3.Cross(Vector3.Cross(vDragVector, Element[i].vNormal), vDragVector);
                tmp = vliftVector.Length();
                vliftVector.Normalize();

                // Find the angle of attack.
                // The attack angle is the angle between the lift vector and the
                // element normal vector. Note that the sine of the attack angle,
                // is equal to the cosine of the angle between the drag vector and
                // the normal vector.
                tmp = Vector3.Dot(vDragVector, Element[i].vNormal);
                if (tmp > 1.0f) tmp = 1;
                if (tmp < -1.0f) tmp = -1;
                fAttackAngle = Geometry.RadianToDegree((float)Math.Asin(tmp));

                // Determine the resultant force (lift and drag) on the element.
                tmp = 0.5f * rho * fLocalSpeed * fLocalSpeed * Element[i].fArea;
                if (i == 6)// Tail/rudder
                {
                    vResultant = (vliftVector * RudderLiftCoefficient(fAttackAngle) +
                        vDragVector * RudderDragCoefficient(fAttackAngle))
                        * tmp;
                }
                else
                {
                    vResultant = (vliftVector * LiftCoefficient(fAttackAngle,
                        Element[i].iFlap) +
                        vDragVector * (DragCoefficint(fAttackAngle,
                        Element[i].iFlap) + Cdof())) * tmp;
                }
                // Check for stall.
                // We can easily determine when stalled by noting when the coefficient
                // of lift is zero. In reality stall warning devices give warnings well
                // before the lift goes to zero to give the pilot time to correct.
                if (i <= 0)
                    if (LiftCoefficient(fAttackAngle, Element[i].iFlap) == 0)
                        Stalling = true;

                // Keep a running total of thes resultant forces (total force)
                Fb += vResultant;

                // Calculate the moment about the CG of this element's force
                // and keep a running total of these moments (total moment)
                vtmp = Vector3.Cross(Element[i].vCGCoords, vResultant);
                Mb += vtmp;

                vcts[i] = vResultant;
            }

            // Now add the thrust
            Fb += Thrust;

            // Convert forces from model space to earth space
            AirPlane.vForces = Fb;
            AirPlane.vForces.TransformCoordinate(Matrix.RotationQuaternion(AirPlane.qOrientation));

            // Apply gravity (g is -32.174 ft/s 2)
            AirPlane.vForces.Y += g * AirPlane.fMass;

            AirPlane.vMoments += Mb;

            vcts[7] = AirPlane.vForces;
        }
        void StepSimulation(float dt,ref RigidBody Airplane)
        {
            // Take care of translation first:
            // (If this body were a particle, this is all you would need to do.)

            Vector3 Ae;

            // calculate all of the forces and moments on the airplane:
            CalcAirplaneLoads(ref Airplane);

            // calculate the acceleration of the airplane in earth space:
            Ae = Airplane.vForces * (1.0f / Airplane.fMass);
            Airplane.vAcceleration = Ae;
            // calculate the velocity of the plane in earth space
            Airplane.vVelocity += Ae * dt;

            // calculete the position  of the airplane in earth space:
            Airplane.vPosition += Airplane.vVelocity * dt;

            // Now handle the rotations:
            float mag;

            // calculate the angular velocity of the airplane in body space:
            Vector3 Iw = Airplane.vAngularVelocity;
            Iw.TransformCoordinate(Airplane.mInertia);
            Vector3 alpha = Vector3.Cross(Airplane.vAngularVelocity, Iw);
            alpha = Airplane.vMoments - alpha;
            alpha.TransformCoordinate(Airplane.mInertiaInverse);
            Airplane.vAngularVelocity += alpha * dt;

            // calculate the new rotation quaternion:
            Quaternion q =
                new Quaternion(
                Airplane.vAngularVelocity.X / 2 * dt,
                Airplane.vAngularVelocity.Y / 2 * dt,
                Airplane.vAngularVelocity.Z / 2 * dt,
                0);
            Airplane.qOrientation += q * Airplane.qOrientation;

            // now normalize the orientation quternion:
            mag = Airplane.qOrientation.Length();
            //if (mag != 0)
            //    Airplane.qOrientation /= mag;
            Airplane.qOrientation.Normalize();

            // calculate the velocity in body space:
            // (we'll need this to calculate lift and drag forces)
            Airplane.vVelocityBody = Airplane.vVelocity;
            Airplane.vVelocityBody.TransformCoordinate(Matrix.RotationQuaternion(Quaternion.Conjugate(Airplane.qOrientation)));

            // calculate the air speed:
            Airplane.fSpeed = Airplane.vVelocity.Length();

            // get the Eular angles for our information
            Vector3 u;
            u = MakeEulerAnglesFromQ(Airplane.qOrientation);
            Airplane.vEulerAngles = u;
        }

        private Vector3 MakeEulerAnglesFromQ(Quaternion quaternion)
        {
            return Vector3.Empty;
        }

        const float _DTHRUST = 100;
        const float _MAXTHRUST = 3000;

        void IncThrust()
        {
            ThrustForce += _DTHRUST;
            if (ThrustForce > _MAXTHRUST)
                ThrustForce = _MAXTHRUST;
        }
        void DecThrust()
        {
            ThrustForce -= _DTHRUST;
            if (ThrustForce < 0)
                ThrustForce = 0;
        }

        void LeftRudder()
        {
            Element[6].fIncidence = 16;
        }
        void RightRudder()
        {
            Element[6].fIncidence = -16;
        }
        void ZeroRudder()
        {
            Element[6].fIncidence = 0;
        }

        void RollLeft()
        {
            Element[0].iFlap = 1;
            Element[3].iFlap = -1;
        }
        void RollRight()
        {
            Element[0].iFlap = -1;
            Element[3].iFlap = 1;
        }
        void ZeroAilerons()
        {
            Element[0].iFlap = 0;
            Element[3].iFlap = 0;
        }

        void PitchUp()
        {
            Element[4].iFlap = 1;
            Element[5].iFlap = 1;
        }
        void PitchDown()
        {
            Element[4].iFlap = -1;
            Element[5].iFlap = -1;
        }
        void ZeroElevator()
        {
            Element[4].iFlap = 0;
            Element[5].iFlap = 0;
        }

        void FlapsDown()
        {
            Element[1].iFlap = -1;
            Element[2].iFlap = -1;
            Flaps = true;
        }
        void ZeroFlaps()
        {
            Element[1].iFlap = 0;
            Element[2].iFlap = 0;
            Flaps = false;
        }

        void Frame()
        {
            if (!b_Paused)
            {
                Physics();

                vPosition = Airplane.vPosition;
                qOrientation = Airplane.qOrientation;

                matWorld = Matrix.RotationQuaternion(qOrientation) * Matrix.Translation(vPosition);

                if (f)
                {
                    FlapsDown();
                }
                else
                {
                    ZeroFlaps();
                }
                if (l)
                    C172.Rudderleft(0.1f);
                if (r)
                    C172.RudderRight(0.1f);
                if (!l && !r)
                    C172.ZeroRudder(0.1f);
                if (f)
                    C172.FlapDown(0.1f);
                else
                    C172.FlapUp(0.1f);

                if (eu)
                    C172.ElevatorUp(0.1f);
                if (ed)
                    C172.ElevatorDown(0.1f);
                if (!eu && !ed)
                    C172.ZeroElavator(0.1f);
                if (au)
                    C172.AlironLeft(0.1f);
                if (ad)
                    C172.AlironRight(0.1f);
                if (!ad && !au)
                    C172.ZeroAliron(0.1f);
                Sections propeller = C172.GetSectionByName("Propeller");

                propeller.SetLocalMatrix(Matrix.RotationZ((Environment.TickCount) * 3.14f * (-ThrustForce / _DTHRUST - 0.1f) / 180) * Matrix.Translation(propeller.GetKeyFrameByName("Rest").Pos));
            }
            UpdateCamera();
        }

        void UpdateCamera()
        {
            Vector3 campos = new Vector3(0, 0.7f, -4f);//-8
            campos = Vector3.TransformCoordinate(campos, Matrix.RotationQuaternion(cam.qOrientation));
            campos += vPosition;

            Vector3 camUp = new Vector3(0, 1, 0);
            camUp = Vector3.TransformCoordinate(camUp, Matrix.RotationQuaternion(cam.qOrientation));

            cam.qOrientation = Quaternion.Slerp(cam.qOrientation, qOrientation, 0.01f);
            switch (CamNumber)
            {
                case 0:
                    {
                        cam.Position = campos;
                        cam.Target = vPosition;
                        cam.Up = camUp;
                    }
                    break;

                case 1:
                    {
                        Vector3 vect = vPosition - cam.Target;
                        cam.Position += vect;
                        cam.Target = vPosition;
                        cam.Up = new Vector3(0, 1, 0);
                    }
                    break;
                case 2:
                    {
                        cam.Position = new Vector3(-0.1f, 0.3f, -0.25f);
                        cam.Target = new Vector3(0, 0.1f, 0.9f);
                        cam.Target.TransformCoordinate(Matrix.RotationQuaternion(qOrientation));
                        cam.Position.TransformCoordinate(Matrix.RotationQuaternion(qOrientation));
                        cam.Up = Vector3.TransformCoordinate(new Vector3(0, 1, 0), Matrix.RotationQuaternion(qOrientation));
                        cam.Position += vPosition;
                        cam.Target += vPosition;

                    }
                    break;

                default:
                    {
                        cam.Position = new Vector3(0, 0.7f, -4f);
                        cam.Target = new Vector3(0, 0, 0);

                        cam.Position.TransformCoordinate(Matrix.RotationQuaternion(qOrientation));
                        cam.Up = Vector3.TransformCoordinate(new Vector3(0, 1, 0), Matrix.RotationQuaternion(qOrientation));
                        cam.Position += vPosition;
                        cam.Target += vPosition;
                    }
                    break;
            }
        }
        void Physics()
        {
            Collision.CollisionStates check = Collision.CollisionStates.None;
            Airplane.vForces = Vector3.Empty;
            Airplane.vMoments= Vector3.Empty;

            RigidBody cpy = Airplane;
            StepSimulation(dt, ref cpy);
            check = CheckCollision(ref cpy);
            if (check == Collision.CollisionStates.None)
            {
                Airplane = cpy;
            }
            else
            {
                MyResolveCollision(ref Airplane);
            }
        }

        Vector3 vPlaneNormal = new Vector3(0, 1, 0);
        float dt = 0.01f, dtime = 0;
        Vector3 vCollitionNormal;
        Vector3 vRelativVelocity;
        bool TryAgain;
        bool didPen = false;

        private void ResolveCollition(ref RigidBody Body)
        {
            dtime = dt;
            TryAgain = true;
            RigidBody copy = Body;
            didPen = false;

            while (TryAgain && dtime > Constants.TimeTolerance)
            {
                TryAgain = false;
                copy = Body;

                Collision.CollisionStates check = CheckCollision(ref copy);
                if (check == Collision.CollisionStates.Contact)
                {
                    copy.vForces = Vector3.Empty;
                    copy.vMoments = Vector3.Empty;

                    CalcAirplaneLoads(ref copy);
                    Vector3 ContactForce = copy.fMass * -copy.vAcceleration;
                    copy.vForces += ContactForce;
                    Move(dtime, ref copy);
                    didPen = false;
                }
                else
                {
                    StepSimulation(dtime, ref copy);

                    check = CheckCollision(ref copy);
                    if (check == Collision.CollisionStates.Penetrating)
                    {
                        dtime = dtime / 2;
                        TryAgain = true;
                        didPen = true;
                    }
                    else
                        if (check == Collision.CollisionStates.Collision ||
                            check == Collision.CollisionStates.Contact)
                        {
                            ApplyImpulse(ref copy);
                            didPen = false;
                        }
                        else
                        {
                            didPen = false;
                        }
                }
            }

            if (!didPen)
            {
                Body = copy;
            }
        }

        private void MyResolveCollision(ref RigidBody Body)
        {
            dtime = dt;
            RigidBody copy = Body;
            if (CheckCollision(ref copy) == Collision.CollisionStates.Contact)
            {
                ApplyContact(ref copy);
                TryAgain = false;
                didPen = false;
            }
            else
            {
                didPen = true;
                TryAgain = true;
            }
            while (TryAgain && dtime > Constants.TimeTolerance)
            {
                dtime = dtime / 2;
                copy = Body;
                StepSimulation(dtime, ref copy);
                Collision.CollisionStates check = CheckCollision(ref copy);
                if (check == Collision.CollisionStates.None)
                {
                    TryAgain = false;
                    didPen = false;
                }
                else
                {
                    if (check == Collision.CollisionStates.Collision)
                    {
                        ApplyImpulse(ref copy);
                        TryAgain = false;
                        didPen = false;
                    }
                    if (check == Collision.CollisionStates.Contact)
                    {
                        ApplyContact(ref copy);
                        TryAgain = false;
                        didPen = false;
                    }
                    if (check == Collision.CollisionStates.Penetrating )
                    {
                        TryAgain = true;
                        didPen = true;
                    }
                }
            }

            if (!didPen)
            {
                Body = copy;
            }
        }

        void Move(float dt, ref RigidBody body)
        {
            // Linear Part of Motion
            body.vAcceleration = body.vForces * body.fMassInverse;
            body.vVelocity += body.vAcceleration * dt;
            body.vPosition += body.vVelocity * dt;

            // Angular Part of Motion
            Vector3 tmp = Vector3.TransformCoordinate(body.vAngularVelocity, body.mInertia);
            tmp = Vector3.Cross(body.vAngularVelocity, tmp);
            tmp = body.vMoments - tmp;
            tmp.TransformCoordinate(body.mInertiaInverse);
            body.vAngularVelocity += tmp * dt;

            Quaternion q = new Quaternion(body.vAngularVelocity.X / 2 * dt, body.vAngularVelocity.Y / 2 * dt, body.vAngularVelocity.Z / 2 * dt, 0);
            body.qOrientation += q * body.qOrientation;
            body.qOrientation.Normalize();

            body.vVelocityBody = body.vVelocity;
            body.vVelocityBody.TransformCoordinate(Matrix.RotationQuaternion(Quaternion.Conjugate(body.qOrientation)));

        }

        void CalcAcceleration(ref RigidBody body)
        {
            body.vAcceleration = body.vForces * body.fMassInverse;
        }

        Collision.CollisionStates CheckCollision(ref RigidBody Body)
        {
            float s = Body.vPosition.Y;
            float Vrn = Vector3.Dot(Body.vVelocity, vPlaneNormal);
            vCollitionNormal = vPlaneNormal;
            vRelativVelocity = Body.vVelocity;// + Vector3.Cross(new Vector3(0, 1f, 0), Body.vAngularVelocity);
            if (Math.Abs(s) <= Constants.DistanceTolerance && Vrn < -Constants.VelocityTolerance)
            {
                return Collision.CollisionStates.Collision;
            }
            else
                if (Math.Abs(s) <= Constants.DistanceTolerance && Math.Abs(Vrn) <= Constants.VelocityTolerance)
                {
                    return Collision.CollisionStates.Contact;
                }
                else
                {
                    if (s < -Constants.DistanceTolerance)
                        return Collision.CollisionStates.Penetrating;
                }
            return Collision.CollisionStates.None;
        }
        void ApplyImpulse(ref RigidBody Body)
        {

            float j = -Vector3.Dot(vRelativVelocity, vCollitionNormal) * (1 + Constants.e);
            j /= Vector3.Dot(vCollitionNormal, vCollitionNormal) * (Body.fMassInverse);

            Body.vVelocity += j * vCollitionNormal * Body.fMassInverse;
        }
        void ApplyContact(ref RigidBody Body)
        {
            CalcAirplaneLoads(ref Body);
            Body.vForces += Body.vAcceleration * -Body.fMass;
            //if (Body.vForces.Y < 0)
                Body.vForces.Y = -g * Body.fMass;
            Move(dtime, ref Body);
        }

        // calculate fuselage drag coefficient
        float Cdof()
        {
            // Cf   : surface drag coefficient
            // fLD  : Lenth over Body
            // fM   : Mach function
            // Swet : wet surface
            // S    : wing Surface
            return Cf() * fLD() * fM() * (Swet / S);
        }

        private float Cf()
        {
            // R : Reynolds Number
            return 0.455f / (float)Math.Pow(Math.Log10(R()),2.58);
        }

        private double R()
        {
            // L : Length
            // V : actual speed
            // v : kinematic air lesjet
            return L * SpeedConvertor.Ft_sec2m_sec(Airplane.vVelocityBody.Length()) / v();
        }

        private float v()
        {
            // u   : dynamic air lesjet
            // roh : air dencity
            return Constants.u0kg_sec_m / Constants.AirDencity0kg_m3;
        }

        private float fLD()
        {
            // L : Lenght
            // D : maximum diameter
            // B : speed variable
            return 1 + (( 60 /(L_D * L_D * L_D)) + (0.0025f * L_D)) / B();
        }

        private float B()
        {
            float b = 0;
            float M = SpeedConvertor.Mach_ft_sec(Airplane.vVelocityBody.Length());
            if (M <= 0.9f)
                b = (float)Math.Sqrt(1 - M * M);
            else
                if (0.9f < M && M < 1.1f)
                    b = 0.44f;
                else
                    if (M >= 1.1)
                        b = 1;
            return b;
        }

        private float fM()
        {
            return 1 - 0.08f * (float)Math.Pow(SpeedConvertor.Mach_ft_sec(Airplane.vVelocityBody.Length()), 1.48f);
        }

        [STAThread]
        public static void Main()
        {
            try
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
            catch (Exception e)
            {
                MessageBox.Show("Error :\nMessage:\n" + e.Message + "\n\n\nData:\n" +e.Data + "\n\n\nSource:\n"+e.Source+"\n\n\nStackTrack:\n"+e.StackTrace);
            }
        }
    }
}
