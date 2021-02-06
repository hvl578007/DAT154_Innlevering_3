using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SpaceSim;

namespace Oblig3Graphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SpaceObject> solarSystem;
        private int time = 0;
        public event Action<int> moveIt;
        private DispatcherTimer t;
        public MainWindow()
        {
            InitializeComponent();

            SetupSolarSystem();
            SetupPlanetShapes();

            solarSystem.ForEach(s => moveIt += s.CalcPos);

            t = new DispatcherTimer();
            t.Interval = new TimeSpan(300000); //ca. 30 fps?
            t.Tick += T_Tick;
            t.Start();
            
        }

        private void T_Tick(object sender, EventArgs e)
        {
            moveIt(time++);
        }

        /// <summary>
        /// Set opp solsystemet, berre sola + planetane for nå
        /// </summary>
        private void SetupSolarSystem()
        {
            Star s;
            Planet p;
            Moon m;
            solarSystem = new();
            s = new Star("The Sun");
            //s.ObjectRadius = 695000; //radius i km TODO må fikse skalering! - bruker polardiameter som utgangspunkt for alle
            s.ObjectRadius = 50;
            s.Color = Colors.Yellow;
            solarSystem.Add(s);
            s.shape = new Ellipse();
            s.shape.Fill = new SolidColorBrush(s.Color);
            s.shape.Height = s.ObjectRadius * 2;
            s.shape.Width = s.ObjectRadius * 2;
            myCanvas.Children.Add(s.shape);
            s.CalcPos(0);

            p = new Planet("Mercury", 57910 / 1000, 88);
            p.ObjectRadius = 2440 / 1000;
            p.Color = Colors.MediumVioletRed;
            solarSystem.Add(p);

            p = new Planet("Venus", 108200 / 1000, 225);
            p.ObjectRadius = 6052 / 1000;
            p.Color = Colors.Orange;
            solarSystem.Add(p);

            p = new Planet("Earth", 149600 / 1000, 365);
            p.ObjectRadius = 6357 / 1000;
            p.Color = Colors.Blue;
            solarSystem.Add(p);

            p = new Planet("Mars", 227940 / 1000, 687);
            p.ObjectRadius = 3378 / 1000;
            p.Color = Colors.Red;
            solarSystem.Add(p);

            p = new Planet("Jupiter", 778330 / 2500, 4333);
            p.ObjectRadius = 66855 / 2000;
            p.Color = Colors.Gray;
            solarSystem.Add(p);

            p = new Planet("Saturn", 1429400 / 3000, 10760);
            p.ObjectRadius = 54364 / 2000;
            p.Color = Colors.LightSlateGray;
            solarSystem.Add(p);

            p = new Planet("Uranus", 2879889 / 5000, 39685);
            p.ObjectRadius = 24973 / 2000;
            p.Color = Colors.Cyan;
            solarSystem.Add(p);

            p = new Planet("Neptune", 4504300 / 5000, 60190);
            p.ObjectRadius = 24341 / 2000;
            p.Color = Colors.DarkBlue;
            solarSystem.Add(p);

            //p = new DwarfPlanet("Pluto", 5913520, 90550);
        }

        private void SetupPlanetShapes()
        {
            foreach (SpaceObject s in solarSystem)
            {
                if (s is Planet)
                {
                    Planet p = s as Planet;
                    p.shape = new Ellipse();
                    p.shape.Fill = new SolidColorBrush(p.Color);
                    p.shape.Height = p.ObjectRadius * 2;
                    p.shape.Width = p.ObjectRadius * 2;
                    myCanvas.Children.Add(p.shape);
                    p.CalcPos(0);
                }
            }
        }
    }
}
