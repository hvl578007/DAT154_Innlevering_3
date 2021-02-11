using System;
using System.Collections.Generic;
using SpaceSim;

namespace Oblig3Console
{
    class Astronomy
    {
        //public Action<int> delCalc;

        static void Main(string[] args)
        {
            //DelCalc dc;

            List<SpaceObject> solarSystem = new List<SpaceObject>
            {
                new Star("Sun"), //0
                new Planet("Mercury", 57910, 88), //1
                new Planet("Venus", 108200, 225), //2
                new Planet("Earth", 149600, 365), //3
                new Planet("Mars", 227940, 687), //4
                new Planet("Jupiter", 778330, 4333), //5
                new Planet("Saturn", 1429400, 10760), //6
                new Planet("Uranus", 2879889, 39685), //7
                new Planet("Neptune", 4504300, 60190), //8
                new DwarfPlanet("Pluto", 5913520, 90550) //9
            };
            //TODO må gjere noko betre enn dette... (ein måne MÅ ha x/y koordinatane til planeten den går rundt...) Planet har liste av måner?
            Moon moon = new Moon("The Moon", 384, 27);
            moon.PlanetOrbiting = solarSystem[3] as Planet;
            solarSystem.Add(moon);
            (solarSystem[3] as Planet).Moons.Add(moon);
            moon = new Moon("Europa", 671, 4);
            moon.PlanetOrbiting = solarSystem[5] as Planet;
            solarSystem.Add(moon);
            (solarSystem[5] as Planet).Moons.Add(moon);
            moon = new Moon("Mimas", 186, 1);
            moon.PlanetOrbiting = solarSystem[6] as Planet;
            (solarSystem[6] as Planet).Moons.Add(moon);
            solarSystem.Add(new AsteroidBelt("The Asteroid Belt"));

            

            Console.WriteLine("Time (number of days):");
            String strTime = Console.ReadLine();
            int time = int.Parse(strTime);

            foreach (SpaceObject obj in solarSystem)
            {
                obj.CalcPos(time);
                //dc += obj.CalcPos;
            }

            Console.WriteLine("Name of the planet:");
            String name = Console.ReadLine();
            SpaceObject sObj = solarSystem.Find(s => s.Name.Equals(name));
            if (sObj is not null)
            {
                sObj.Draw();
                (sObj as Planet)?.DrawMoons();
            } else
            {
                //skriv ut info om berre sola og planetane
                solarSystem[0].Draw();
                solarSystem.FindAll(s => s is Planet).ForEach(s => s.Draw());
            }

            /*
            foreach (SpaceObject obj in solarSystem)
            {
                obj.Draw();
            }
            */

            Console.ReadLine();
        }
    }

}
