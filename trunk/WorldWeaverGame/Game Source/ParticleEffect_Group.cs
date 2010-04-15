using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    public class ParticleEffect_Group : SceneObject, IDrawableObject, ILoadableObject, IUpdatableObject
    {
        private GraphicsDevice graphics;
        private Texture2D texture;

        private ChaseCamera camera;

        // A vertex buffer holding our particles. This contains the same data as
        // the particles array, but copied across to where the GPU can access it.
        DynamicVertexBuffer vertexBuffer;

        private Matrix world;

        private CustomEffects visualEffects;

        private Vector3 origin;
        private Behavior behavior;
        public Behavior Behavior
        {
            get { return behavior; }
        }

        ParticleVertex[] particles;
        public ParticleVertex[] Particles
        {
            get { return particles; }
        }

        private int numParticles;
        public int NumParticles
        {
            get { return numParticles; }
        }

        char cardinalAxis;

        public ParticleEffect_Group(Vector3 where,Behavior behavior, 
            int numParticles)
        {
            this.origin = where;
            this.behavior = behavior;
            this.numParticles = numParticles;
            this.graphics = Globals.sceneGraphManager.GraphicsDevice;
            this.visualEffects = new CustomEffects();
            this.world = Matrix.Identity;
            Initialize();
            LoadContent(behavior.texture_filename);
        }
        public ParticleEffect_Group(Vector3 where, Behavior behavior,
           int numParticles, char cardAxis)
        {
            this.origin = where;
            this.behavior = behavior;
            this.numParticles = numParticles;
            this.cardinalAxis = cardAxis;
            this.graphics = Globals.sceneGraphManager.GraphicsDevice;
            this.visualEffects = new CustomEffects();
            this.world = Matrix.Identity;
            Initialize();
            LoadContent(behavior.texture_filename);
        }

        private void LoadContent(String texture_filename)
        {
            // Create a dynamic vertex buffer.
            int size = ParticleVertex.SizeInBytes * particles.Length;

            vertexBuffer = new DynamicVertexBuffer(graphics, size,
                                                   BufferUsage.Points);
            vertexBuffer.SetData(particles);
            texture = Globals.contentManager.Load<Texture2D>(
                "..\\Content\\Textures\\" + texture_filename);
            visualEffects.Phong = Globals.contentManager.Load<Effect>(
                Globals.AssetList.PhongFXPath);
            camera = Globals.ChaseCamera;
        }

        public void Initialize()
        {
            particles = new ParticleVertex[numParticles];
            for (int i = 0; i < particles.Length; i++)
            {
                    particles[i] = new ParticleVertex();

                    particles[i] = ParticleVertex_Handler.Initialize(particles[i],origin,
                        this.behavior);
            }
        }

        public void LoadContent() 
        {
            ReadyToRender = true;
        }
        public void UnloadContent()
        {
            //crap
        }


        /// <summary>
        /// Updates the particle group. The pos updates the group
        /// origin(could be player, could be a shooting asteroid! :O
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="pos"></param>
        public void Update()
        {
            if (behavior.technique == ParticleTechniques.Spray)
            {
                for (int v = 0; v < particles.Length; v++)
                {
                    particles[v].velocity = DegradeVelocity(
                        particles[v].velocity);
                    particles[v] = ResetPos(particles[v]);
                }
            }
        }
        public void UpdatePos(Vector3 pos)
        {
            this.origin = pos;
        }

        #region Utilities

        /// <summary>
        /// Slows the velocity of a particle. Returns the slowed
        /// velocity.
        /// </summary>
        /// <param name="v_in"></param>
        /// <returns></returns>
        private Vector3 DegradeVelocity(Vector3 v_in)
        {
            return (v_in * 0.99f);
        }

        /// <summary>
        /// Checks if a particle has slowed down to whats basically
        /// rest. If so we return the origin of this group so that it
        /// may begin it's journey anew! Else we scoot it along back
        /// to where it was.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private ParticleVertex ResetPos(ParticleVertex current)
        {
            if ((current.velocity.X + current.velocity.Y +
                current.velocity.Z) < 0.09f)
            {
                current.position = origin;
                current.velocity = current.velocityInitial;
            }
            return (current);
        }

        void SetParticleRenderStates(RenderState renderState)
        {
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;
            renderState.PointSizeMin = 10;

            renderState.AlphaBlendEnable = false;
            renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = Blend.SourceAlpha;
            renderState.DestinationBlend = Blend.DestinationAlpha;

            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
        }

        void ResetParticleRenderStates(RenderState renderState)
        {
            renderState.PointSpriteEnable = false;

            renderState.AlphaBlendEnable = false;
            renderState.AlphaTestEnable = false;

            renderState.DepthBufferEnable = false;
            renderState.DepthBufferWriteEnable = true;
        }

        #endregion

        public new void Draw(GameTime gameTime)
        {
            if (!Globals.gameplayScreenDestroyed)
            {
                ResetParticleRenderStates(graphics.RenderState);
                SetParticleRenderStates(graphics.RenderState);
                visualEffects.Set_IsGreymapped(false);
                visualEffects.Set_Specials_Phong(false, false, false, false);
                DrawParticle_Phong(particles, behavior.technique, gameTime);
                ResetParticleRenderStates(graphics.RenderState);
            }
        }

        private void DrawParticle_Phong(ParticleVertex[] p, string technique,
            GameTime gameTime)
        {
            graphics.VertexDeclaration = new VertexDeclaration(
                   graphics, ParticleVertex.VertexElements);
            visualEffects.Phong.CurrentTechnique = visualEffects.Phong.Techniques[technique];
            

            visualEffects.Phong.Begin();
            for (int v = 1; v <= p.Length; v++ )
            {
                foreach (EffectPass pass in visualEffects.Phong.CurrentTechnique.Passes)
                {
                    // Begin current pass
                    pass.Begin();

                    visualEffects.Update_Phong(world, camera.View, camera.Projection, camera.Position);
                    visualEffects.Set_Viewport_Height(graphics.Viewport.Height);

                    visualEffects.Update_Time(gameTime);
                    visualEffects.Phong.Parameters["gTex1"].SetValue(texture);
                    visualEffects.Phong.Parameters["gAccel"].SetValue(new Vector3(0.5f));
                    visualEffects.Phong.Parameters["gVelocity"].SetValue(p[v - 1].velocity);
                    visualEffects.Phong.Parameters["gAngle"].SetValue(30);
                    visualEffects.Phong.Parameters["gGrav"].SetValue(-9.8f);
                    visualEffects.Phong.Parameters["gCenter"].SetValue(this.origin);
                    visualEffects.Phong.Parameters["gRotAxis"].SetValue(this.cardinalAxis);
                    visualEffects.Phong.Parameters["gOrbitSpeed"].SetValue(350);
                    
                    graphics.Vertices[0].SetSource(vertexBuffer, 0,
                                                 ParticleVertex.SizeInBytes);
                    graphics.Indices = null;
                    graphics.DrawPrimitives(
                        PrimitiveType.PointList,
                        0,
                        1);
                    pass.End();
                }
            }
            visualEffects.Phong.End();
        }
    }
}
