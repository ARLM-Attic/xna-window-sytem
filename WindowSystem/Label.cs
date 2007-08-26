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
using XNAExtras;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A graphical text label.
    /// </summary>
    public class Label : DrawableUIComponent
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
        private BitmapFont font;
        private Color color;
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
                }
            }
        }

        /// <summary>
        /// Sets the text font.
        /// </summary>
        /// <value>Must not be null.</value>
        public BitmapFont Font
        {
            set
            {
                Debug.Assert(value != null);
                this.font = value;
                Redraw();
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
                    result = this.font.LineHeight;
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
                    result = this.font.MeasureString(Text);
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
                Font = GUIManager.ContentManager.Load<BitmapFont>(defaultFont);

            base.LoadGraphicsContent(loadAllContent);
        }

        /// <summary>
        /// Draws text.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw control with.</param>
        protected override void DrawControl(SpriteBatch spriteBatch)
        {
            // Draw text
            if (this.font != null)
            {
                string text = "";

                if (this.text != null)
                    text += this.text;

                if (this.isCursorShown)
                    text += this.cursor;

                if (text.Length > 0)
                {
                    this.font.SpriteBatchOverride(spriteBatch);
                    this.font.DrawString(0, 0, this.color, text);
                }
            }
        }

        //protected override void DrawControl(GameTime gameTime, SpriteBatch spriteBatch, float transparency)
        //{
        //    // Draw text
        //    if (this.font != null)
        //    {
        //        string text = "";

        //        if (this.text != null)
        //            text += this.text;

        //        if (this.isCursorShown)
        //            text += this.cursor;

        //        if (text.Length > 0)
        //        {
        //            this.font.SpriteBatchOverride(spriteBatch);
                    
        //            // Fix up alpha
        //            Vector4 vector = this.color.ToVector4();
        //            vector.W = transparency;
        //            Color color = new Color(vector);

        //            this.font.DrawString(AbsolutePosition.X, AbsolutePosition.Y, color, text);
        //        }
        //    }
        //}
    }
}