using System;
using System.Collections.Generic;
using System.Text;


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
            //gjer ingenting? -> flytte til Planet og SSSB??
        }

        public virtual void Draw()
        {
            Console.WriteLine(Name + ". X: " + XPos.ToString() + ", Y: " + YPos.ToString() + ".");
        }
    }

    public class Star : SpaceObject
    {
        public int ObjectRadius { get; set; }
        public int RotationalPeriod { get; set; }
        public String Color { get; set; }

        public Star(String name) : base(name) {
            XPos = 0; //todo?
            YPos = 0;
        }

        public override void CalcPos(int time)
        {
            //står i ro...
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
        public String Color { get; set; }
        //ha referanse til ellipse?
        public List<Moon> Moons { get; set; }

        public Planet(String name, int orbitalRadius, int orbitalPeriod) : base(name)
        {
            Moons = new();
            OrbitalRadius = orbitalRadius;
            OrbitalPeriod = orbitalPeriod;
        }

        public override void CalcPos(int time)
        {
            //Canvas c = (Canvas)shape.Parent;
            //putt inni XPos/YPos: c.RenderSize.Width / 2 - shape.Width / 2 + ... og c.RenderSize.Height / 2 - shape.Height / 2 + ...
            XPos = (int)(OrbitalRadius * Math.Cos(time * OrbitalPeriod * 3.1416 / 180));
            YPos = (int)(OrbitalRadius * Math.Sin(time * OrbitalPeriod * 3.1416 / 180));
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
        public String Color { get; set; }

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
}
