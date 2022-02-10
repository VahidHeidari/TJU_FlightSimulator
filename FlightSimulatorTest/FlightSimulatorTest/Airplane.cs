using System;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Framework
{
    struct Keyframe
    {
        public string Name;
        public float Time;
        public Vector3 Pos;
        public Vector3 Scale;
        public Quaternion Quat;

        public Keyframe(string name,float time,Vector3 pos,Vector3 scale,Quaternion quat)
        {
            Name = name;
            Time = time;
            Pos = pos;
            Scale = scale;
            Quat = quat;
        }

        public static Matrix LerpKeyframe(Keyframe K0,Keyframe K1,float Time)
        {
            float t0 = K0.Time;
            float t1 = K1.Time;
            float LerpTime = (Time - t0) / (t1 - t0);
            if (LerpTime.CompareTo(Single.NaN) == 0 || LerpTime.CompareTo(Single.NegativeInfinity) == 0 || LerpTime.CompareTo(Single.PositiveInfinity) == 0)
                LerpTime = 1;

            Vector3 S = Vector3.Lerp(K0.Scale, K1.Scale, LerpTime);
            Vector3 T = Vector3.Lerp(K0.Pos, K1.Pos, LerpTime);
            Quaternion R = Quaternion.Slerp(K0.Quat, K1.Quat, LerpTime);

            return Matrix.Scaling(S) * Matrix.RotationQuaternion(R) * Matrix.Translation(T);
        }
    }

    class Sections
    {
        string m_Name;
        IndexedNormal m_model;
        Keyframe[] m_Keyframes;
        Sections m_Child;
        Sections m_Sibling;
        Matrix m_matLocal;

        public int FrameNum
        {
            get { return m_Keyframes.Length; }
        }
        public string Name
        {
            get { return m_Name; }
        }

        public Sections(string Name ,string FilePath,string TexturePath)
        {
            m_Name = Name.Trim().ToUpper();
            if (!m_Name.StartsWith("GLASS", StringComparison.CurrentCultureIgnoreCase))
                m_model = new IndexedNormal(m_Name,FilePath, TexturePath);
            else
                m_model = new Glass(m_Name,FilePath, TexturePath);
        }
        public Sections(string Name, string FilePath, Texture texture)
        {
            m_Name = Name;
            if (!m_Name.StartsWith("GLASS", StringComparison.CurrentCultureIgnoreCase))
                m_model = new IndexedNormal(m_Name, FilePath, texture);
            else
                m_model = new Glass(m_Name, FilePath, texture);
        }

        public void SetChild(Sections Child)
        {
            m_Child = Child;
        }
        public void SetSibling(Sections Sibling)
        {
            m_Sibling = Sibling;
        }
        public void SetKeyframes(Keyframe[] Keyframes)
        {
            m_Keyframes = Keyframes;
        }

        public Keyframe GetKeyFrameByName(string Name)
        {
            Name = Name.Trim().ToUpper();
            for (int i = 0; i < m_Keyframes.Length; ++i)
                if (m_Keyframes[i].Name.Trim().ToUpper() == Name)
                    return m_Keyframes[i];

            return new Keyframe(null, -1, Vector3.Empty, Vector3.Empty, Quaternion.Zero);
        }
        public int GetKeyframeIndex(string Name)
        {
            for (int i = 0; i < m_Keyframes.Length; ++i)
            {
                if (Name == m_Keyframes[i].Name)
                    return i;
            }
            return -1;
        }

        public void SetLocalMatrix(string K0,string K1,float Time)
        {
            Keyframe key0 = GetKeyFrameByName(K0);
            Keyframe key1 = GetKeyFrameByName(K1);
            m_matLocal = Keyframe.LerpKeyframe(key0, key1, Time);
        }
        public void SetLocalMatrix(int K0, int K1, float Time)
        {
            Keyframe key0 = m_Keyframes[K0];
            Keyframe key1 = m_Keyframes[K1];
            m_matLocal = Keyframe.LerpKeyframe(key0, key1, Time);
        }
        public void SetLocalMatrix(int K)
        {
            Vector3 S = m_Keyframes[K].Scale;
            Vector3 T = m_Keyframes[K].Pos;
            Quaternion R = m_Keyframes[K].Quat;

            m_matLocal = Matrix.Scaling(S) * Matrix.RotationQuaternion(R) * Matrix.Translation(T);
        }
        public void SetLocalMatrix(Matrix MatWorld)
        {
            m_matLocal = MatWorld;
        }

        public void Render(Matrix matParent)
        {
            m_model.Render(m_matLocal * matParent);
            if (m_Child != null)
                m_Child.Render(m_matLocal * matParent);
            if (m_Sibling != null)
                m_Sibling.Render(matParent);
        }
    }

    class Airplane
    {
        Sections[] m_Airplane;
        Sections m_Parent;
        int m_SectionNum;
        bool flap = false;

        public Airplane(Device Graphic ,string FilePath)
        {
            int SectionsNum;
            string ParentName;
            string TexturePath;
            string[] Names;
            string[] FilePathes;
            string[] KeyframePathes;

            string[] SectionName;
            string[] ChildName;
            string[] SiblingName;

            ///////////////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////
            StreamReader SR = new StreamReader(FilePath);
            string line = "";


            ReadLine(ref SR, ref line);
            SectionsNum = Convert.ToInt32(line); // Number of Sections
            ReadLine(ref SR, ref line);
            ParentName = line.Trim().ToUpper(); // Parent Name
            ReadLine(ref SR, ref line);
            TexturePath = line;                 // Texture Path


            Names = new string[SectionsNum];    // Sections Path
            FilePathes = new string[SectionsNum];
            for (int i = 0; i < SectionsNum; ++i)
            {
                ReadLine(ref SR, ref line);
                Names[i] = line.Trim().ToUpper();
                ReadLine(ref SR, ref line);
                FilePathes[i] = line;
            }


            KeyframePathes = new string[SectionsNum];// Keyframes path
            for (int i = 0; i < SectionsNum; ++i)
            {
                ReadLine(ref SR, ref line);
                KeyframePathes[i] = line;
            }

            // read hierarchy tree
            string[] tree;
            SectionName = new string[SectionsNum];
            ChildName = new string[SectionsNum];
            SiblingName = new string[SectionsNum];
            for (int i = 0; i < SectionsNum; ++i)
            {
                ReadLine(ref SR, ref line);
                tree = line.Split(',');
                SectionName[i] = tree[0].Trim().ToUpper();
                ChildName[i] = tree[1].Trim().ToUpper();
                SiblingName[i] = tree[2].Trim().ToUpper();
            }
            SR.Close();
            ///////////////////////////////////////////////////////
            ///////////////////////////////////////////////////////

            m_SectionNum = SectionsNum;

            // initilalize sections
            Texture text = TextureLoader.FromFile(Graphic,TexturePath);
            m_Airplane = new Sections[m_SectionNum];
            for (int i = 0; i < m_SectionNum; ++i)
            {
                m_Airplane[i] = new Sections(Names[i], FilePathes[i], text);
                m_Airplane[i].SetKeyframes(LoadKeyframes(KeyframePathes[i]));
            }

            // initialize hierarchy tree
            for (int i = 0; i < m_SectionNum; ++i)
            {
                int index = GetSectionIndex(Names[i]);
                if (ChildName[i] != "NULL")
                {
                    int secnum = GetSectionIndex(ChildName[i]);
                    m_Airplane[index].SetChild(m_Airplane[secnum]);
                }
                if (SiblingName[i] != "NULL")
                {
                    int secnum = GetSectionIndex(SiblingName[i]);
                    m_Airplane[index].SetSibling(m_Airplane[secnum]);
                }
            }

            // Parent
            m_Parent = GetSectionByName(ParentName);
        }

        private Keyframe[] LoadKeyframes(string Path)
        {
            StreamReader SR = new StreamReader(Path);
            string line = "";

            int KeyframesNum;
            Keyframe[] Frames;

            ReadLine(ref SR, ref line);
            KeyframesNum = Convert.ToInt32(line);   // Number of Frames
            Frames = new Keyframe[KeyframesNum];

            string[] spliter;
            float x, y, z, w;
            for (int i = 0; i < KeyframesNum; ++i)
            {
                ReadLine(ref SR, ref line);
                Frames[i].Name = line;          // Name

                ReadLine(ref SR, ref line);     // Time
                Frames[i].Time = Convert.ToSingle(line);

                ReadLine(ref SR, ref line);     // Pos
                spliter = line.Split(',');
                x = Convert.ToSingle(spliter[0]);
                y = Convert.ToSingle(spliter[1]);
                z = Convert.ToSingle(spliter[2]);
                Frames[i].Pos = new Vector3(x, y, z);

                ReadLine(ref SR,ref line);      // scale
                spliter = line.Split(',');
                x = Convert.ToSingle(spliter[0]);
                y = Convert.ToSingle(spliter[1]);
                z = Convert.ToSingle(spliter[2]);
                Frames[i].Scale = new Vector3(x, y, z);

                ReadLine(ref SR, ref line);      // quaternion
                spliter = line.Split(',');
                x = Convert.ToSingle(spliter[0]);
                y = Convert.ToSingle(spliter[1]);
                z = Convert.ToSingle(spliter[2]);
                w = Convert.ToSingle(spliter[3]);
                Vector3 axis = new Vector3(x, y, z);
                w = Geometry.DegreeToRadian(w);
                Frames[i].Quat = Quaternion.RotationAxis(axis, w);
                Frames[i].Quat.Normalize();
            }
            SR.Close();
            return Frames;
        }

        void ReadLine(ref StreamReader SR, ref string line)
        {
            do
            {
                line = SR.ReadLine();
            }
            while (line == "" || line.StartsWith("#"));
        }

        public Sections GetSectionByName(string Name)
        {
            Name = Name.Trim().ToUpper();
            for (int i = 0; i < m_SectionNum; ++i)
            {
                if (Name == m_Airplane[i].Name)
                    return m_Airplane[i];
            }
            return null;
        }
        public int GetSectionIndex(string Name)
        {
            for (int i = 0; i < m_SectionNum; ++i)
            {
                if (Name == m_Airplane[i].Name)
                    return i;
            }

            return -1;
        }

        public void SetKey0()
        {
            for (int i = 0; i < m_SectionNum; ++i)
                m_Airplane[i].SetLocalMatrix(0);
        }

        float rudderTime = 0;
        public virtual void Rudderleft(float dt)
        {
            Sections s = GetSectionByName("Rudder");
            if (rudderTime > 0)
            {
                s.SetLocalMatrix(0, 1, rudderTime);
            }
            else
            {
                s.SetLocalMatrix(0, 2, -rudderTime);
            }
            rudderTime += dt;
            if (rudderTime > s.GetKeyFrameByName("left").Time)
                rudderTime = s.GetKeyFrameByName("left").Time;
        }
        public virtual void RudderRight(float dt)
        {
            Sections s = GetSectionByName("Rudder");
            if (rudderTime <= 0)
            {
                s.SetLocalMatrix(0, 2, -rudderTime);
            }
            else
            {
                s.SetLocalMatrix(0, 1, rudderTime);
            }
            rudderTime -= dt;
            if (rudderTime < -s.GetKeyFrameByName("right").Time)
                rudderTime = -s.GetKeyFrameByName("right").Time;
        }
        public virtual void ZeroRudder(float dt)
        {
            if (rudderTime == 0)
                GetSectionByName("Rudder").SetLocalMatrix(0);
            else
            {
                Sections s = GetSectionByName("Rudder");
                if (rudderTime > 0)
                {
                    s.SetLocalMatrix(0, 1, rudderTime);
                    rudderTime -= dt;
                    if (rudderTime < 0)
                        rudderTime = 0;
                }
                else
                {
                    s.SetLocalMatrix(0, 2, -rudderTime);
                    rudderTime += dt;
                    if (rudderTime > 0)
                        rudderTime = 0;
                }
            }
        }

        float flaptime = 0;
        public virtual void FlapDown(float dt)
        {
            Sections s = GetSectionByName("Flap");
            s.SetLocalMatrix("Rest", "Down", flaptime);
            flaptime += dt;
            if (flaptime > s.GetKeyFrameByName("Down").Time)
                flaptime = s.GetKeyFrameByName("Down").Time;
        }
        public virtual void FlapUp(float dt)
        {
            Sections s = GetSectionByName("Flap");
            s.SetLocalMatrix("Down", "Rest", flaptime);
            flaptime -= dt;
            if (flaptime < 0.0)
                flaptime = 0;
        }

        float elevTime = 0;
        public virtual void ElevatorUp(float dt)
        {
            Sections sl = GetSectionByName("elevleft");
            Sections sr = GetSectionByName("elevRight");
            if (elevTime >= 0)
            {
                sl.SetLocalMatrix(0, 1, elevTime);
                sr.SetLocalMatrix(0, 1, elevTime);
                elevTime += dt;
                if (elevTime > sl.GetKeyFrameByName("Up").Time)
                    elevTime = sl.GetKeyFrameByName("Up").Time;
            }
            else
            {
                sl.SetLocalMatrix(0, 2, -elevTime);
                sr.SetLocalMatrix(0, 2, -elevTime);
                elevTime += dt;
            }

        }
        public virtual void ElevatorDown(float dt)
        {
            Sections sl = GetSectionByName("elevleft");
            Sections sr = GetSectionByName("elevright");
            if (elevTime <= 0)
            {
                sl.SetLocalMatrix(0, 2, -elevTime);
                sr.SetLocalMatrix(0, 2, -elevTime);
                elevTime -= dt;
                if (elevTime < -sl.GetKeyFrameByName("Down").Time)
                    elevTime = -sl.GetKeyFrameByName("Down").Time;
            }
            else
            {
                sl.SetLocalMatrix(0, 1, elevTime);
                sr.SetLocalMatrix(0, 1, elevTime);
                elevTime -= dt;
            }
        }
        public virtual void ZeroElavator(float dt)
        {
            Sections sl = GetSectionByName("elevleft");
            Sections sr = GetSectionByName("elevright");
            if (elevTime <= 0)
            {
                sl.SetLocalMatrix(0, 2, -elevTime);
                sr.SetLocalMatrix(0, 2, -elevTime);
                elevTime += dt;
                if (elevTime > 0)
                    elevTime = 0;
            }
            if (elevTime > 0)
            {
                sl.SetLocalMatrix(0, 1, elevTime);
                sr.SetLocalMatrix(0, 1, elevTime);
                elevTime -= dt;
                if (elevTime < 0)
                    elevTime = 0;
            }
            if (elevTime == 0)
            {
                sl.SetLocalMatrix(0);
                sr.SetLocalMatrix(0);
            }
        }

        float alironTime = 0;
        public virtual void AlironLeft(float dt)
        {
            Sections sl = GetSectionByName("alironLeft");
            Sections sr = GetSectionByName("alironRight");

            if (alironTime >= 0)
            {
                sl.SetLocalMatrix(0, 1, alironTime);
                sr.SetLocalMatrix(0, 1, alironTime);
                alironTime += dt;
                if (alironTime > sl.GetKeyFrameByName("Up").Time)
                    alironTime = sl.GetKeyFrameByName("Up").Time;
            }
            else
            {
                sl.SetLocalMatrix(0, 2, -alironTime);
                sr.SetLocalMatrix(0, 2, -alironTime);
                alironTime += dt;
            }
        }
        public virtual void AlironRight(float dt)
        {
            Sections sl = GetSectionByName("alironLeft");
            Sections sr = GetSectionByName("alironRight");

            if (alironTime <= 0)
            {
                sl.SetLocalMatrix(0, 2, -alironTime);
                sr.SetLocalMatrix(0, 2, -alironTime);
                alironTime -= dt;
                if (alironTime < -sl.GetKeyFrameByName("down").Time)
                    alironTime = -sl.GetKeyFrameByName("down").Time;
            }
            else
            {
                sl.SetLocalMatrix(0, 1, alironTime);
                sr.SetLocalMatrix(0, 1, alironTime);
                alironTime -= dt;
            }
        }
        public virtual void ZeroAliron(float dt)
        {
            Sections sl = GetSectionByName("alironleft");
            Sections sr = GetSectionByName("alironright");
            if (alironTime > 0)
            {
                sl.SetLocalMatrix(0, 1, alironTime);
                sr.SetLocalMatrix(0, 1, alironTime);
                alironTime -= dt;
                if (alironTime < 0)
                    alironTime = 0;
            }
            if (alironTime < 0)
            {
                sl.SetLocalMatrix(0, 2, -alironTime);
                sr.SetLocalMatrix(0, 2, -alironTime);
                alironTime += dt;
                if (alironTime > 0)
                    alironTime = 0;
            }
            if (alironTime == 0)
            {
                sr.SetLocalMatrix(0);
                sl.SetLocalMatrix(0);
            }
        }

        public void Render()
        {
            m_Parent.Render(Matrix.Identity);
        }
        public void Render(Matrix matWorld)
        {
            m_Parent.Render(matWorld);
        }
    }
}
