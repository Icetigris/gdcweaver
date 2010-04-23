using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace WorldWeaver
{
    public abstract class CelestialBody : SceneObject
    {
        #region Variables

        private string name; //name of object
        private int mySceneIndex;
        private Vector3 radius; //radius for 3D graphics
        private Vector3 position; //position in 3D space

        private double r; //radius for physics
        private double mass; //mass
        private double volume; //volume
        private double density; //density = mass/volume
        private double effectiveTemp; //surface temperature
        private int gravityPoints; //gravity points
        
        public MoleculePool mPool;

        public const double PI = 3.14159265358979323846;
        public const double FOURoverTHREE = 1.33333333333333333333;
        public const double G = 0.00000000006674;
        public const double SPEEDOFLIGHT = 299792458;


        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int MySceneIndex
        {
            get { return mySceneIndex; }
            set { mySceneIndex = value; }
        }

        public Vector3 Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public double R
        {
            get { return r; }
            set { r = value; }
        }

        public double Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public double Volume
        {
            get
            {
                volume = FOURoverTHREE * PI * (r*r*r); //4/3 pi r^3
                return volume;
            }
        }

        public double Density
        {
            get
            {
                density = mass / volume;
                return density;
            }
        }

        public double EffectiveTemp
        {
            get { return effectiveTemp; }
            set { effectiveTemp = value; }
        }

        public int GravityPoints
        {
            get { return gravityPoints; }
            set { gravityPoints = value; }
        }

        #endregion

        #region Method Outlines

        public abstract void calculateMass();
        public abstract void calculateGravityPoints();

        #endregion
    }
}
