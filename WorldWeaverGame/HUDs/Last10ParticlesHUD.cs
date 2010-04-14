using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldWeaver
{
    public class Last10ParticlesHUD : HUDElement
    {
        public const int TOTAL_PARTICLES_TO_DISPLAY = 10;
        public const int SIN_CURVE_SCALE = 15;
        public const float TOTAL_SECONDS_TO_ANIMATE = 0.5f;
        private const float FLYING_ANIMATE_SPEED = 1;
        private Texture2D last10InChain;
        private Queue<HUDParticle> lastTen;
        private List<HUDParticle> particlesToAnimate;
        private int TotalParticlesCollected;
        private Texture2D twoDparticle;
        private string twoDparticleString;
        private float[] calculatedCurve;
        private Vector2[] targetLocation;

        public Last10ParticlesHUD()
        {
            TexturePath = "Textures\\last10particlesinchain";
            twoDparticleString = "Textures\\largeparticle";
            lastTen = new Queue<HUDParticle>();
            particlesToAnimate = new List<HUDParticle>();
            TotalParticlesCollected = 0;
            calculatedCurve = new float[TOTAL_PARTICLES_TO_DISPLAY];
            this.CalculateCurve();
            targetLocation = new Vector2[TOTAL_PARTICLES_TO_DISPLAY];
        }

        public override void LoadContent()
        {
            /*
            if (Content == null)
            {
                Content = new ContentManager(HudManager.Game.Services, "Content");
            }
            */
            last10InChain = Globals.hudManager.Game.Content.Load<Texture2D>(TexturePath);
            twoDparticle = Globals.hudManager.Game.Content.Load<Texture2D>(twoDparticleString);

            Position = new Rectangle((int)((Globals.hudManager.HudAreaWidth - Globals.hudManager.HudAreaWidth * 0.1f) - last10InChain.Width / 2),
                                     (int)((Globals.hudManager.HudAreaHeight - Globals.hudManager.HudAreaHeight * 0.6f) - last10InChain.Height / 2),
                                     last10InChain.Width,
                                     last10InChain.Height);


            for (int i = 0; i < TOTAL_PARTICLES_TO_DISPLAY; i++)
            {
                targetLocation[i] = this.calculateEndLocation(i);
                Console.WriteLine(i + " " + targetLocation[i]);
            }
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            updateChain();
        }

        public void Clear()
        {
            lastTen.Clear();
            particlesToAnimate.Clear();
            TotalParticlesCollected = 0;
        }

        public void updateChain()
        {
            if (TotalParticlesCollected != Globals.Player.CurrentChain.Chain.Count)
            {
                List<HUDParticle> tempParticleList = lastTen.ToList();

                int numNewParticles = Math.Max(0, Globals.Player.CurrentChain.Chain.Count - TotalParticlesCollected);

                TotalParticlesCollected = 0;
                lastTen.Clear();

                int arrayOffset = Math.Max(0, Globals.Player.CurrentChain.Chain.Count - TOTAL_PARTICLES_TO_DISPLAY);

                while (TotalParticlesCollected < Globals.Player.CurrentChain.Chain.Count && TotalParticlesCollected < TOTAL_PARTICLES_TO_DISPLAY)
                {
                    Particle p = Globals.Player.CurrentChain.Chain[TotalParticlesCollected + arrayOffset];

                    bool particleExists = false;
                    foreach (HUDParticle hudP in tempParticleList)
                    {
                        if (hudP.particle == p)
                        {
                            lastTen.Enqueue(hudP);
                            particleExists = true;
                            break;
                        }
                    }

                    if (!particleExists)
                    {
                        HUDParticle hudP = new HUDParticle(new Vector2(p.Position.X, p.Position.Y),
                            new Color(p.AssignColor((int)p.Colour)), twoDparticle, p);
                        lastTen.Enqueue(hudP);
                    }

                    TotalParticlesCollected++;
                }

                TotalParticlesCollected = Globals.Player.CurrentChain.Chain.Count;

                //do a compare to see what particles are no longer in there
                foreach (HUDParticle p in lastTen)
                {
                    foreach (HUDParticle aP in tempParticleList)
                    {
                        if (p.particle == aP.particle)
                        {
                            tempParticleList.Remove(aP);
                            break;
                        }
                    }
                }

                //yes, I know it will add a position multiple times. I don't care. -Matt Song
                for (int i = 0; i < lastTen.Count; i++)
                {
                    lastTen.ElementAt(i).AddToPositionQueue(targetLocation[lastTen.Count - (i + 1)]);
                }

                foreach (HUDParticle p in tempParticleList)
                {
                    particlesToAnimate.Add(p);

                    for (int j = 0; j < TOTAL_PARTICLES_TO_DISPLAY; j++)
                    {
                        if (p.Position.Y < targetLocation[j].Y ||
                            p.NumPositionsReached <= 0)
                        {
                            p.AddToPositionQueue(targetLocation[j]);
                        }
                    }

                    p.AddToPositionQueue(targetLocation[TOTAL_PARTICLES_TO_DISPLAY - 1] + new Vector2(1048 - this.Position.X, 0));
                }
            }


        }

        private Vector2 calculateEndLocation(float index)
        {
            int i = (int)index;
            int haxyMultiplier = (i > 8) ? -5 : 1;
            return new Vector2(this.Position.X + 50 + (haxyMultiplier * (calculatedCurve[i] * SIN_CURVE_SCALE)), this.Position.Y + 120 + (i * twoDparticle.Height));
        }

        public override void Draw(GameTime gameTime)
        {
            Globals.hudManager.SpriteBatch.Draw(last10InChain, Position, Color.White);
            DrawParticles(gameTime);

        }

        //yay crappy solution
        public void DrawParticles(GameTime gameTime)
        {
            if (Globals.gameplayScreenHasFocus)
            {
                //Update code here because of easy access to gameTime
                for (int i = 0; i < lastTen.Count; i++)
                {
                    lastTen.ElementAt(i).Update(gameTime);
                }

                int screenWidth = Globals.hudManager.HudAreaWidth + (Globals.hudManager.hudAreaX * 2);

                for (int i = 0; i < particlesToAnimate.Count; i++)
                {
                    particlesToAnimate.ElementAt(i).Update(gameTime);

                    if (particlesToAnimate.ElementAt(i).Position.X > screenWidth
                        && particlesToAnimate.ElementAt(i).QueueCount() <= 0)
                    {
                        particlesToAnimate.RemoveAt(i);
                        i--;
                    }
                }
            }

            foreach (HUDParticle p in particlesToAnimate)
            {
                p.Draw(gameTime);
            }

            foreach (HUDParticle p in lastTen)
            {
                p.Draw(gameTime);
            }
        }

        private void CalculateCurve()
        {
            float increment = (float)(3.5 * Math.PI) / TOTAL_PARTICLES_TO_DISPLAY;
            float increments = 0;

            for (int i = 0; i < TOTAL_PARTICLES_TO_DISPLAY; i++)
            {
                this.calculatedCurve[i] = (float)Math.Sin(increments);
                increments += increment;
            }
        }

        protected class HUDParticle
        {
            Color color;
            Vector2 position, startingPosition;
            Texture2D twoDparticle;
            Queue<Vector2> listOfPositions;
            public Particle particle;
            int numPositionsReached;
            float timeAnimated, totalTimeToAnimate;

            public HUDParticle(Vector2 newPosition, Color newColor, Texture2D texture, Particle newP)
            {
                this.color = newColor;
                this.position = newPosition;
                this.startingPosition = new Vector2(this.Position.X, this.Position.Y);
                twoDparticle = texture;
                listOfPositions = new Queue<Vector2>();
                this.particle = newP;
                numPositionsReached = 0;
                timeAnimated = 0;
                totalTimeToAnimate = TOTAL_SECONDS_TO_ANIMATE;
            }

            public void Update(GameTime gameTime)
            {
                if (listOfPositions.Count > 0)
                {
                    timeAnimated += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    timeAnimated = Math.Min(totalTimeToAnimate, timeAnimated);

                    this.Position = Vector2.Lerp(this.startingPosition, this.listOfPositions.ElementAt(0), timeAnimated / totalTimeToAnimate);

                    if (timeAnimated >= totalTimeToAnimate)
                    {
                        listOfPositions.Dequeue();
                        numPositionsReached++;
                        timeAnimated = 0;
                        startingPosition.X = this.Position.X;
                        startingPosition.Y = this.Position.Y;

                        if (listOfPositions.Count > 0)
                        {
                            totalTimeToAnimate = CalculateAnimationTime(position, listOfPositions.ElementAt(0));
                        }
 
                    }
                }
            }

            protected float CalculateAnimationTime(Vector2 p1, Vector2 p2)
            {
                float distance = Vector2.Distance(p1, p2);

                if (distance < 200)
                    return TOTAL_SECONDS_TO_ANIMATE / (Math.Min(listOfPositions.Count, 3));
                else
                    return TOTAL_SECONDS_TO_ANIMATE * 2.5f;
            }

            public void Draw(GameTime gameTime)
            {
                Globals.hudManager.SpriteBatch.Draw(twoDparticle, position, color);
            }

            public void AddToPositionQueue(Vector2 newLocation)
            {
                listOfPositions.Enqueue(newLocation);

                if (listOfPositions.Count == 1)
                {
                    totalTimeToAnimate = CalculateAnimationTime(position, listOfPositions.ElementAt(0));
                }
            }

            public Vector2 Position
            {
                get { return position; }
                set { position = value; }
            }

            public int QueueCount()
            {
                return this.listOfPositions.Count;
            }

            public int NumPositionsReached
            {
                get { return numPositionsReached; }
                protected set { numPositionsReached = value; }
            }
        }
    }
}
