using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    class SolarSystem
    {
        //int gravityPoints; //gravity points = SolarSystem.Sun.Mass * 10
        bool centralStarExists = false; //true when there is at least 1 star in the bodyList, false when it's empty
        Dictionary<string, CelestialBody> bodyList;
        Dictionary<string, int> hitList;

        #region Constructor

        public SolarSystem()
        {
            bodyList = new Dictionary<string, CelestialBody>();
            hitList = new Dictionary<string, int>();
        }

        #endregion

        #region Methods

        //adds CelestialBody object to SolarSystem
        public void Add(CelestialBody body)
        {
            //I got an exception for having 2 things with the same key in a Dictionary once
            //but I can't seem to replicate the error. - Elizabeth
            if (bodyList.ContainsKey(body.Name) && !hitList.ContainsKey(body.Name))
            {
                hitList.Add(body.Name, 1);
                string tempName = body.Name + "1";
                Console.WriteLine("HAY GUISE");
                bodyList.Add(tempName, body);
            }
            else if (bodyList.ContainsKey(body.Name) && hitList.ContainsKey(body.Name))
            {
                int currentHitCount;
                hitList.TryGetValue(body.Name, out currentHitCount);
                currentHitCount++;
                hitList.Remove(body.Name);
                hitList.Add(body.Name, currentHitCount);
                string tempName = body.Name + currentHitCount;
                Console.WriteLine("HURR");
                bodyList.Add(tempName, body);
            }
            else if (!bodyList.ContainsKey(body.Name))
            {
                Console.WriteLine("derp");
                bodyList.Add(body.Name, body);
            }
        }


        //if the bodyList is empty
        //the only thing a player can make is a star
        //otherwise, they can make whatever they want
        public bool SystemEmpty()
        {
            if (bodyList.Count < 1)
            {
                centralStarExists = false;
            }
            else
            {
                centralStarExists = true;
            }

            return centralStarExists;
        }

        #endregion
    }
}
