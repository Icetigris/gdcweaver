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
        public const float TOTAL_SECONDS_TO_ANIMATE = 0.3f;
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

        public void updateChain()
        {
            Console.WriteLine(Globals.Player.CurrentChain.Chain.Count);
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
                            this.convertParticleColorTo(p), twoDparticle, p);
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

            foreach (HUDParticle p in particlesToAnimate)
            {
                p.Draw(gameTime);
            }

            foreach (HUDParticle p in lastTen)
            {
                p.Draw(gameTime);
            }
        }

        //need to update here if additional colors are added
        public Color convertParticleColorTo(Particle p)
        {
            Color aColour = Color.White;

            switch (p.Colour)
            {
                case Particle.Colours.Red:
                    aColour = Color.Red;
                    break;
                case Particle.Colours.Orange:
                    aColour = Color.DarkOrange;
                    break;
                case Particle.Colours.Yellow:
                    aColour = Color.Yellow;
                    break;
                case Particle.Colours.Green:
                    aColour = Color.Green;
                    break;
                case Particle.Colours.Blue:
                    aColour = Color.Blue;
                    break;
                case Particle.Colours.Purple:
                    aColour = Color.Purple;
                    break;
                case Particle.Colours.Silver:
                    aColour = Color.Silver;
                    break;
            }

            return aColour;
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
            private const int MAX_SPEED_MODIFIER = 5;
            Color color;
            Vector2 position;
            Texture2D twoDparticle;
            Queue<Vector2> listOfPositions;
            public Particle particle;
            int numPositionsReached;
            private int currSpeedModifier;

            public HUDParticle(Vector2 newPosition, Color newColor, Texture2D texture, Particle newP)
            {
                this.color = newColor;
                this.position = newPosition;
                twoDparticle = texture;
                listOfPositions = new Queue<Vector2>();
                this.particle = newP;
                numPositionsReached = 0;
                currentSpeedModifier = 0;
            }

            public void Update(GameTime gameTime)
            {
                if (listOfPositions.Count > 0)
                {
                    this.Position += new Vector2((float)((listOfPositions.ElementAt(0).X - this.Position.X) *
                        gameTime.ElapsedGameTime.TotalSeconds / (TOTAL_SECONDS_TO_ANIMATE / currentSpeedModifier)),
                        (float)((listOfPositions.ElementAt(0).Y - this.Position.Y) *
                        gameTime.ElapsedGameTime.TotalSeconds / (TOTAL_SECONDS_TO_ANIMATE / currentSpeedModifier)));

                    //if (this.Position.X == listOfPositions.ElementAt(0).X && this.Position.Y == listOfPositions.ElementAt(0).Y)
                    if (Math.Abs(this.Position.X - listOfPositions.ElementAt(0).X) < 0.5 && Math.Abs(this.Position.Y - listOfPositions.ElementAt(0).Y) < 0.5)
                    {
                        listOfPositions.Dequeue();
                        numPositionsReached++;
                    }
                }
                else
                {
                    currentSpeedModifier = 0;
                }
            }

            public void Draw(GameTime gameTime)
            {
                Globals.hudManager.SpriteBatch.Draw(twoDparticle, position, color);
            }

            public void AddToPositionQueue(Vector2 newLocation)
            {
                listOfPositions.Enqueue(newLocation);
                currentSpeedModifier++;
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

            protected int currentSpeedModifier
            {
                get { return currSpeedModifier; }
                set { currSpeedModifier = value;
                    if(currSpeedModifier > MAX_SPEED_MODIFIER)
                        currSpeedModifier = MAX_SPEED_MODIFIER;
                }
            }
        }
    }
}
