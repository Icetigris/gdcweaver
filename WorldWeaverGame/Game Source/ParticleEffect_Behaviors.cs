using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    /// <summary>
    /// Yarg! This is the structure that organizes the
    /// different attributes of the sprites that can
    /// be implemented. Things such as texture,
    /// travel course, life span, etc... 
    /// Wallace Brown
    /// </summary>
    struct Behavior
    {
        public String texture_filename;

        public Vector3 initialVelocity;

        public Vector3 initialAcceleration;

        public float lifeTime;

        public float initialScale;

        public int numParticles;

        public SpriteBlendMode spriteBlendMode;

        public int drawOrder;

        public float addRate;

        public Behavior(String texture_filename,
            Vector3 initialVelocity,
            Vector3 initialAcceleration,
            float lifeTime,
            float initialScale,
            int numParticles,
            SpriteBlendMode spriteBlendMode, int drawOrder,
            float addRate)
        {
            this.texture_filename = texture_filename;
            this.initialVelocity = initialVelocity;
            this.initialAcceleration = initialAcceleration;
            this.lifeTime = lifeTime;
            this.initialScale = initialScale;
            this.numParticles = numParticles;
            this.spriteBlendMode = spriteBlendMode;
            this.drawOrder = drawOrder;
            this.addRate = addRate;
        }
    }
    
    /// <summary>
    /// Pre-defined
    /// attributes to make your sprite group.
    /// Wallace Brown
    /// </summary>
    class ParticleEffect_Behavior
    {
        public static Behavior 
        orbit = new Behavior( "testParticle",
                    new Vector3(1.0f),
                    new Vector3(1.0f),
                    5.0f,
                    1.0f,
                    10,
                    SpriteBlendMode.AlphaBlend,
                    100,
                    2.0f
        );
    }
}
