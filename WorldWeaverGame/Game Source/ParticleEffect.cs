using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using WorldWeaver;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WorldWeaver
{
    /// <summary>
    /// Particle objects class by Wallace Brown :P. Contains
    /// specific particle information such as position, lifetime,
    /// lifeSpent, velocity, acceleration, etc... DOES NOT have
    /// texture. Textures are managed by the ParticleGroup object!
    /// </summary>
    struct ParticleEffect
    {
        #region variables

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        private Vector3 velocity;
        public Vector3 Velocity
        {
            get { return velocity; }
        }

        private Vector3 accelaration;
        public Vector3 Acceleration
        {
            get { return accelaration; }
        }

        private float lifetime;
        public float LifeTime
        {
            get { return lifetime; }
        }

        private float timeSinceCreation;
        public float TimeSinceCreation
        {
            get { return timeSinceCreation; }
        }

        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public bool Alive
        {
            get { return timeSinceCreation < lifetime; }
        }

        #endregion

        public void Initialize(Vector3 position, Vector3 velocity, Vector3 acceleration,
            float lifetime, float scale, float rotationSpeed)
        {
            this.position = position;
            this.velocity = velocity;
            this.accelaration = acceleration;
            this.lifetime = lifetime;
            this.Scale = scale;

            // reset TimeSinceStart - do this because particles will be
            // reused!
            this.timeSinceCreation = 0.0f;
        }

        public void Update(float dt)
        {
            velocity += Acceleration * dt;
            position += Velocity * dt;

            timeSinceCreation += dt;
        }
    }

    #region vertexDeclaration
    struct ParticleVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public float size;
        public float birth;
        public float lifeTime;
        public float mass;
        public Color color;
        public float angle;
        
        public static readonly VertexElement[] VertexElements =
            {
                // position!
                new VertexElement(0, 0, VertexElementFormat.Vector3,
                                        VertexElementMethod.Default,
                                        VertexElementUsage.Position, 0),
                // Texture!
                new VertexElement(0, 12, VertexElementFormat.Vector2,
                                         VertexElementMethod.Default,
                                         VertexElementUsage.TextureCoordinate, 0),
                             
            };

        public const int SizeInBytes = 20;
    }

    struct ParticleVertex_Handler
    {
        public static ParticleVertex Initialize(ParticleVertex p, Vector3 position,
            Behavior behavior)
        {
            Vector3 randVelocity = new Vector3(
                ParticleEffect_Utility.RandomBetween(0.2f,behavior.initialVelocity.X),
                ParticleEffect_Utility.RandomBetween(0.2f, behavior.initialVelocity.Y),
                ParticleEffect_Utility.RandomBetween(0.2f, behavior.initialVelocity.Z)
                );

            p.position = position;
            p.velocity = randVelocity;
            p.lifeTime = behavior.lifeTime;
            p.size = behavior.initialScale;

            p.mass = 2.0f;
            p.color = Color.Green;

            // reset TimeSinceStart - do this because particles will be
            // reused!
            p.birth = 0.0f;

            return (p);
        }
    }

    #endregion
}
