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
        private float m_sceneDistance =800.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;
        

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

                // transformations based on keyboard events will apply to all of the objects on the scene
                gl.Translate(0.0f, 15.0f, -m_sceneDistance);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

                // computer
                gl.PushMatrix();
                DrawMeshModel(gl);
                gl.PopMatrix();

                // disc
                gl.PushMatrix();
                DrawDisc(gl);
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
            
            gl.Translate(30.0f, 205.0f, 70.0f);
            gl.Scale(1.1f, 1.1f, 1.1f);
            m_scene.Draw();
        }

        private void DrawDisc(OpenGL gl)
        {
            
            gl.Scale(12.3f, 12.3f, 12.3f);
            gl.Translate(5.55f, -0.7f, 0.5f);
            gl.Rotate(270f, 0f, 0f);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.Color(0.34f, 0.6f, 0.91f);
            
            Disk disk = new Disk();
            disk.Loops = 120;
            disk.Slices = 40;
            disk.InnerRadius = 0.4f;
            disk.OuterRadius = 2f;
            disk.CreateInContext(gl);
            disk.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.Disable(OpenGL.GL_COLOR_MATERIAL);
            // Oznaci kraj iscrtavanja
        }

        private void DrawTable(OpenGL gl)
        {
            // gornja ploca
            gl.PushMatrix();
                gl.Scale(220.3f, 5.3f, 100.3f);
                gl.Translate(0, -2.7f, 0.0f);
                gl.Rotate(90f, 0f, 0f);
                gl.Color(1f, 1f, 1f);
                Cube ploca = new Cube();
                ploca.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            gl.Color(0.4f, 0.4f, 0.4f);

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
                -330.0f, -150.0f, -170.0f,
            };

            gl.Color(0.6f, 0.1f, 0.3f);
            gl.Begin(OpenGL.GL_QUADS);
            int i = 0;
            while (i < carpetVertex.Length)
            {
                gl.Vertex(carpetVertex[i], carpetVertex[i+1], carpetVertex[i+2]);

                i += 3;
            }
            
            gl.End();
            
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
