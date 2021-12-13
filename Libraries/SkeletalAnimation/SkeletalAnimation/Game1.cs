using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Xml;

namespace SkeletalAnimation
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        /// <summary>
        /// our skeleton instance
        /// </summary>
        Skeleton skeleton;

        /// <summary>
        /// the loaded animation data
        /// </summary>
        SkeletonAnimation skeletonAnimation;

        /// <summary>
        /// the animation player to play an instance of the skeleton animation
        /// </summary>
        SkeletonAnimationPlayer animationPlayer;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            skeleton = Content.Load<Skeleton>("machine_skeleton");
            skeleton.LoadContent(Content);

            skeletonAnimation = Content.Load<SkeletonAnimation>("machine_animation");

            animationPlayer = new SkeletonAnimationPlayer(skeletonAnimation);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            animationPlayer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            animationPlayer.Apply(skeleton);
            skeleton.ComputeAbsoluteTransforms();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            for (int i = 0; i < skeleton.DrawOrder.Length; i++)
            {
                int index = skeleton.DrawOrder[i];
                Transformation key = skeleton.AbsoluteTransforms[index];
                spriteBatch.Draw(skeleton.Textures[index], 
                                    key.Position, null,Color.White, 
                                    key.Rotation, skeleton.Joints[index].TextureOrigin, key.Scale, 
                                    SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
