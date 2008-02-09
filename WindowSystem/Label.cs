#region File Description
//-----------------------------------------------------------------------------
// File:      Label.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A graphical text label.
    /// </summary>
    public class Label : UIComponent
    {
        #region Default Properties
        private static string defaultFont = "Content/Fonts/DefaultFont";
        private static Color defaultColor = Color.Black;

        /// <summary>
        /// Sets the default font.
        /// </summary>
        /// <value>Must be a non-empty string.</value>
        public static string DefaultFont
        {
            set
            {
                Debug.Assert(value != null);
                Debug.Assert(value.Length > 0);
                defaultFont = value;
            }
        }

        /// <summary>
        /// Sets the default text colour.
        /// </summary>
        public static Color DefaultColor
        {
            set { defaultColor = value; }
        }
        #endregion

        #region Fields
        private string text;
        private char cursor;
        private bool isCursorShown;
        private SpriteFont font;
        private Color color;
        private RenderTarget2D renderTarget;
        private Texture2D renderedTexture;
        private bool isRedrawRequired;
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set the label text.
        /// </summary>
        /// <value>Must not be null.</value>
        public string Text
        {
            get { return text; }
            set
            {
                Debug.Assert(value != null);

                this.text = value;
                Redraw();
                this.isRedrawRequired = true;
            }
        }

        /// <summary>
        /// Get/Set whether the cursor should be shown.
        /// </summary>
        public bool IsCursorShown
        {
            get { return isCursorShown; }
            set
            {
                if (value != this.isCursorShown)
                {
                    this.isCursorShown = value;
                    Redraw();
                    this.isRedrawRequired = true;
                }
            }
        }

        /// <summary>
        /// Sets the text font.
        /// </summary>
        /// <value>Must not be a valid path.</value>
        public string Font
        {
            set
            {
                this.font = GUIManager.ContentManager.Load<SpriteFont>(value);
                Redraw();
                this.isRedrawRequired = true;
            }
        }

        /// <summary>
        /// Get/Set the text colour.
        /// </summary>
        public Color Color
        {
            get { return this.color; }
            set
            {
                this.color = value;
                Redraw();
                this.isRedrawRequired = true;
            }
        }

        /// <summary>
        /// Gets the font height.
        /// </summary>
        public int TextHeight
        {
            get
            {
                int result = 0;
                if (this.font != null)
                    result = this.font.LineSpacing;
                return result;
            }
        }

        /// <summary>
        /// Gets the width of current text.
        /// </summary>
        public int TextWidth
        {
            get
            {
                int result = 0;
                if (this.font != null)
                    result = (int)(this.font.MeasureString(Text).X + 1.0f);
                return result;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public Label(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.text = string.Empty;
            this.cursor = '_';
            this.isCursorShown = false;

            #region Properties
            CanHaveFocus = false;
            #endregion

            #region Set Default Properties
            Color = defaultColor;
            #endregion
        }
        #endregion

        /// <summary>
        /// Load default font.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
                Font = defaultFont;

            base.LoadGraphicsContent(loadAllContent);
        }

        /// <summary>
        /// Invalidate the control so that the texture is drawn on the first
        /// frame.
        /// </summary>
        public override void Initialize()
        {
            this.isRedrawRequired = true;

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
            this.isRedrawRequired = true;

            base.UnloadGraphicsContent(unloadAllContent);
        }

        /// <summary>
        /// Draws the text and performs clipping. Clipping is implemented by
        /// rendering to a texture, which can be kept to draw each frame, until
        /// the control is invalidated, such as a text change, a resize, colour
        /// change, or if the graphics device becomes invalidated.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        /// <param name="parentScissor">The scissor region of the parent control.</param>
        protected override void DrawControl(SpriteBatch spriteBatch, Rectangle parentScissor)
        {
            if (this.font != null)
            {
                // Has the control become invalid?
                if (this.isRedrawRequired)
                {
                    string text = "";

                    if (this.text != null)
                        text += this.text;

                    if (this.isCursorShown)
                        text += this.cursor;

                    // End the current sprite drawing, so that immediate mode
                    // isn't affected.
                    spriteBatch.End();

                    // Create a new render target
                    this.renderTarget = new RenderTarget2D(
                        GraphicsDevice,
                        Width,
                        Height,
                        1,
                        SurfaceFormat.Color
                        );

                    GraphicsDevice.SetRenderTarget(0, this.renderTarget);

                    // Clear to transparent (same colour as text to fix alpha
                    // blending)
                    Color clearColor = new Color(new Vector4(this.color.R, this.color.G, this.color.B, 0.0f));
                    GraphicsDevice.Clear(clearColor);

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

                    // Custom alpha blending prevents text getting screwed up
                    spriteBatch.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = true;
                    spriteBatch.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    spriteBatch.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
                    spriteBatch.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One;
                    spriteBatch.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha;

                    spriteBatch.DrawString(this.font, text, new Vector2(0, 0), color);

                    spriteBatch.End();

                    // Save rendered texture
                    GraphicsDevice.ResolveRenderTarget(0);
                    this.renderedTexture = this.renderTarget.GetTexture();

                    // Set the render target back to the screen
                    GraphicsDevice.SetRenderTarget(0, null);

                    this.isRedrawRequired = false;

                    // Start the original drawing mode again, so that other
                    // controls are not affected.
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                }

                if (this.renderedTexture != null)
                {
                    bool draw = true;
                    Rectangle source;
                    Rectangle destination;
                    int dif;

                    source = new Rectangle(0, 0, Width, Height);
                    destination = new Rectangle(AbsolutePosition.X, AbsolutePosition.Y, Width, Height);

                    if (!parentScissor.Contains(destination))
                    {
                        // Perform culling
                        if (parentScissor.Intersects(destination))
                        {
                            // Perform clipping

                            if (destination.X < parentScissor.X)
                            {
                                dif = parentScissor.X - destination.X;

                                if (destination.Width == source.Width)
                                {
                                    source.Width -= dif;
                                    source.X += dif;
                                    destination.Width -= dif;
                                    destination.X += dif;
                                }
                                else
                                {
                                    destination.Width -= dif;
                                    destination.X += dif;
                                }
                            }
                            else if (destination.Right > parentScissor.Right)
                            {
                                dif = destination.Right - parentScissor.Right;

                                if (destination.Width == source.Width)
                                {
                                    source.Width -= dif;
                                    destination.Width -= dif;
                                }
                                else
                                    destination.Width -= dif;
                            }

                            if (destination.Y < parentScissor.Y)
                            {
                                dif = parentScissor.Y - destination.Y;

                                if (destination.Height == source.Height)
                                {
                                    source.Height -= dif;
                                    source.Y += dif;
                                    destination.Height -= dif;
                                    destination.Y += dif;
                                }
                                else
                                {
                                    destination.Height -= dif;
                                    destination.Y += dif;
                                }
                            }
                            else if (destination.Bottom > parentScissor.Bottom)
                            {
                                dif = destination.Bottom - parentScissor.Bottom;

                                if (destination.Height == source.Height)
                                {
                                    source.Height -= dif;
                                    destination.Height -= dif;
                                }
                                else
                                    destination.Height -= dif;
                            }
                        }
                        else
                            draw = false;
                    }

                    if (draw)
                    {
                        // Actually draw finally!
                        spriteBatch.Draw(
                            this.renderedTexture,
                            destination,
                            source,
                            Color.White
                            );
                    }
                }
            }
        }
    }
}