using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    //This is where all the players collected particles are dumped
    //You use the particles in here to make CelestialBodies
    //It is cleared when a CelestialBody is made
    public class MoleculePool
    {
      #region Declarations

        List<Particle> particles;
        List<CountableColour> frequencies = new List<CountableColour>();

      #endregion

      #region Properties

        public List<Particle> Particles
        {
            get
            {
                return particles;
            }
        }

      #endregion

      #region Constructors

        public MoleculePool()
        {
            particles = new List<Particle>();
        }

      #endregion

      #region Methods

        //Takes the player's current, now zeroed chain and adds it to the MoleculePool
        public void appendChainToPool(ParticleChain chain)
        {
            List<Particle> aZeroedChain = chain.Chain;

            foreach (Particle p in aZeroedChain)
            {
                this.particles.Add(p);
            }
        }        

        //sort by most common colour
        //return a list of Colours in most common to least common order
        public Particle.Colours[] getColourFrequencies()
        {
            //int count = 0;
            int red = 0, orange = 0, yellow = 0, green = 0, blue = 0, purple = 0, silver = 0, shiny = 0;

            Particle.Colours[] freqs = new Particle.Colours[8];


            foreach (Particle p in particles)
            {
                switch (p.Colour)
                {
                    case Particle.Colours.Red:
                        red++;
                        break;
                    case Particle.Colours.Orange:
                        orange++;
                        break;
                    case Particle.Colours.Yellow:
                        yellow++;
                        break;
                    case Particle.Colours.Green:
                        green++;
                        break;
                    case Particle.Colours.Blue:
                        blue++;
                        break;
                    case Particle.Colours.Purple:
                        purple++;
                        break;
                    case Particle.Colours.Silver:
                        silver++;
                        break;
                    case Particle.Colours.Shiny:
                        shiny++;
                        break;
                }

            }
            frequencies.Add(new CountableColour(red, Particle.Colours.Red));
            frequencies.Add(new CountableColour(orange, Particle.Colours.Orange));
            frequencies.Add(new CountableColour(yellow, Particle.Colours.Yellow));
            frequencies.Add(new CountableColour(green, Particle.Colours.Green));
            frequencies.Add(new CountableColour(blue, Particle.Colours.Blue));
            frequencies.Add(new CountableColour(purple, Particle.Colours.Purple));
            frequencies.Add(new CountableColour(silver, Particle.Colours.Silver));
            frequencies.Add(new CountableColour(shiny, Particle.Colours.Shiny));

            frequencies.Sort(); //have to change this to sort on Key
            frequencies.Reverse();

            int i = 0;
            foreach (CountableColour cc in frequencies)
            {
                freqs[i] = cc.Colour;
                i++;
            }
            i = 0;
            frequencies.Clear();

            return freqs;
        }

      #endregion
    }
}
