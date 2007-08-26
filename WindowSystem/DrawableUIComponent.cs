#region File Description
//-----------------------------------------------------------------------------
// File:      DrawableUIComponent.cs
// Namespace: WindowSystem
// Author:    Aaron MacDougall
//-----------------------------------------------------------------------------
#endregion

#region License
//-----------------------------------------------------------------------------
// Copyright (c) 2007, Aaron MacDougall
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of Aaron MacDougall nor the names of its contributors may
//   be used to endorse or promote products derived from this software without
//   specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// UIComponent that draws itself and children to a texture, then draws
    /// that texture onto the screen. Drawing to a texture means the GUI to
    /// limit number of draws, and can allow cool effects like transparency
    /// and animations simpler.
    /// </summary>
    public class DrawableUIComponent : UIComponent
    {
        #region Fields
        private RenderTarget2D renderTarget;
        private Texture2D renderedTexture;
        private SpriteBatch spriteBatch;
        private float transparency;
        private Color color;
        // DELETE AFTER DEBUG
        private static int instanceCount = 0;
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set the component transparency.
        /// </summary>
        /// <value>Between and including 0.0f and 1.0f.</value>
        public float Transparency
        {
            get { return transparency; }
            set
            {
                Debug.Assert(value >= 0.0f && value <= 1.0f);
                transparency = value;
            }
        }

        /// <summary>
        /// Get/Set the control colour shading.
        /// </summary>
        public Color RenderColor
        {
            get { return this.color; }
            set
            {
                Debug.Assert(value != null);
                this.color = value;
            }
        }

        // DELETE AFTER DEBUG
        new public static int InstanceCount
        {
            get { return instanceCount; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public DrawableUIComponent(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.transparency = 1.0f;
            this.color = Color.White;

            // DELETE AFTER DEBUG
            instanceCount++;
        }

        // DELETE AFTER DEBUG
        ~DrawableUIComponent()
        {
            instanceCount--;
        }
        #endregion

        public override void Initialize()
        {
            if (!IsInitialized)
                IsRedrawRequired = true;

            base.Initialize();
        }

        public override void CleanUp()
        {
            if (IsInitialized)
            {
                // Tidy render target
                if (this.renderTarget != null)
                {
                    this.renderTarget.Dispose();
                    this.renderTarget = null;
                }

                this.renderedTexture = null;
            }

            base.CleanUp();
        }

        /// <summary>
        /// Create SpriteBatch object.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
                this.spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadGraphicsContent(loadAllContent);
        }

        /// <summary>
        /// This method is called when the graphics device has been reset, so a
        /// redraw is required.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            // Make changes to handle the new device
            if (this.renderTarget != null)
            {
                this.renderTarget.Dispose();
                this.renderTarget = null;
            }
            // Control must be redrawn after device changes
            Redraw();

            base.UnloadGraphicsContent(unloadAllContent);
        }

        /// <summary>
        /// Creates a new render target, and draws control and children onto
        /// it. Only renders when necessary, allowing the texture instead to be
        /// drawn each frame.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        internal override void DrawTexture(GameTime gameTime)
        {
            // Only redraw texture if required
            if (IsRedrawRequired)
            {
                // First tell all children to draw textures
                base.DrawTexture(gameTime);

                // Create new render target if necessary
                if (this.renderTarget == null ||
                    this.renderTarget.Width != Width ||
                    this.renderTarget.Height != Height
                    )
                {
                    // Cleanup existing render target
                    if (this.renderTarget != null)
                        this.renderTarget.Dispose();

                    this.renderTarget = new RenderTarget2D(
                        GraphicsDevice,
                        Width,
                        Height,
                        1,
                        SurfaceFormat.Color
                        );
                }

                // Draw to texture, which is then drawn to the screen. Allows
                // effects such as transparency, and minimizing renders.
                GraphicsDevice.SetRenderTarget(0, this.renderTarget);

                // Clear to transparent and draw child controls
                GraphicsDevice.Clear(Color.TransparentWhite);

                // Custom render state is used to prevent alpha values from
                // control being copied onto other controls underneith.
                this.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                this.spriteBatch.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
                this.spriteBatch.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.One;

                // Draw this control, and children on top
                DrawControl(this.spriteBatch);
                base.DrawChildren(gameTime, this.spriteBatch);

                this.spriteBatch.End();
                
                // Obtain rendered texture
                GraphicsDevice.ResolveRenderTarget(0);
                this.renderedTexture = this.renderTarget.GetTexture();

                // Set the render target back to the screen
                GraphicsDevice.SetRenderTarget(0, null);

                IsRedrawRequired = false;
            }
        }

        /// <summary>
        /// Override to do the actual drawing of controls.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw control with.</param>
        protected virtual void DrawControl(SpriteBatch spriteBatch)
        {
        }

        /// <summary>
        /// Draws control, then all children if required.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">SpriteBatch to draw control with.</param>
        internal override void DrawChildren(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawControl(gameTime, spriteBatch);

            if (IsRedrawRequired)
                base.DrawChildren(gameTime, spriteBatch);
        }

        /// <summary>
        /// Draws rendered control texture onto the screen.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// <param name="spriteBatch">SpriteBatch to draw control with.</param>
        internal override void DrawControl(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw to screen
            if (renderedTexture != null)
            {
                // Work out colour shading
                Vector4 controlColor = this.color.ToVector4();
                controlColor.W = this.transparency;

                spriteBatch.Draw(
                    this.renderedTexture,
                    Location,
                    new Rectangle(0, 0, Width, Height),
                    new Color(controlColor)
                    );
            }
        }

        //public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, float transparency)
        //{
        //    transparency *= this.transparency;

        //    DrawControl(gameTime, spriteBatch, transparency);

        //    base.Draw(gameTime, spriteBatch, transparency);
        //}

        //protected virtual void DrawControl(GameTime gameTime, SpriteBatch spriteBatch, float transparency)
        //{
        //}

        #region Event Handlers
        /// <summary>
        /// Flags control to be redrawn whenever control is resized.
        /// </summary>
        /// <param name="sender">Resized control.</param>
        protected override void OnResize(UIComponent sender)
        {
            base.OnResize(sender);
            Redraw();
        }
        #endregion
    }
}