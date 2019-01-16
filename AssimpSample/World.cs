// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;

using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
     

        #region Atributi



        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance =20.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        /// <summary>
        ///	 Scaling factor for model
        /// </summary>
        private float mm_scale = 1.0f;

        /// <summary>
        ///	 X position for model
        /// </summary>
        private float mm_positionX = 0f;

        /// <summary>
        ///	 X position for model
        /// </summary>
        private float m_positionX = 0f;

        /// <summary>
        ///	 Ambient color for Light1
        /// </summary>
        private float[] m_ambientColor = { 0.2f, 0.2f, 0.2f, 0.2f };

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum Textures { Desk = 0, Carpet, Disc};
        private readonly int m_textureCount = Enum.GetNames(typeof(Textures)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Fajlovi tekstura
        /// </summary>
        private string[] m_textureFiles = {
                                        "..//..//textures//desk1.jpg",
                                        "..//..//textures//carpet.jpg",
                                        "..//..//textures//disc.jpg",

        };


        private bool m_animationStarted = false;

        private DispatcherTimer timer;

        // variables for counting position changes
        private int xDirection;
        private int yDirection;

        private float rotateDisc = 0f;

        private bool phase1 = false;
        private bool phase2 = false;

        private double[] discPosition = { 0f, 0f, 0f };
        private double[] readerPosition = { 0f, 0f, 0f };

        private double[] discAndReaderPosition = { 0f, 0f, 0f };
        
        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        ///	 Scaling factor for model
        /// </summary>
        public float MMScale
        {
            get { return mm_scale; }
            set { mm_scale = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public float MMPositionX
        {
            get { return mm_positionX; }
            set { mm_positionX = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public float[] MAmbientColor
        {
            get { return m_ambientColor; }
            set { m_ambientColor = value; }
        }



        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public float MPositionX
        {
            get { return m_positionX; }
            set { m_positionX = value; }
        }

        /// <summary>
        ///	 Boolean inticator if the animation has started
        /// </summary>
        public bool m_AnimationStarted
        {
            get { return m_animationStarted; }
            set { m_animationStarted = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;

            // inicijalizacija niza 
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);

            SetLights(gl);
            SetTextures(gl);


            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // podesi boju za brisanje ekrana na crnu



            m_scene.LoadScene();
            m_scene.Initialize();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        public void SetLights(OpenGL gl)
        {
            //// point lighting 
            float[] light0pos = new float[] { 0.0f, 20.0f, 10.0f, 0.0f };
            float[] light0ambient = new float[] { 0.3f, 0.3f, 0.3f, 0.3f };
            float[] light0diffuse = new float[] { 0.8f, 0.8f, 0.8f, 0.9f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);

            // for point lighting, cutoff is 180
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);


            float[] light1diffuse = new float[] { 0.7f, 0.0f, 0.0f, 0.5f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);




            //color material
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);


            gl.Enable(OpenGL.GL_NORMALIZE);

            gl.ShadeModel(OpenGL.GL_SMOOTH);

        }

        public void SetTextures(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);


            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0); //ova metoda pravi mipmape, ne smao jednu teksturu
                //gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGB8, image.Width, image.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                image.UnlockBits(imageData);
                image.Dispose();
            }

        }
        

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // drawing objects which can be manipuleted by keyboard events

            gl.PushMatrix();
            {
                

                // setting viewport to take full screen
                gl.Viewport(0, 0, m_width, m_height);
                gl.LookAt(50.6f, 14.8f, -60f, -15f, -15f, 0f, 0f, 1f, 0f);


                // transformations based on keyboard events will apply to all of the objects on the scene
                gl.Translate(m_positionX, 1.0f, -m_sceneDistance);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
                gl.Scale(0.1f, 0.1f, 0.1f);

                // moving together computer, disc and disc reader
                gl.PushMatrix();

                gl.Scale(mm_scale, mm_scale, mm_scale);
                gl.Translate(mm_positionX, 0f, 0f);


                // computer
                gl.PushMatrix();
                    DrawMeshModel(gl);
                    gl.PopMatrix();
                
                    gl.PushMatrix();

                        // align inside computer
                        gl.Translate(-44f, 0f, 0f);
                        // translating for animation together the disc and the reader
                        gl.Translate(discAndReaderPosition[0], discAndReaderPosition[1], discAndReaderPosition[2]);
                    
                        // disc
                        gl.PushMatrix();
                        DrawDisc(gl);
                        gl.PopMatrix();

                        // disc reader
                        gl.PushMatrix();
                        DrawReader(gl);
                        gl.PopMatrix();

                    gl.PopMatrix();
                gl.PopMatrix();

                // table
                gl.PushMatrix();
                DrawTable(gl);
                gl.PopMatrix();

                // carpet
                gl.PushMatrix();
                DrawCarpet(gl);
                gl.PopMatrix();

                


            }
            gl.PopMatrix();

            



            // placing text by redefining viewport
            if (m_width < 400)
            {
                gl.Viewport(0, 0, m_width / 2, m_height / 2);
             }
            else if (m_width >= 400 && m_width < 700) 
            {
                gl.Viewport((int)(m_width / 1.4), 0, m_width / 2, m_height / 2);

            }
            else if (m_width >= 700 && m_width < 1000)
            {
                gl.Viewport((int)(m_width / 1.26), 0, m_width / 2, m_height / 2);

            }
            else if (m_width >= 1000 && m_width < 1400) 
            {
                gl.Viewport((int)(m_width / 1.16), 0, m_width / 2, m_height / 2);

            } else
            {
                gl.Viewport((int)(m_width / 1.11), 0, m_width / 2, m_height/2);
            }

            // right bottom corner text
            gl.PushMatrix();
            {
                DrawTextRightBottomCorner(gl);
            }
            gl.PopMatrix();

            
            // reset the viewport
            gl.Viewport(0, 0, m_width, m_height);
           
            gl.Flush();
        }

       



        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        private void DrawMeshModel(OpenGL gl)
        {
            gl.Translate(30f, 185.0f, 70.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.0f });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] {5.0f, 50.0f, 0f, 1.0f });

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, m_ambientColor);
            m_scene.Draw();
            

        }

       

        private void DrawDisc(OpenGL gl)
        {
            
            gl.Scale(10.3f, 10.3f, 10.3f);
            gl.Translate(5.25f, -0.4f, 1.9f);
            gl.Translate(discPosition[0], discPosition[1], discPosition[2]);
            gl.Rotate(270f, 0f, 0f);
            gl.Color(0.6f, 0.6f, 0.6f);

            gl.Rotate(0f, 0f, rotateDisc);



            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)Textures.Disc]);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(1.1f, 1.1f, 1.1f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            Disk disk = new Disk();
            disk.TextureCoords = true;
            disk.Loops = 120;
            disk.Slices = 40;
            disk.InnerRadius = 0.4f;
            disk.OuterRadius = 1.9f;
            disk.CreateInContext(gl);
            disk.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            // Oznaci kraj iscrtavanja
        }


        private void DrawReader(OpenGL gl)
        {
            gl.Scale(21.7f, 0.5f, 21.7f);
            gl.Translate(2.5f, -10.5f, 0.9f);
            gl.Translate(readerPosition[0], readerPosition[1], readerPosition[2]);
            gl.Rotate(90f, 0f, 0f);
            
            gl.Color(0.2f, 0.1f, 0.1f);
            Cube reader = new Cube();
            reader.Render(gl, RenderMode.Render);

        }


        private void DrawTable(OpenGL gl)
        {
            // gornja ploca
            gl.PushMatrix();
                gl.Scale(220.3f, 5.3f, 100.3f);
                gl.Translate(0, -2.7f, 0.0f);
                gl.Rotate(90f, 0f, 0f);
                gl.Color(1f, 1f, 1f);

                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)Textures.Desk]);
                Cube ploca = new Cube();
                ploca.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

                gl.MatrixMode(OpenGL.GL_TEXTURE);
                gl.LoadIdentity();
                gl.Scale(4.1f, 4.1f, 4.1f);
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();


            gl.Color(0.8f, 0.8f, 0.8f);

            // nogica -x +z
            gl.PushMatrix();
                gl.Scale(3.54f, 67.3f, 3.6f);
                gl.Translate(-60.0f, -1.2f, 25.3f);
                gl.Rotate(90f, 0f, 0f);
            
                Cube nogica1 = new Cube();
                nogica1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // nogica -x -z
            gl.PushMatrix();
                gl.Scale(3.54f, 67.3f, 3.6f);
                gl.Translate(-60.0f, -1.2f, -25.3f);
                gl.Rotate(90f, 0f, 0f);
                Cube nogica2 = new Cube();
                nogica2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            // nogica +x +z
            gl.PushMatrix();
                gl.Scale(3.54f, 67.3f, 3.6f);
                gl.Translate(60.0f, -1.2f, 25.3);
                gl.Rotate(90f, 0f, 0f);
                Cube nogica3 = new Cube();
                nogica3.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            // nogica +x -z
                gl.PushMatrix();
                gl.Scale(3.54f, 67.3f, 3.6f);
                gl.Translate(60.0f, -1.2f, -25.3f);
                gl.Rotate(90f, 0f, 0f);
                Cube nogica4 = new Cube();
                nogica4.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        private void DrawCarpet(OpenGL gl)
        {
            float[] carpetVertex =
            {                       
                -330.0f, -150.0f,  170.0f,
                 330.0f, -150.0f,  170.0f,
                 330.0f, -150.0f, -170.0f,
                -330.0f, -150.0f, -170.0f
            };

            float[] textCoords =
            {
                1.0f, 0.0f,
                1.0f, 1.0f,
                0.0f, 1.0f,
                0.0f, 0.0f
            };


            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)Textures.Carpet]);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.Scale(2.0f, 3.4f, 3.2f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);


            gl.Color(0.0f, 0.0f, 0.1f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0, 1, 0);
            int i = 0;
            int j = 0;
            while (i < carpetVertex.Length)
            {
                gl.TexCoord(textCoords[j], textCoords[j + 1]);
                gl.Vertex(carpetVertex[i], carpetVertex[i+1], carpetVertex[i+2]);

                i += 3;
                j += 2;
            }
            
            gl.End();
            
        }

        public void StartAnimation()
        {
            if (!m_animationStarted)
            {
                m_animationStarted = true;

                xDirection = 0;
                yDirection = 0;
                phase1 = true;

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(35);
                timer.Tick += new EventHandler(UpdateAnimation);
                timer.Start();
             }
            
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            if (phase1)
            {
                if (xDirection < 29)
                {
                    discAndReaderPosition[0] += 1.5f;

                    if (xDirection < 19)
                    {
                        rotateDisc += 25f;
                    }
                    else if (xDirection >= 19 && xDirection < 25)
                    {
                        rotateDisc += 10f;
                    }
                    else
                    {
                        rotateDisc += 3f;
                    }

                    xDirection++;
                }
                else
                {
                    if (yDirection < 10)
                    {
                        rotateDisc = 0f;
                        discPosition[1] += 0.1f;
                        yDirection++;
                    }
                    else
                    {
                        if (xDirection < 41)
                        {
                            discPosition[0] += 0.4f;
                            xDirection++;
                        }
                        else
                        {
                            phase1 = false;
                            phase2 = true;

                        }
                    }
                }
            }

            if (phase2)
            {
                if (yDirection > 0)
                {
                    discPosition[1] -= 0.1f;
                    yDirection--;
                }
                else
                {
                    if (xDirection > 20)
                    {
                        readerPosition[0] -= 0.1f;
                        xDirection--;
                    } else
                    {
                        timer.Stop();
                        phase2 = false;
                        phase1 = false;
                        discPosition[0] = 0f;
                        discPosition[1] = 0f;
                        discPosition[2] = 0f;
                        readerPosition[0] = 0f;
                        readerPosition[1] = 0f;
                        readerPosition[2] = 0f;
                        discAndReaderPosition[0] = 0;
                        m_animationStarted = false;

                    }
                }
            }


            
            

        }

        private void DrawTextRightBottomCorner(OpenGL gl)
        {
            gl.DrawText(5, 125, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "Predmet: Racunarska grafika");
            gl.DrawText(5, 125, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "_______________________");
            gl.DrawText(5, 100, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "Sk.god: 2018/19.");
            gl.DrawText(5, 100, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "______________");
            gl.DrawText(5, 75, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "Ime: Milan");
            gl.DrawText(5, 75, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "________");
            gl.DrawText(5, 50, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "Prezime: Lazic");
            gl.DrawText(5, 50, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "___________");
            gl.DrawText(5, 25, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "Sifra zad: 6.2");
            gl.DrawText(5, 25, 1.0f, 1.0f, 0.0f, "Tahoma Italic", 10, "___________");

        }

      
        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
