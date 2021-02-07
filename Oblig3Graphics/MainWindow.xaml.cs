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

            SetupComboBox();

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

        private void SetupComboBox()
        {
            solarSystem.FindAll(s => s is Planet).ForEach(s => planetComboBox.Items.Add(s));
            planetComboBox.Items.Add("Stop planet view");
            planetComboBox.SelectionChanged += PlanetComboBox_SelectionChanged;
        }

        private void PlanetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            planetInfoCanvas.Children.Clear();
            if (planetComboBox.SelectedItem is Planet)
            {
                Planet selectedPlanet = (Planet)(planetComboBox.SelectedItem);
                planetInfoCanvas.Children.Add(selectedPlanet.infoShape);
                planetInfoCanvas.Children.Add(selectedPlanet.infoText);
                selectedPlanet.Moons.ForEach(m => planetInfoCanvas.Children.Add(m.infoShape));
                planetInfoCanvas.Visibility = Visibility.Visible;
                selectedPlanet.CalcPosInfo(0);
            } else
            {
                planetInfoCanvas.Visibility = Visibility.Hidden;
            }
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
            s.ObjectRadius = 695000; //radius i km - bruker polardiameter som utgangspunkt for alle
            s.Color = Colors.Yellow;
            solarSystem.Add(s);
            s.shape = new Ellipse();
            s.shape.Fill = new SolidColorBrush(s.Color);
            int shapeRadius = SpaceScalingHelper.ObjectRadiusScaling(myWindow.Width, s.ObjectRadius);
            s.shape.Height = shapeRadius;
            s.shape.Width = shapeRadius;
            myCanvas.Children.Add(s.shape);
            s.CalcPos(0);

            p = new Planet("Mercury", 57910, 88);
            p.ObjectRadius = 2440;
            p.Color = Colors.MediumVioletRed;
            solarSystem.Add(p);

            p = new Planet("Venus", 108200, 225);
            p.ObjectRadius = 6052;
            p.Color = Colors.Orange;
            solarSystem.Add(p);

            p = new Planet("Earth", 149600, 365);
            p.ObjectRadius = 6357;
            p.Color = Colors.Blue;
            solarSystem.Add(p);

            p = new Planet("Mars", 227940, 687);
            p.ObjectRadius = 3378;
            p.Color = Colors.Red;
            solarSystem.Add(p);

            p = new Planet("Jupiter", 778330, 4333);
            p.ObjectRadius = 66855;
            p.Color = Colors.Gray;
            solarSystem.Add(p);

            p = new Planet("Saturn", 1429400, 10760);
            p.ObjectRadius = 54364;
            p.Color = Colors.LightSlateGray;
            solarSystem.Add(p);

            p = new Planet("Uranus", 2879889, 39685);
            p.ObjectRadius = 24973;
            p.Color = Colors.Cyan;
            solarSystem.Add(p);

            p = new Planet("Neptune", 4504300, 60190);
            p.ObjectRadius = 24341;
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
                    //p.shape.Name = p.Name;
                    p.shape.Fill = new SolidColorBrush(p.Color);
                    int shapeRadius = SpaceScalingHelper.ObjectRadiusScaling(myWindow.Width, p.ObjectRadius);
                    p.shape.Height = shapeRadius;
                    p.shape.Width = shapeRadius;
                    p.shape.MouseDown += Planet_MouseDown;
                    myCanvas.Children.Add(p.shape);
                    //lagar tekstboks for planetnamnet
                    p.shapeText = new TextBlock();
                    p.shapeText.Background = Brushes.AntiqueWhite;
                    p.shapeText.TextAlignment = TextAlignment.Center;
                    p.shapeText.Inlines.Add(new Bold(new Run(p.Name)));
                    p.shapeText.MouseDown += Planet_MouseDown;
                    myCanvas.Children.Add(p.shapeText);
                    p.CalcPos(0);

                    //detaljert info:
                    p.infoShape = new Ellipse();
                    p.infoShape.Fill = new SolidColorBrush(p.Color);
                    //shapeRadius = SpaceScalingHelper.ObjectRadiusScaling(myWindow.Width, p.ObjectRadius); TODO ny metode her!!
                    p.infoShape.Height = 100;
                    p.infoShape.Width = 100;
                    //lagar tekstboks for planetnamnet
                    p.infoText = new TextBlock();
                    p.infoText.Background = Brushes.AntiqueWhite;
                    //p.shapeText.TextAlignment = TextAlignment.Center;
                    p.infoText.Inlines.Add(new Run(p.Info()));
                }
            }
        }

        private void Planet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
