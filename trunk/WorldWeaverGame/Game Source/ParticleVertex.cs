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
    public struct ParticleVertex
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 velocityInitial;
        public Vector3 displacement;
        public Color color;
        public float size;
        public float angle;

        public float age;
        public float lifetime;

        public float offset_X;
        public float offset_Z;

        public bool isAlive;

        public static VertexElement[] VertexElements =
        {
             new VertexElement(0, 0, VertexElementFormat.Vector3, 
                 VertexElementMethod.Default, VertexElementUsage.Position, 0),
             new VertexElement(0, sizeof(float)*3, VertexElementFormat.Vector3, 
                 VertexElementMethod.Default, VertexElementUsage.Position, 1),
             new VertexElement(0, sizeof(float)*6, VertexElementFormat.Vector3, 
                 VertexElementMethod.Default, VertexElementUsage.Position, 2),
             new VertexElement(0, sizeof(float)*9, VertexElementFormat.Color, 
                 VertexElementMethod.Default, VertexElementUsage.Color, 0),
             new VertexElement(0, sizeof(float)*10, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.PointSize, 0),
             new VertexElement(0, sizeof(float)*11, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
                 
             new VertexElement(0, sizeof(float)*12, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
             new VertexElement(0, sizeof(float)*13, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),
             new VertexElement(0, sizeof(float)*14, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 3),
             new VertexElement(0, sizeof(float)*15, VertexElementFormat.Single, 
                 VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 4),
                 
             new VertexElement(0, sizeof(float)*16, VertexElementFormat.Color, 
                 VertexElementMethod.Default, VertexElementUsage.Color, 1),

                 
             new VertexElement(0, sizeof(float)*17, VertexElementFormat.Vector3, 
                 VertexElementMethod.Default, VertexElementUsage.Position, 3),
        };
        public static int SizeInBytes = sizeof(float) * 20;
    }

    struct ParticleVertex_Handler
    {
        public static ParticleVertex Initialize(ParticleVertex p, Vector3 position,
            Behavior behavior)
        {
            Vector3 randVelocity = new Vector3(
                ParticleEffect_Utility.RandomBetween(0.02f,behavior.initialVelocity.X),
                ParticleEffect_Utility.RandomBetween(0.02f, behavior.initialVelocity.Y),
                ParticleEffect_Utility.RandomBetween(0.02f, behavior.initialVelocity.Z)
                );

            p.position = position;
            p.velocity = randVelocity;
            p.velocityInitial = p.velocity;
            p.displacement = position;
            p.size = behavior.initialScale;

            p.color = Color.Black;

            
            p.age = 0.0f;
            p.lifetime = behavior.lifeTime;

            p.offset_X = 0.0f;
            p.offset_Z = 0.0f;

            p.isAlive = true;

            return (p);
        }
    }
}
