#region File Description
//-----------------------------------------------------------------------------
// File:      MenuItem.cs
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
    /// A graphical menu item, that can be clicked, or contain child menu
    /// items that are shown as a popup menu.
    /// </summary>
    public class MenuSeperator : MenuItem
    {
        #region Default Properties
        #endregion

        #region Fields
        private Image image;
        private int numMenuItems;
        private int numClicks;
        private bool showImageMargin;
        private bool isPopUpShown;
        private bool isHighlightShown;
        private bool isEnabled;
        private bool canClose;
        #endregion

        #region Properties
        /// <summary>
        /// Get the image to display beside the menu item text.
        /// </summary>
        /// <value></value>
        public Image Image
        {
            get { return this.image; }
        }

        /// <summary>
        /// Get/Set whether the manu item is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;

                // Change colour to grey if menu item is disabled
                if (this.isEnabled)
                {
                    this.image.Tint = Color.White;
                }
                else
                {
                    this.image.Tint = new Color(255, 255, 255, 128);
                }
            }
        }
        #endregion

        #region Events
        new public event ClickHandler Click;
        internal event PopUpOpenHandler PopUpOpen;
        internal event PopUpClosedHandler PopUpClose;
        internal event CloseAllHandler CloseAll;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public MenuSeperator(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.numMenuItems = 0;
            this.numClicks = 0;
            this.showImageMargin = true;
            this.isPopUpShown = false;
            this.isHighlightShown = false;
            this.isEnabled = true;
            this.canClose = true;

            #region Set Default Properties
            #endregion
        }
        #endregion

        public override void Initialize()
        {
            this.showImageMargin = !(this.Parent is MenuBar);

            base.Initialize();
        }

        /// <summary>
        /// Overridden to prevent any control except MenuItem from being added.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public override void Add(UIComponent control)
        {
            Debug.Assert(false);
        }

        #region Event Handlers
        /// <summary>
        /// Update highlight size.
        /// </summary>
        /// <param name="sender">Resized control</param>
        protected override void OnResize(UIComponent sender)
        {
            base.OnResize(sender);

            //
            this.image.X = 0;
            this.image.Width = this.Width;
        }
        #endregion
    }
}