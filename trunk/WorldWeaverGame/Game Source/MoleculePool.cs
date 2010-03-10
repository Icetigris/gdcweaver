using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWeaver
{
    public class MoleculePool
    {
      #region Declarations

        List<Particle> particles;

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

        public void appendChainToPool(ParticleChain chain)
        {
            List<Particle> aZeroedChain = chain.Chain;

            foreach (Particle p in aZeroedChain)
            {
                this.particles.Add(p);
            }
        }

      #endregion
    }
}
