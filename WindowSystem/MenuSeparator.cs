#region File Description
//-----------------------------------------------------------------------------
// File:      MenuSeparator.cs
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
using InputEventSystem;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A menu divider.
    /// </summary>
    sealed public class MenuSeparator : MenuItem
    {
        #region Default Properties
        private static Rectangle defaultSeparatorSkin = new Rectangle(111, 124, 14, 2);
        #endregion

        #region Fields
        private Image image;
        #endregion

        #region Properties
        /// <summary>
        /// Sets the skin to use for the menu divider.
        /// </summary>
        [SkinAttribute]
        public Rectangle SeparatorSkin
        {
            set
            {
                // Set image source area and refresh
                this.image.Source = value;
                SetSize();
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public MenuSeparator(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            #region Create Child Controls
            this.image = new Image(game, guiManager);
            #endregion

            #region Add Child Controls
            base.Add(this.image);
            #endregion

            #region Set Default Properties
            SeparatorSkin = defaultSeparatorSkin;
            #endregion
        }
        #endregion

        /// <summary>
        /// Overridden to prevent controls from being added.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public override void Add(UIComponent control)
        {
            Debug.Assert(false, "No controls can be added to this component.");
        }

        /// <summary>
        /// Resizes height to match divider image.
        /// </summary>
        private void SetSize()
        {
            this.image.ResizeToFit();
            this.image.Y = vMargin;
            this.Height = this.image.Height + (base.vMargin * 2);
        }

        #region EventHandlers
        /// <summary>
        /// Keep divider height the same, but resize to width of parent.
        /// </summary>
        /// <param name="sender">Resizing control.</param>
        protected override void OnResize(UIComponent sender)
        {
            base.OnResize(sender);

            this.image.Width = this.Width;
            this.image.Scale = true;
        }

        /// <summary>
        /// Refresh margins when they have changed.
        /// </summary>
        protected override void OnMarginsChanged()
        {
            base.OnMarginsChanged();

            SetSize();
        }
        #endregion
    }
}