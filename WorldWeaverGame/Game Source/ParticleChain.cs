#region File Description
//-----------------------------------------------------------------------------
// ParticleChain.cs
//
// World Weaver
// Elizabeth Baumel
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WorldWeaver
{
    public class ParticleChain
    {

      #region Declarations

        int overallCharge;
        const int CHARGE = 25;
        List<Particle> particleChain;
        int totalParticlesCollected;

      #endregion

      #region Properties

        //The current particle chain's overall charge and its getter and setter
        public int OverallCharge
        {
            get
            {
                return overallCharge; 
            }
            set
            {
                overallCharge = value;
            }
        }

        //The list of particles in the current chain and its getter and setter
        public List<Particle> Chain
        {
            get
            {
                return particleChain;
            }
            set
            {
                particleChain = value;
            }
        }

      #endregion

      #region Constructors

        //Makes a new Particle chain with an overall charge of the player's default charge
        public ParticleChain()
        {
            particleChain = new List<Particle>();
            totalParticlesCollected = 0;
        }

      #endregion

      #region Methods

        //adds a particle to the current chain
        public void addParticle(Particle particle)
        {
            particleChain.Add(particle);
            sumChainCharge();
            totalParticlesCollected++;
        }

        //adds the charges of every particle in the chain and returns the overall charge
        public int sumChainCharge()
        {
            //changing to -30  and maybe some other numbers...
            //turns out the slider is not centered...
            overallCharge = 0;

            if (particleChain != null && particleChain.Count > 0)
            {
                foreach (Particle p in particleChain)
                {
                    overallCharge += p.Charge;
                }
            }
















            return overallCharge;
        }

        public void clearChain()
        {
            particleChain.Clear();
        }

        public int totalParicles
        {
            get { return totalParticlesCollected; }
        }

        public override string ToString()
        {
            string outlol = "\n";

            foreach(Particle particle in particleChain)
            {
                outlol += "Charge: " + particle.Charge + ", Colour: " + particle.Colour +"\n";
            }
            outlol += "Overall Charge of Chain: " + overallCharge;
            return outlol;
        }
      #endregion
    }
}
