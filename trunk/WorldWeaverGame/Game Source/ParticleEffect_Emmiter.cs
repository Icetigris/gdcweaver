using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    class ParticleEffect_Emmiter
    {
        #region Fields

        ParticleEffect_Group particleGroup;
        Vector3 previousPosition;

        #endregion


        /// <summary>
        /// Constructs a new particle emitter object.
        /// </summary>
        public ParticleEffect_Emmiter() { }
        public ParticleEffect_Emmiter(Behavior behavior,
                               float particlesPerSecond, Vector3 initialPosition,
                                int numParticles)
        {
            this.particleGroup = new ParticleEffect_Group(initialPosition,behavior,
                numParticles);

            previousPosition = initialPosition;
        }
        public ParticleEffect_Emmiter(Behavior behavior,
                               float particlesPerSecond, Vector3 initialPosition,
                                int numParticles, char axis)
        {
            this.particleGroup = new ParticleEffect_Group(initialPosition, behavior,
                numParticles, axis);

            previousPosition = initialPosition;
        }

        /// <summary>
        /// PickRandomDirection is used by InitializeParticles to decide which direction
        /// particles will move. The default implementation is a random vector in a
        /// circular pattern.
        /// </summary>
        protected virtual Vector3 PickRandomDirection()
        {
            float angle = ParticleEffect_Utility.RandomBetween(0, MathHelper.TwoPi);
            return new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle),
                0.0f);
        }

        public void UpdatePos(GameTime gameTime, Vector3 pos)
        {
            particleGroup.UpdatePos(gameTime,pos);
        }

        public void Draw(GameTime gameTime)
        {
            particleGroup.Draw(gameTime);
        }
    }
}
