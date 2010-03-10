#region File Description
//-----------------------------------------------------------------------------
// Planet.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    class Planet : CelestialBody
    {
        //has an atmosphere
        //core composition (does it have a magnetosphere?)

        #region Constructor

        public Planet(string name, Vector3 radius, Vector3 position, double r, MoleculePool pool)
        {
            this.Name = name;
            this.Radius = radius;
            this.Position = position;
            this.R = r;
            this.mPool = pool;
        }

        #endregion

        #region Methods

        //if mass of the planet is at least 30% due to silver particles, you get a magnetic field
        public void hasMagneticField()
        {
            foreach (Particle p in mPool.Particles)
            {
                //
            }
        }

        //calculate mass of planet from sum of particle masses
        public override void calculateMass()
        {
            foreach (Particle p in mPool.Particles)
            {
                Mass += p.AssignMass((int)p.Colour);
            }
        }

        //calculate gravity points for the planet (for moons and stuff)
        public override void calculateGravityPoints()
        {
            GravityPoints = (int)R * 2;
        }

        #endregion
    }
}
