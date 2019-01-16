using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
    
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        private int _computerSize = 100;

        public int ComputerSize
        {
            get { return _computerSize; }
            set
            {
                _computerSize = value;
                OnPropertyChanged("ComputerSize");
                float forScaling = (float) _computerSize / 100;

                m_world.MMScale = forScaling;

            }

        }

        private int _computerPositionX;

        public int ComputerPositionX
        {
            get { return _computerPositionX; }
            set
            {
                _computerPositionX = value;
                OnPropertyChanged("ComputerPositionX");

                m_world.MMPositionX = (float)_computerPositionX;

            }
        }

        private ObservableCollection<string> _ambientColors;

        public ObservableCollection<string> AmbientColors
        {
            get { return _ambientColors; }
            set
            {
                _ambientColors = value;
                OnPropertyChanged("AmbientColors");
            }
        }


        #endregion Atributi

            #region Konstruktori

        public MainWindow()
        {

            AmbientColors = new ObservableCollection<string>();


            AmbientColors.Add("Red");
            AmbientColors.Add("Orange");
            AmbientColors.Add("Yellow");
            AmbientColors.Add("Green");
            AmbientColors.Add("Blue");
            AmbientColors.Add("Purple");
            AmbientColors.Add("White");


            this.DataContext = this;



            // Inicijalizacija komponenti
            InitializeComponent();

            AmbientColors_combo.SelectedIndex = 6;



            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Computer"), "Laptop.3DS", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        public void Window_Loaded(object sender, RoutedEventArgs e) 
        {
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }


        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        //private void cb1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    int i = 0;
        //    float scale = 0.8f;

        //   switch(cb1.SelectedIndex)
        //    {
        //        case 0: scale = 0.7f; break;
        //        case 1: scale = 0.8f; break;
        //        case 2: scale = 0.9f; break;
        //        case 3: scale = 1.0f; break;
        //        case 4: scale = 1.1f; break;
        //        case 5: scale = 1.2f; break;
        //        case 6: scale = 1.3f; break;
        //        case 7: scale = 1.4f; break;
        //        case 8: scale = 1.5f; break;


        //    }

        //    if (m_world != null)
        //        m_world.MMScale = scale;
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (!m_world.m_AnimationStarted)
            {


                switch (e.Key)
                {
                    case Key.F4: this.Close(); break;

                    case Key.C:
                        {
                            m_world.StartAnimation();
                            DispatcherTimer timer = new DispatcherTimer();
                            timer.Interval = TimeSpan.FromMilliseconds(20);
                            timer.Tick += new EventHandler(disableUIControls);
                            timer.Start();
                            break;
                        }

                    case Key.W:
                        {

                            if (m_world.RotationX >= -30f)
                                m_world.RotationX -= 5.0f;
                            break;
                        }
                    case Key.S:
                        {
                            if (m_world.RotationX <= 30f)
                                m_world.RotationX += 5.0f;
                            break;
                        }
                
                    case Key.A: m_world.RotationY -= 5.0f; break;
                    case Key.D: m_world.RotationY += 5.0f; break;
                    case Key.OemOpenBrackets: m_world.SceneDistance -= 5.0f; break;
                    case Key.OemCloseBrackets: m_world.SceneDistance += 5.0f; break;
                    case Key.OemSemicolon:
                        {
                            ComputerSize -= 1;
                            break;
                        }
                    case Key.OemQuotes:
                        {
                            ComputerSize += 1;
                            break;
                        }
                    case Key.OemPeriod:
                        {
                            ComputerPositionX -= 1;
                            break;
                        }
                    case Key.OemQuestion:
                        {
                            ComputerPositionX += 1;
                            break;
                        }
                    case Key.U: 
                        {
                            m_world.MPositionX += 2;
                            break;
                        }


                    case Key.T:
                        {
                            m_world.MPositionX -= 2;
                            break;
                        }
                } 
                
            } else
            {
                switch(e.Key)
                {
                    case Key.F4: this.Close(); break;
                }
                
            } 

            
        }

        private void disableUIControls(object sender, EventArgs e)
        {
            if (m_world.m_AnimationStarted)
            {
                ComputerSizeTextBox.IsEnabled = false;
                ComputerPositionTextBox.IsEnabled = false;
                AmbientColors_combo.IsEnabled = false;
            } else
            {
                ComputerSizeTextBox.IsEnabled = true;
                ComputerPositionTextBox.IsEnabled = true;
                AmbientColors_combo.IsEnabled = true;
            }


        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        private void ComputerSize_KeyUp(object sender, KeyEventArgs e)
        {
            int n;
            bool number = Int32.TryParse(ComputerSizeTextBox.Text, out n);
            if (number)
            {
                if (ComputerSize != n)
                    ComputerSize = n;
            }

        }

        private void ComputerPosition_KeyUp(object sender, KeyEventArgs e)
        {
            int n;
            bool number = Int32.TryParse(ComputerPositionTextBox.Text, out n);
            if (number)
            {
                if (ComputerPositionX != n)
                    ComputerPositionX = n;
            }

        }

        private void AmbientColors_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_world != null)
            {


                switch(AmbientColors_combo.SelectedIndex)
                {
                    case 0: m_world.MAmbientColor = new float[] { 0.2f, 0f, 0f, 0.15f}; break;
                    case 1: m_world.MAmbientColor = new float[] { 0.2f, 0.1f, 0f, 0.15f }; break;
                    case 2: m_world.MAmbientColor = new float[] { 0.2f, 0.2f, 0f, 0.15f }; break;
                    case 3: m_world.MAmbientColor = new float[] { 0.0f, 0.2f, 0f, 0.15f }; break;
                    case 4: m_world.MAmbientColor = new float[] { 0.0f, 0.0f, 0.3f, 0.15f }; break;
                    case 5: m_world.MAmbientColor = new float[] { 0.2f, 0.0f, 0.2f, 0.15f }; break;
                    case 6: m_world.MAmbientColor = new float[] { 0.2f, 0.2f, 0.2f, 0.15f }; break;



                }

            }
        }
    }
}
