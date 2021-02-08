using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SpaceSim
{
    public class SpaceObject
    {
        public String Name { get; set; }
        public int XPos { get; set; } //idk putte ein annan plass?
        public int YPos { get; set; }

        public SpaceObject(String name)
        {
            Name = name;
        }

        public virtual void CalcPos(int time)
        {
            //gjer ingenting?
           
        }

        public virtual void Draw()
        {
            Console.WriteLine(Name + ". X: " + XPos.ToString() + ", Y: " + YPos.ToString() + ".");
        }

        public virtual String Info()
        {
            return "Name: " + Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Star : SpaceObject
    {
        public int ObjectRadius { get; set; }
        public int RotationalPeriod { get; set; }
        public Color Color { get; set; }

        public Ellipse shape { get; set; }

        public Star(String name) : base(name)
        {
            XPos = 0; //todo?
            YPos = 0;
        }

        public override void CalcPos(int time)
        {
            //står i ro...
            Canvas c = (Canvas)shape.Parent;
            //putt inni XPos/YPos: c.RenderSize.Width / 2 - shape.Width / 2 + ... og c.RenderSize.Height / 2 - shape.Height / 2 + ...
            XPos = (int)(c.RenderSize.Width / 2 - shape.Width / 2);
            YPos = (int)(c.RenderSize.Height / 2 - shape.Height / 2);
            Canvas.SetLeft(shape, XPos);
            Canvas.SetTop(shape, YPos);
        }

        public override void Draw()
        {
            Console.Write("Star  : ");
            base.Draw();
        }
    }

    public class Planet : SpaceObject
    {
        public int OrbitalRadius { get; set; }
        public int OrbitalPeriod { get; set; }
        public int ObjectRadius { get; set; }
        public int RotationalPeriod { get; set; }
        public Color Color { get; set; }
        //ha referanse til ellipse?
        public Ellipse shape { get; set; }
        public TextBlock shapeText { get; set; }
        public List<Moon> Moons { get; set; }

        //planetinfo greier... burde nok kanskje vere egen klasse eller noko, blir rot...
        public Ellipse infoShape { get; set; }
        public TextBlock infoText { get; set; }

        public Planet(String name, int orbitalRadius, int orbitalPeriod) : base(name)
        {
            Moons = new();
            OrbitalRadius = orbitalRadius;
            OrbitalPeriod = orbitalPeriod;
        }

        public override void CalcPos(int time)
        {
            Canvas c = (Canvas)shape.Parent;
            int scaledOrbitRadius = SpaceScalingHelper.OrbitalRadiusScaling(c.RenderSize.Width, OrbitalRadius);
            //fart er 2*PI*R/T
            XPos = (int)(c.RenderSize.Width / 2 - shape.Width / 2 + scaledOrbitRadius * Math.Cos(time * (2 * 3.1416 * scaledOrbitRadius / OrbitalPeriod) / 30));
            YPos = (int)(c.RenderSize.Height / 2 - shape.Height / 2 + scaledOrbitRadius * -Math.Sin(time * (2 * 3.1416 * scaledOrbitRadius / OrbitalPeriod) / 30));
            //TODO må ha noko skalering her
            Canvas.SetLeft(shape, XPos);
            Canvas.SetTop(shape, YPos);

            Canvas.SetLeft(shapeText, XPos + shape.Width / 2);
            Canvas.SetTop(shapeText, YPos + shape.Height);
        }

        public virtual void CalcPosInfo(int time)
        {
            Canvas c = (Canvas)infoShape.Parent;
            int x = (int)(c.RenderSize.Width / 2 - infoShape.Width / 2);
            int y = (int)(c.RenderSize.Height / 2 - infoShape.Height / 2);
            Canvas.SetLeft(infoShape, x);
            Canvas.SetTop(infoShape, y);

            //TODO for tekstboks også
            Canvas.SetLeft(infoText, 10);
            Canvas.SetTop(infoText, c.RenderSize.Height - 100);
        }

        public override void Draw()
        {
            Console.Write("Planet: ");
            base.Draw();
        }

        public void DrawMoons()
        {
            Console.WriteLine("Moons: ");
            foreach (Moon m in Moons)
            {
                m?.Draw();
            }
        }

        public override string Info()
        {
            string svar = base.Info() + "\n"
                + "Orbital radius: " + OrbitalRadius + "\n"
                + "Orbital period: " + OrbitalPeriod + "\n"
                + "Rotational period: " + RotationalPeriod + "\n"
                + "Polar radius: " + ObjectRadius + "\n"
                + "Moons: ";
            Moons.ForEach(m => svar += (m.Name + ", "));
            return svar; 
        }
    }

    public class Moon : Planet
    {
        public Planet PlanetOrbiting { get; set; } //???

        public Moon(String name, int orbitalRadius, int orbitalPeriod) : base(name, orbitalRadius, orbitalPeriod) { }

        public override void CalcPos(int time)
        {
            base.CalcPos(time);
            XPos += PlanetOrbiting.XPos;
            YPos += PlanetOrbiting.YPos;
        }

        public override void CalcPosInfo(int time)
        {
            //base.CalcPosInfo(time); //?
            Canvas c = (Canvas)infoShape.Parent;
            int scaledOrbitRadius = SpaceScalingHelper.OrbitalRadiusInfoScaling(c.RenderSize.Width, OrbitalRadius);
            int slowDown = 200;
            if (OrbitalRadius < 100) slowDown = 5500;

            XPos = (int)(c.RenderSize.Width / 2 - infoShape.Width / 2 + scaledOrbitRadius * Math.Cos(time * (2 * 3.1416 * scaledOrbitRadius / OrbitalPeriod) / slowDown));
            YPos = (int)(c.RenderSize.Height / 2 - infoShape.Height / 2 + scaledOrbitRadius * -Math.Sin(time * (2 * 3.1416 * scaledOrbitRadius / OrbitalPeriod) / slowDown));

            Canvas.SetLeft(infoShape, XPos);
            Canvas.SetTop(infoShape, YPos);

            Canvas.SetLeft(infoText, XPos + infoShape.Width / 2);
            Canvas.SetTop(infoText, YPos + infoShape.Height);
        }

        public override void Draw()
        {
            Console.Write("Moon  : ");
            base.Draw();
        }
    }

    public class SmallSolarSystemBody : SpaceObject
    {
        //idk om alle har dette menmen
        public int OrbitalRadius { get; set; }
        public int OrbitalPeriod { get; set; }
        public int ObjectRadius { get; set; }
        public int RotationalPeriod { get; set; }
        public Color Color { get; set; }

        public SmallSolarSystemBody(String name) : base(name) { }

        public override void Draw()
        {
            Console.Write("SSSB  : ");
            base.Draw();
        }

        public override void CalcPos(int time)
        {
            //Canvas c = (Canvas)shape.Parent;
            //putt inni XPos/YPos: c.RenderSize.Width / 2 - shape.Width / 2 + ... og c.RenderSize.Height / 2 - shape.Height / 2 + ...
            XPos = (int)(OrbitalRadius * Math.Cos(time * OrbitalPeriod * 3.1416 / 180));
            YPos = (int)(OrbitalRadius * Math.Sin(time * OrbitalPeriod * 3.1416 / 180));
        }
    }

    public class Comet : SmallSolarSystemBody
    {
        public Comet(String name) : base(name) { }

        public override void Draw()
        {
            Console.Write("Comet : ");
            base.Draw();
        }

    }

    public class Asteroid : SmallSolarSystemBody
    {
        public Asteroid(String name) : base(name) { }

        public override void Draw()
        {
            Console.Write("Asteroid: ");
            base.Draw();
        }

    }

    public class AsteroidBelt : SpaceObject
    {
        public int OrbitalRadius { get; set; }
        public List<Asteroid> Asteroids { get; set; }

        public AsteroidBelt(String name) : base(name)
        {
            Asteroids = new();
        }

        public override void Draw()
        {
            Console.Write("Asteroid belt: ");
            base.Draw();
        }

    }

    //er ikkje ein planet eller SSSB, men effektivt ein planet sååååå...
    public class DwarfPlanet : Planet
    {
        public DwarfPlanet(String name, int orbitalRadius, int orbitalPeriod) : base(name, orbitalRadius, orbitalPeriod) { }

        public override void Draw()
        {
            Console.Write("Dwarf planet: ");
            base.Draw();
        }

    }

    public class SpaceScalingHelper
    {
        /// <summary>
        /// Gir ein skalering på orbitalradius-skalering (?)
        /// </summary>
        /// <param name="canvasWidth"></param>
        /// <param name="canvasHeight"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static int OrbitalRadiusScaling(double canvasWidth, int radius)
        {
            //større width = mindre faktor, mindre width = større faktor
            if ((int)canvasWidth == 0) return 0;
            int scalingFactor = (int)(2000.0 * 1000.0 / canvasWidth);
            double extraScaling = 1.0 + radius / 1250000.0;
            //TODO fiks...
            //if (radius > 500000) extraScaling = 1.6;
            //if (radius > 1000000) extraScaling = 2.2;
            //if (radius > 2000000) extraScaling = 3.5;
            //if (radius > 4000000) extraScaling = 4.7;
            //double bigRadius = ;
            //if (bigRadius >= 0.2) extraScaling += bigRadius;
            int scaledRadius = (int)(radius / (scalingFactor * extraScaling));
            return scaledRadius;
        }

        public static int ObjectRadiusScaling(double windowWidth, int radius)
        {
            //maks radius = 60px? (sola), minimum = 5px? - blir ikkje til scale då,menmen
            //9 objekt som skal teiknast, så ~1/10 av breidda til den største
            int maxR = 50, minR = 6;
            if (radius > 100000) maxR = 75;
            int scalingFactor = (int)(1500.0 * 1000.0 / windowWidth); //større width = mindre faktor, mindre width = større faktor - NB føl ikkje med ein resize!!
            int scaledRadius = radius / scalingFactor;

            if (scaledRadius > maxR) scaledRadius = maxR;
            if (scaledRadius < minR) scaledRadius = minR;
            return scaledRadius;
        }

        public static int ObjectRadiusInfoScaling(double canvasWidth, int radius)
        {
            //TODO fiks!!!
            //maks radius = 60px? (sola), minimum = 5px? - blir ikkje til scale då,menmen
            //9 objekt som skal teiknast, så ~1/10 av breidda til den største
            int maxR = 10, minR = 5;
            if (radius > 100000) maxR = 75;
            int scalingFactor = (int)(1000.0 / canvasWidth); //større width = mindre faktor, mindre width = større faktor - NB føl ikkje med ein resize!!
            int scaledRadius = radius / scalingFactor;

            if (scaledRadius > maxR) scaledRadius = maxR;
            if (scaledRadius < minR) scaledRadius = minR;
            return scaledRadius;
        }

        public static int OrbitalRadiusInfoScaling(double canvasWidth, int radius)
        {
            //todo denne er berre tull...
            //større width = mindre faktor, mindre width = større faktor
            if ((int)canvasWidth == 0) return 0;
            int scalingFactor = (int)(1000.0 / canvasWidth);
            double extraScaling = 1.0 + radius / 100;
            if (radius < 100) extraScaling = 0.12;
            int scaledRadius = (int)(radius / (scalingFactor * extraScaling));
            return scaledRadius;
        }
    }
}
