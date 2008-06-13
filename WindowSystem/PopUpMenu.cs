#region File Description
//-----------------------------------------------------------------------------
// File:      PopUpMenu.cs
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
using InputEventSystem;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// A graphical popup menu used by menu bars, and as context menus.
    /// </summary>
    public class PopUpMenu : Box
    {
        #region Default Properties
        private static int defaultButtonMargin = 2;
        private static int defaultImageMargin = 1;
        private static Rectangle defaultSkin = new Rectangle(84, 41, 25, 25);
        private static Rectangle defaultMarginImageSkin = new Rectangle(84, 125, 20, 14);

        /// <summary>
        /// Sets the default padding for MenuButton controls.
        /// </summary>
        /// <value>Must be at least 0.</value>
        public static int DefaultButtonMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                defaultButtonMargin = value;
            }
        }

        /// <summary>
        /// Sets the default padding for the margin image.
        /// </summary>
        /// <value>Must be at least 0.</value>
        public static int DefaultImageMargin
        {
            set
            {
                Debug.Assert(value >= 0);
                defaultImageMargin = value;
            }
        }

        /// <summary>
        /// Sets the default skin.
        /// </summary>
        public static Rectangle DefaultSkin
        {
            set { defaultSkin = value; }
        }

        /// <summary>
        /// Sets the default image margin skin.
        /// </summary>
        public static Rectangle DefaultMarginImageSkin
        {
            set { defaultMarginImageSkin = value; }
        }
        #endregion

        #region Fields
        private int buttonMargin;
        private int imageMargin;
        private Image marginImage;
        private List<MenuItem> menuItems;
        private bool isPopUpShown;
        private MenuButton selectedMenuItem;
        #endregion

        #region Properties
        /// <summary>
        /// Get/Set the padding for MenuButton controls.
        /// </summary>
        /// <value>Must be at least 0.</value>
        [SkinAttribute]
        public int ButtonMargin
        {
            get { return this.buttonMargin; }
            set
            {
                Debug.Assert(value >= 0);
                this.buttonMargin = value;
            }
        }

        /// <summary>
        /// Get/Set the padding for the margin image.
        /// </summary>
        /// <value>Must be at least 0.</value>
        [SkinAttribute]
        public int ImageMargin
        {
            get { return this.imageMargin; }
            set
            {
                Debug.Assert(value >= 0);
                this.imageMargin = value;
            }
        }

        /// <summary>
        /// Sets the control skin.
        /// </summary>
        [SkinAttribute]
        public Rectangle Skin
        {
            set { SetSkinLocation(0, value); }
        }

        /// <summary>
        /// Sets the image margin skin.
        /// </summary>
        [SkinAttribute]
        public Rectangle MarginImageSkin
        {
            set
            {
                this.marginImage.Source = value;
                this.marginImage.ResizeToFit();
            }
        }

        /// <summary>
        /// Get/Set whether to show the image margin.
        /// </summary>
        /// <value></value>
        public bool ShowMarginImage
        {
            get { return this.marginImage.Visible; }
            set { this.marginImage.Visible = value; }
        }

        /// <summary>
        /// Get/Set the width of the image margin, if visible.
        /// </summary>
        protected internal int MarginWidth
        {
            // Must subtract buttonMargin because margin is drawn farther left
            get { return this.marginImage.Width - this.buttonMargin; }
        }
        #endregion

        #region Events
        public CloseHandler Close;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public PopUpMenu(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.menuItems = new List<MenuItem>();
            this.isPopUpShown = false;

            #region Create Child Controls
            this.marginImage = new Image(game, guiManager);
            #endregion

            #region Add Child Controls
            base.Add(this.marginImage);
            #endregion

            #region Set Properties
            CanHaveFocus = true;
            #endregion

            #region Set Default Properties
            ButtonMargin = defaultButtonMargin;
            ImageMargin = defaultImageMargin;
            Skin = defaultSkin;
            MarginImageSkin = defaultMarginImageSkin;
            #endregion

            #region Event Handlers
            Close += new CloseHandler(OnClose);
            #endregion
        }
        #endregion

        /// <summary>
        /// Overloaded to prevent any control except MenuItem from being added.
        /// </summary>
        /// <param name="control">MenuItem to add.</param>
        public void Add(MenuItem control)
        {
            MenuButton button = control as MenuButton;

            if (button != null)
            {
                // Add event handlers
                button.PopUpOpen += new PopUpOpenHandler(OnPopUpOpened);
                button.PopUpClose += new PopUpClosedHandler(OnPopUpClosed);
                button.CloseAll += new CloseAllHandler(OnCloseAll);
            }

            // Add Control
            this.menuItems.Add(control);
            base.Add(control);
        }

        /// <summary>
        /// Overridden to prevent any control except MenuItem from being added.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public override void Add(UIComponent control)
        {
            Debug.Assert(false);
        }

        /// <summary>
        /// Refreshes popup size, and the locations of child menu items.
        /// </summary>
        public void Populate()
        {
            if (!this.isPopUpShown)
            {
                // Resize control
                int width = 0;
                int height = this.buttonMargin;

                foreach (MenuItem item in this.menuItems)
                {
                    MenuButton button = item as MenuButton;

                    if (button != null)
                    {
                        // Ensure highlight status is reset
                        button.RemoveHighlight();
                        button.CanClose = false;
                    }

                    item.X = this.buttonMargin;
                    item.Y = height;

                    if (item.Width > width)
                        width = item.Width;
                    height += item.Height;
                }

                // Fix widths to maximum
                foreach (MenuItem item in this.menuItems)
                    item.Width = width;

                width += this.buttonMargin * 2;
                height += this.buttonMargin;

                Width = width;
                Height = height;

                this.isPopUpShown = true;
            }
        }

        public override void Initialize()
        {
            this.marginImage.Scale = true;

            base.Initialize();
        }

        /// <summary>
        /// Closes this popup menu, and child menu.
        /// </summary>
        internal void ClosePopUp()
        {
            Close.Invoke(this);

            if (this.selectedMenuItem != null)
                this.selectedMenuItem.ClosePopUp();

            this.isPopUpShown = false;
        }

        /// <summary>
        /// Checks if the supplied location is within the menu structure (this
        /// control or its children).
        /// </summary>
        /// <param name="x">X-position.</param>
        /// <param name="y">Y-position.</param>
        /// <returns>true if position is inside menu strucure, otherwise false.</returns>
        internal bool CheckMenuCoordinates(int x, int y)
        {
            bool result = false;

            if (CheckCoordinates(x, y)) // This control
                result = true;
            else
            {
                // Child menu items
                foreach (MenuItem item in this.menuItems)
                {
                    MenuButton button = item as MenuButton;
                    if (button == null) continue;

                    if (button.CheckMenuCoordinates(x, y))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if mouse button was released on top of a menu item.
        /// </summary>
        /// <param name="args">Mouse event arguments.</param>
        protected override void MouseUpIntercept(MouseEventArgs args)
        {
            if (this.isPopUpShown && CheckCoordinates(args.Position.X, args.Position.Y))
            {
                // Check if a child menu item should be clicked
                foreach (MenuItem item in this.menuItems)
                {
                    MenuButton button = item as MenuButton;
                    if (button == null) continue;

                    if (button.CheckCoordinates(args.Position.X, args.Position.Y))
                    {
                        button.InvokeClick();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// If any menu is currently open, automatically open popups when the
        /// mouse hovers over the menu item.
        /// </summary>
        /// <param name="args">Mouse event arguments.</param>
        protected override void MouseMoveIntercept(MouseEventArgs args)
        {
            if (this.isPopUpShown && CheckCoordinates(args.Position.X, args.Position.Y))
            {
                foreach (MenuItem item in menuItems)
                {
                    MenuButton button = item as MenuButton;
                    if (button == null) continue;

                    if (button.CheckCoordinates(args.Position.X, args.Position.Y))
                    {
                        if (button != this.selectedMenuItem)
                        {
                            if (this.selectedMenuItem != null)
                                this.selectedMenuItem.ClosePopUp();

                            button.SelectByMove();
                        }
                        break;
                    }
                }
            }
        }

        #region Event Handlers
        /// <summary>
        /// Close when all menus are being closed.
        /// </summary>
        protected void OnCloseAll()
        {
            ClosePopUp();
        }

        /// <summary>
        /// Child menu item has opened its popup.
        /// </summary>
        /// <param name="sender">Menu item opening its popup.</param>
        protected void OnPopUpOpened(object sender)
        {
            this.selectedMenuItem = (MenuButton)sender;
        }

        /// <summary>
        /// Child menu item has closed its popup.
        /// </summary>
        /// <param name="sender">Menu item closing its popup.</param>
        protected void OnPopUpClosed(object sender)
        {
            this.selectedMenuItem = null;
        }

        /// <summary>
        /// When popup is closed, remove it from the GUIManager.
        /// </summary>
        /// <param name="sender">Sender.</param>
        protected void OnClose(object sender)
        {
            GUIManager.Remove(this);
        }

        /// <summary>
        /// Resize image margin.
        /// </summary>
        /// <param name="sender">Sender.</param>
        protected override void OnResize(UIComponent sender)
        {
            // Update bounds of image margin.
            this.marginImage.X = this.imageMargin;
            this.marginImage.Y = this.imageMargin;
            this.marginImage.Height = Height -(this.imageMargin * 2);

            base.OnResize(sender);
        }
        #endregion
    }
}