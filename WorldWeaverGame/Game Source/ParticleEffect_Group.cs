using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace WorldWeaver
{
    class ParticleEffect_Group
    {
        public const int AlphaBlendDrawOrder = 100;
        public const int AdditiveDrawOrder = 200;

        private GraphicsDevice graphics;
        private SpriteBatch spriteBatch;
        private Texture2D texture;

        private ChaseCamera camera;

        // A vertex buffer holding our particles. This contains the same data as
        // the particles array, but copied across to where the GPU can access it.
        DynamicVertexBuffer vertexBuffer;

        // Vertex declaration describes the format of our ParticleVertex structure.
        VertexDeclaration vertexDeclaration;

        private Matrix world;

        private CustomEffects visualEffects;

        private Vector3 origin;
        private Behavior behavior;
        public Behavior Behavior
        {
            get { return behavior; }
        }

        private int numParticles;
        ParticleVertex[] particles;
        public ParticleVertex[] Particles
        {
            get { return particles; }
        }

        private float addCounter;
        public float AddCounter
        {
            get { return addCounter; }
            set { addCounter = value; }
        }
        public float AddRate
        {
            get { return behavior.addRate; }
        }

        public int NumParticles
        {
            get { return numParticles; }
        }

        char cardAxis;

        public ParticleEffect_Group(Vector3 where,Behavior behavior, 
            int numParticles)
        {
            this.origin = where;
            this.behavior = behavior;
            this.numParticles = numParticles;
            this.graphics = Globals.sceneGraphManager.GraphicsDevice;
            this.spriteBatch = new SpriteBatch(graphics);
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
            this.cardAxis = cardAxis;
            this.graphics = Globals.sceneGraphManager.GraphicsDevice;
            this.spriteBatch = new SpriteBatch(graphics);
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
                                                   BufferUsage.WriteOnly |
                                                   BufferUsage.Points);

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

        public void UpdatePos(GameTime gameTime, Vector3 pos)
        {
            this.origin = pos;
        }

        public void Draw(GameTime gameTime)
        {
            if (!Globals.gameplayScreenDestroyed)
            {

                // Restore the vertex buffer contents if the graphics device was lost.
                if (vertexBuffer.IsContentLost)
                {
                    vertexBuffer.SetData(particles);
                }
                SetParticleRenderStates(graphics.RenderState);



                visualEffects.Set_IsGreymapped(false);
                visualEffects.Set_Specials_Phong(false, false, false, false);
                DrawParticle_Phong_2(particles, "Particle_Orbit", gameTime);
            }
        }

        private void DrawParticle_Phong_2(ParticleVertex[] p, string technique,
            GameTime gameTime)
        {
            visualEffects.Phong.CurrentTechnique = visualEffects.Phong.Techniques[technique];
            graphics.VertexDeclaration = new VertexDeclaration(
                graphics, ParticleVertex.VertexElements);

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
                    visualEffects.Phong.Parameters["gTex0"].SetValue(texture);
                    visualEffects.Phong.Parameters["gAccel"].SetValue(new Vector3(0.5f));
                    visualEffects.Phong.Parameters["gVelocity"].SetValue(p[v - 1].velocity);
                    visualEffects.Phong.Parameters["gAngle"].SetValue(45);
                    visualEffects.Phong.Parameters["gGrav"].SetValue(-9.8f);
                    visualEffects.Phong.Parameters["gCenter"].SetValue(this.origin);
                    visualEffects.Phong.Parameters["gRotAxis"].SetValue(this.cardAxis);
                    visualEffects.Phong.Parameters["gOrbitSpeed"].SetValue(350);
                    graphics.Vertices[0].SetSource(vertexBuffer, 0,
                                                 ParticleVertex.SizeInBytes);
                    graphics.DrawPrimitives(
                        PrimitiveType.PointList,
                        0,
                        1);
                    pass.End();
                }
            }
            visualEffects.Phong.End();


            graphics.RenderState.PointSpriteEnable = false;
            graphics.RenderState.DepthBufferWriteEnable = true;
            graphics.RenderState.AlphaBlendEnable = false;
            graphics.RenderState.AlphaTestEnable = false;
        }

        void SetParticleRenderStates(RenderState renderState)
        {
            // Enable point sprites.
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;
            renderState.PointSizeMin = 10;

            //Set the alpha blend mode.
            //renderState.AlphaBlendEnable = true;
            //renderState.AlphaBlendOperation = BlendFunction.Add;
            //renderState.SourceBlend = Blend.SourceAlpha;
            //renderState.DestinationBlend = Blend.DestinationAlpha;

            // Set the alpha test mode.
            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
        }
    }
}
