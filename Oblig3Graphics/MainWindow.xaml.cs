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
        public event Action<int> moveSolarSystem;
        public event Action<int> moveInfo;
        private DispatcherTimer t;
        private bool planetTextIsHidden = false, planetOrbitIsHidden = false;
        public MainWindow()
        {
            InitializeComponent();

            SetupSolarSystem();

            SetupComboBox();

            t = new DispatcherTimer();
            t.Interval = new TimeSpan(300000); ; //ca. 30 fps?
            t.Tick += T_Tick;
            t.Start();

            planetInfoCanvas.MouseRightButtonDown += PlanetInfoCanvas_MouseRightButtonDown;

            myWindow.KeyDown += MyWindow_KeyDown;

            planetTextButton.Click += PlanetTextButton_Click;

            planetOrbitButton.Click += PlanetOrbitButton_Click;
            
        }

        private void PlanetOrbitButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement ui in myCanvas.Children)
            {

                if (ui is Ellipse && (ui as Ellipse).Name.Equals("Orbit"))
                {
                    if (planetOrbitIsHidden)
                    {
                        ui.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ui.Visibility = Visibility.Hidden;
                    }
                }
            }
            foreach (UIElement ui in planetInfoCanvas.Children)
            {
                if (ui is Ellipse && (ui as Ellipse).Name.Equals("Orbit"))
                {
                    if (planetOrbitIsHidden)
                    {
                        ui.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ui.Visibility = Visibility.Hidden;
                    }
                }
            }


            planetOrbitIsHidden = !planetOrbitIsHidden;

            if (planetOrbitIsHidden)
            {
                planetOrbitButton.Content = "Show orbits";
            }
            else
            {
                planetOrbitButton.Content = "Hide orbits";
            }
        }

        private void PlanetTextButton_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (UIElement ui in myCanvas.Children)
            {
                
                if (ui is TextBlock)
                {
                    if (planetTextIsHidden)
                    {
                        ui.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ui.Visibility = Visibility.Hidden;
                    }
                }
            }
            foreach (UIElement ui in planetInfoCanvas.Children)
            {

                if (ui is TextBlock)
                {
                    if (planetTextIsHidden)
                    {
                        ui.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ui.Visibility = Visibility.Hidden;
                    }
                }
            }

            planetTextIsHidden = !planetTextIsHidden;

            if (planetTextIsHidden)
            {
                planetTextButton.Content = "Show textboxes";
            } else
            {
                planetTextButton.Content = "Hide textboxes";
            }
        }

        private void MyWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //endre det intervallet timeren oppdaterer seg på? eller det eg deler med i biblioteket...?
            //følte ikkje begge deler blei så veeeldig bra... interval er meir "fps" men går raskare også, + hopper om eg endrer verdien i cos/sin funksjonen...
            switch (e.Key)
            {
                case Key.Up:
                    //SpaceScalingHelper.IncreaseSimSpeed();
                    if (t.Interval.Ticks is not 0) t.Interval = t.Interval.Subtract(new TimeSpan(20000));
                    break;

                case Key.Down:
                    //SpaceScalingHelper.DecreaseSimSpeed();
                    t.Interval = t.Interval.Add(new TimeSpan(20000));
                    break;

                default:

                    break;
            }
        }

        private void PlanetInfoCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (planetInfoCanvas.Visibility == Visibility.Visible)
            {
                planetInfoCanvas.Children.Clear();
                moveInfo = null;
                planetInfoCanvas.Visibility = Visibility.Hidden;
                planetComboBox.SelectedIndex = planetComboBox.Items.Count - 1;
            }
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (moveInfo is not null) moveInfo(time);
            moveSolarSystem(time++);
        }

        private void SetupComboBox()
        {
            solarSystem.FindAll(s => s is Planet && s is not Moon).ForEach(s => planetComboBox.Items.Add(s));
            planetComboBox.Items.Add("Stop planet view");
            planetComboBox.SelectionChanged += PlanetComboBox_SelectionChanged;
        }

        private void PlanetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            planetInfoCanvas.Children.Clear();
            moveInfo = null;
            if (planetComboBox.SelectedItem is Planet)
            {
                Planet selectedPlanet = (Planet)(planetComboBox.SelectedItem);
                planetInfoCanvas.Children.Add(selectedPlanet.infoShape);
                planetInfoCanvas.Children.Add(selectedPlanet.infoText);
                selectedPlanet.Moons.ForEach(m => {
                    planetInfoCanvas.Children.Add(m.orbit);
                    planetInfoCanvas.Children.Add(m.infoShape);
                    planetInfoCanvas.Children.Add(m.infoText);
                    moveInfo += m.CalcPosInfo;
                });
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
            s.shapeText = new TextBlock();
            s.shapeText = new TextBlock();
            s.shapeText.Background = Brushes.AntiqueWhite;
            s.shapeText.TextAlignment = TextAlignment.Center;
            s.shapeText.Inlines.Add(new Bold(new Run(s.Name)));
            myCanvas.Children.Add(s.shape);
            myCanvas.Children.Add(s.shapeText);
            s.CalcPos(0);

            p = new Planet("Mercury", 57910, 88);
            p.ObjectRadius = 2440;
            p.Color = Colors.MediumVioletRed;
            solarSystem.Add(p);

            p = new Planet("Venus", 108200, 225);
            p.ObjectRadius = 6052;
            p.Color = Colors.Orange;
            solarSystem.Add(p);

            //Lagar jorda
            p = new Planet("Earth", 149600, 365);
            p.ObjectRadius = 6357;
            p.Color = Colors.Blue;
            solarSystem.Add(p);
            //lagar månen til jorda
            m = new Moon("The Moon", 384, 27);
            m.ObjectRadius = 1738;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            //mars
            p = new Planet("Mars", 227940, 687);
            p.ObjectRadius = 3378;
            p.Color = Colors.Red;
            solarSystem.Add(p);
            m = new Moon("Phobos", 9, 1);
            m.ObjectRadius = 11;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Deimos", 23, 2);
            m.ObjectRadius = 6;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            //jupiter, tar berre 4 månar?
            p = new Planet("Jupiter", 778330, 4333);
            p.ObjectRadius = 66855;
            p.Color = Colors.Gray;
            solarSystem.Add(p);
            m = new Moon("Io", 422, 2);
            m.ObjectRadius = 1821;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Europa", 671, 4);
            m.ObjectRadius = 1561;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Ganymede", 1070, 7);
            m.ObjectRadius = 2632;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Callisto", 1883, 17);
            m.ObjectRadius = 2410;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            //saturn + 3 første månar
            p = new Planet("Saturn", 1429400, 10760);
            p.ObjectRadius = 54364;
            p.Color = Colors.LightSlateGray;
            solarSystem.Add(p);
            m = new Moon("Mimas", 186, 1);
            m.ObjectRadius = 196;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Enceladus", 238, 1);
            m.ObjectRadius = 249;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Tethys", 295, 2);
            m.ObjectRadius = 530;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            //uranus + 4 første månar
            p = new Planet("Uranus", 2879889, 39685);
            p.ObjectRadius = 24973;
            p.Color = Colors.Cyan;
            solarSystem.Add(p);
            m = new Moon("Ariel", 191, 3);
            m.ObjectRadius = 579;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Umbriel", 266, 4);
            m.ObjectRadius = 585;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Titania", 436, 9);
            m.ObjectRadius = 789;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Oberon", 583, 13);
            m.ObjectRadius = 762;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            //neptun + 3 første månar
            p = new Planet("Neptune", 4504300, 60190);
            p.ObjectRadius = 24341;
            p.Color = Colors.DarkBlue;
            solarSystem.Add(p);
            m = new Moon("Triton", 355, -6);
            m.ObjectRadius = 1350;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Nereid", 5513, 360);
            m.ObjectRadius = 170;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);
            m = new Moon("Naiad", 48, 1);
            m.ObjectRadius = 29;
            m.Color = Colors.Black; //todo farge!
            p.Moons.Add(m);
            solarSystem.Add(m);

            SetupPlanetShapes();
            SetupMoonInfoShapes();
            //p = new DwarfPlanet("Pluto", 5913520, 90550);

            solarSystem.FindAll(s => s is not Moon).ForEach(s => moveSolarSystem += s.CalcPos);
        }

        private void SetupMoonInfoShapes()
        {
            foreach(SpaceObject s in solarSystem)
            {
                if (s is Moon)
                {
                    Moon m = s as Moon;
                    m.infoShape = new Ellipse();
                    m.infoShape.Width = SpaceScalingHelper.ObjectRadiusInfoScaling(planetInfoCanvas.Width, m.ObjectRadius);
                    m.infoShape.Height = SpaceScalingHelper.ObjectRadiusInfoScaling(planetInfoCanvas.Width, m.ObjectRadius);
                    m.infoShape.Fill = new SolidColorBrush(m.Color);
                    m.infoText = new TextBlock();
                    m.infoText.Background = Brushes.AntiqueWhite;
                    m.infoText.TextAlignment = TextAlignment.Center;
                    m.infoText.Inlines.Add(new Bold(new Run(m.Name)));
                    m.orbit = new Ellipse();
                    m.orbit.Name = "Orbit";
                    m.orbit.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void SetupPlanetShapes()
        {
            foreach (SpaceObject s in solarSystem)
            {
                if (s is Planet && s is not Moon)
                {
                    Planet p = s as Planet;
                    p.orbit = new Ellipse();
                    p.orbit.Name = "Orbit"; //til bruk for å skjule dei
                    p.orbit.Stroke = new SolidColorBrush(Colors.Black);
                    myCanvas.Children.Add(p.orbit);

                    p.shape = new Ellipse();
                    p.shape.Name = p.Name;
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
                    //lagar orbit

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
            Planet p = null;
            if (sender is Ellipse)
            {
                Ellipse ell = (Ellipse)sender;
                p = (Planet)solarSystem.Find(s => s.Name.Equals(ell.Name)); 

            } else if (sender is TextBlock)
            {
                TextBlock tb = (TextBlock)sender;
                Bold b = (Bold)tb.Inlines.ElementAt(0);
                string content = ((Run)b.Inlines.ElementAt(0)).Text;
                p = (Planet)solarSystem.Find(s => s.Name.Equals(content));
            }
            if (p is not null)
            {
                planetInfoCanvas.Children.Clear();
                moveInfo = null;

                planetInfoCanvas.Children.Add(p.infoShape);
                planetInfoCanvas.Children.Add(p.infoText);
                p.Moons.ForEach(m => {
                    planetInfoCanvas.Children.Add(m.orbit);
                    planetInfoCanvas.Children.Add(m.infoShape);
                    planetInfoCanvas.Children.Add(m.infoText);
                    moveInfo += m.CalcPosInfo;
                });
                planetInfoCanvas.Visibility = Visibility.Visible;
                p.CalcPosInfo(0);
            }
        }
    }
}
