#region File Description
//-----------------------------------------------------------------------------
// File:      SkinnedComponent.cs
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
    /// Handy for referring to skins by name rather than by their index.
    /// </summary>
    public enum SkinState
    {
        Normal,
        Hover,
        Pressed,
        Checked,
        CheckedHover,
        CheckedPressed
    }

    /// <summary>
    /// Interface allows for classes containing SkinnedComponent objects, to
    /// pass interfaces for changing skin locations, rather than the full
    /// object.
    /// </summary>
    public interface ISingleSkin
    {
        /// <summary>
        /// Sets the location on the source texture of the control skin.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle SkinLocation
        {
            set;
        }
    }

    /// <summary>
    /// Interface allows for classes containing SkinnedComponent objects, to
    /// pass interfaces for changing skin locations, rather than the full
    /// object. This version has the regular state from ISingleSkin, as well as
    /// states for hover and pressed states.
    /// </summary>
    public interface IThreeSkins : ISingleSkin
    {
        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle HoverSkinLocation
        {
            set;
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle PressedSkinLocation
        {
            set;
        }
    }

    public interface ISixSkins : IThreeSkins
    {
        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle CheckedSkinLocation
        {
            set;
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle CheckedHoverSkinLocation
        {
            set;
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        Rectangle CheckedPressedSkinLocation
        {
            set;
        }
    }

    /// <summary>
    /// Combines two Rectangles representing the source and destination of an
    /// image.
    /// </summary>
    public class GUIRect
    {
        /// <summary>
        /// The location on the source texture.
        /// </summary>
        public Rectangle Source;
        /// <summary>
        /// The location on the destination texture.
        /// </summary>
        public Rectangle Destination;
    }

    /// <summary>
    /// Represents a GUI skin, with a reference to the source texture, and a
    /// list of GUIRect objects which make up the different parts of the skin.
    /// </summary>
    public class ComponentSkin
    {
        /// <summary>
        /// The texture used by the skin. Only used if the default GUI texture
        /// is not being used.
        /// </summary>
        public Texture2D Skin;
        /// <summary>
        /// List of locations that make up the skin.
        /// </summary>
        public List<GUIRect> Rects = new List<GUIRect>();
        /// <summary>
        /// Set to true if a custom texture is being used.
        /// </summary>
        public bool UseCustomSkin = false;
    }

    /// <summary>
    /// Class allows for default control skins to be accessed, and can be used
    /// to populate skins in a SkinnedComponent.
    /// </summary>
    public class DefaultSingleSkin : ISingleSkin
    {
        #region Fields
        private Rectangle skinLocation;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="skinLocation">Default location on the source texture.</param>
        public DefaultSingleSkin(Rectangle skinLocation)
        {
            this.skinLocation = skinLocation;
        }
        #endregion

        #region ISingleSkin Implementation
        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle SkinLocation
        {
            get { return this.skinLocation; }
            set { this.skinLocation = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class allows for default control skins to be accessed, and can be used
    /// to populate skins in a SkinnedComponent.
    /// </summary>
    public class DefaultThreeSkins : DefaultSingleSkin, IThreeSkins
    {
        #region Fields
        private Rectangle hoverSkinLocation;
        private Rectangle pressedSkinLocation;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="skinLocation">Default location on the source texture.</param>
        /// <param name="hoverSkinLocation">Default location on the source texture.</param>
        /// <param name="pressedSkinLocation">Default location on the source texture.</param>
        public DefaultThreeSkins(
            Rectangle skinLocation,
            Rectangle hoverSkinLocation,
            Rectangle pressedSkinLocation
            )
            : base(skinLocation)
        {
            this.hoverSkinLocation = hoverSkinLocation;
            this.pressedSkinLocation = pressedSkinLocation;
        }
        #endregion

        #region IThreeSkins Implementation
        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin, for the hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle HoverSkinLocation
        {
            get { return this.hoverSkinLocation; }
            set { this.hoverSkinLocation = value; }
        }

        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin, for the pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle PressedSkinLocation
        {
            get { return this.pressedSkinLocation; }
            set { this.pressedSkinLocation = value; }
        }
        #endregion
    }

    /// <summary>
    /// Class allows for default control skins to be accessed, and can be used
    /// to populate skins in a SkinnedComponent.
    /// </summary>
    public class DefaultSixSkins : DefaultThreeSkins, ISixSkins
    {
        #region Fields
        private Rectangle checkedSkinLocation;
        private Rectangle checkedHoverSkinLocation;
        private Rectangle checkedPressedSkinLocation;
        #endregion

        #region Constructor
        public DefaultSixSkins(
            Rectangle skinLocation,
            Rectangle hoverSkinLocation,
            Rectangle pressedSkinLocation,
            Rectangle checkedSkinLocation,
            Rectangle checkedHoverSkinLocation,
            Rectangle checkedPressedSkinLocation
            )
            : base(skinLocation, hoverSkinLocation, pressedSkinLocation)
        {
            this.checkedSkinLocation = checkedSkinLocation;
            this.checkedHoverSkinLocation = checkedHoverSkinLocation;
            this.checkedPressedSkinLocation = checkedPressedSkinLocation;
        }
        #endregion

        #region ISixSkins Implementation
        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin, for the checked state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedSkinLocation
        {
            get { return this.checkedSkinLocation; }
            set { this.checkedSkinLocation = value; }
        }

        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin, for the checked hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedHoverSkinLocation
        {
            get { return this.checkedHoverSkinLocation; }
            set { this.checkedHoverSkinLocation = value; }
        }

        /// <summary>
        /// Sets the default location on the source texture of the control
        /// skin, for the checked pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedPressedSkinLocation
        {
            get { return this.checkedPressedSkinLocation; }
            set { this.checkedPressedSkinLocation = value; }
        }
        #endregion
    }

    /// <summary>
    /// Control with one or more skin states, allowing any component with a
    /// graphical skin to inherit the functionality of this class.
    /// </summary>
    /// <remarks>
    /// Drawable controls can inherit from this class to provide multiple skins
    /// representing different states. An example would be a button with
    /// regular, hover, and pressed states.
    /// </remarks>
    public class SkinnedComponent : UIComponent, ISixSkins
    {
        #region Fields
        private Dictionary<int, Rectangle> locations;
        private Dictionary<int, ComponentSkin> skins;
        private int currentSkin;
        #endregion

        #region Properties
        /// <summary>
        /// Gets all current skins.
        /// </summary>
        /// <remarks>
        /// Intended to be used by inherited classes to update the skins.
        /// </remarks>
        protected Dictionary<int, ComponentSkin> Skins
        {
            get { return this.skins; }
        }

        /// <summary>
        /// Get/Set the key of the currently active skin.
        /// </summary>
        public int CurrentSkin
        {
            get { return this.currentSkin; }
            set { SetActiveSkin(value); }
        }

        /// <summary>
        /// Get/Set the skin state of the currently active skin.
        /// </summary>
        /// <remarks>
        /// Same as CurrentSkin property, except SkinState enum is used for
        /// ease of use.
        /// </remarks>
        public SkinState CurrentSkinState
        {
            get { return (SkinState)this.currentSkin; }
            set { SetActiveSkin((int)value); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        public SkinnedComponent(Game game, GUIManager guiManager)
            : base(game, guiManager)
        {
            this.locations = new Dictionary<int, Rectangle>();
            this.skins = new Dictionary<int, ComponentSkin>();
            this.currentSkin = -1;
        }
        #endregion

        #region Setting Skins From Defaults
        /// <summary>
        /// Populates control skin locations from a DefaultSingleSkin.
        /// </summary>
        /// <param name="defaults">Default skin location.</param>
        public void SetSkinsFromDefaults(DefaultSingleSkin defaults)
        {
            SkinLocation = defaults.SkinLocation;
        }

        /// <summary>
        /// Populates control skin locations from a DefaultThreeSkins.
        /// </summary>
        /// <param name="defaults">Default skin locations.</param>
        public void SetSkinsFromDefaults(DefaultThreeSkins defaults)
        {
            SkinLocation = defaults.SkinLocation;
            HoverSkinLocation = defaults.HoverSkinLocation;
            PressedSkinLocation = defaults.PressedSkinLocation;
        }

        /// <summary>
        /// Populates control skin locations from a DefaultSixSkins.
        /// </summary>
        /// <param name="defaults">Default skin locations.</param>
        public void SetSkinsFromDefaults(DefaultSixSkins defaults)
        {
            SkinLocation = defaults.SkinLocation;
            HoverSkinLocation = defaults.HoverSkinLocation;
            PressedSkinLocation = defaults.PressedSkinLocation;
            CheckedSkinLocation = defaults.CheckedSkinLocation;
            CheckedHoverSkinLocation = defaults.CheckedHoverSkinLocation;
            CheckedPressedSkinLocation = defaults.CheckedPressedSkinLocation;
        }
        #endregion

        /// <summary>
        /// Retrieves location on the source texture of the specified control
        /// skin.
        /// </summary>
        /// <param name="index">Skin index.</param>
        /// <returns>
        /// Location on the source texture of skin if index is valid, otherwise
        /// an empty Rectangle.
        /// </returns>
        protected Rectangle GetSkinLocation(int index)
        {
            if (this.locations.ContainsKey(index))
                return this.locations[index];

            return Rectangle.Empty;
        }

        /// <summary>
        /// Sets the location on the source texture of the specified control
        /// skin.
        /// </summary>
        /// <remarks>
        /// Using SkinLocation properties does the same thing without
        /// the skin index to be specified, making them easier to use, unless
        /// custom number of skins is required.
        /// 
        /// Expects a valid Rectangle location, with width and height greater
        /// than 0.
        /// </remarks>
        /// <param name="index">Skin index.</param>
        /// <param name="location">Location on the source texture of skin.</param>
        public void SetSkinLocation(int index, Rectangle location)
        {
            Debug.Assert(CheckSkinLocation(location));

            // Change value if already added, otherwise just add
            if (this.locations.ContainsKey(index))
                this.locations[index] = location;
            else
            {
                this.locations.Add(index, location);
                this.skins.Add(index, new ComponentSkin());

                // Latest skin location needs to be refreshed
                RefreshSkins();

                // If this is the first entry, set it to active
                if (this.currentSkin == -1)
                    SetActiveSkin(index);
            }
        }

        /// <summary>
        /// Retrives the specified skin.
        /// </summary>
        /// <param name="index">Skin index.</param>
        /// <returns>Specified skin if index is valid, otherwise null.</returns>
        protected ComponentSkin GetSkin(int index)
        {
            if (this.skins.ContainsKey(index))
                return this.skins[index];

            return null;
        }

        /// <summary>
        /// Sets the currently active skin, and flags a redraw.
        /// </summary>
        /// <remarks>
        /// Does nothing if no skin has been set for the specified index, or
        /// clears current skin if -1.
        /// </remarks>
        /// <param name="index">Skin index.</param>
        private void SetActiveSkin(int index)
        {
            // Minimize redraws
            if (index != this.currentSkin)
            {
                ComponentSkin skin = null;
                
                if (index != -1)
                    skin = GetSkin(index);

                if (skin != null || index == -1)
                {
                    this.currentSkin = index;
                    Redraw();
                }
            }
        }

        /// <summary>
        /// Currently does nothing. Override to customize how skins are
        /// refreshed.
        /// </summary>
        protected virtual void RefreshSkins()
        {
        }

        /// <summary>
        /// Draws all skins associated with control, after clipping them to the
        /// parent control area.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with.</param>
        /// <param name="parentScissor">The scissor region of the parent control.</param>
        protected override void DrawControl(SpriteBatch spriteBatch, Rectangle parentScissor)
        {
            ComponentSkin skin = GetSkin(this.currentSkin);

            if (skin != null)
            {
                Texture2D texture;

                // Should the default GUI skin be used?
                if (skin.UseCustomSkin)
                    texture = skin.Skin;
                else
                    texture = GUIManager.SkinTexture;

                bool draw;
                Rectangle source;
                Rectangle destination;
                int dif;

                foreach (GUIRect rect in skin.Rects)
                {
                    draw = true;

                    source = rect.Source;

                    // Convert to absolute position
                    destination = rect.Destination;
                    destination.X += AbsolutePosition.X;
                    destination.Y += AbsolutePosition.Y;

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
                            texture,
                            destination,
                            source,
                            Color.White
                            );
                    }
                }
            }
        }

        #region Event Handlers
        /// <summary>
        /// Refresh the skins whenever control is resized.
        /// </summary>
        /// <param name="sender">Resized control.</param>
        protected override void OnResize(UIComponent sender)
        {
            base.OnResize(sender);
            RefreshSkins();
        }
        #endregion

        #region ISingleSkin Implementation
        /// <summary>
        /// Sets the location on the source texture of the control skin.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle SkinLocation
        {
            set { SetSkinLocation(0, value); }
        }
        #endregion

        #region IThreeSkins Implementation
        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle HoverSkinLocation
        {
            set { SetSkinLocation(1, value); }
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle PressedSkinLocation
        {
            set { SetSkinLocation(2, value); }
        }
        #endregion

        #region ISixSkins Implementation
        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedSkinLocation
        {
            set { SetSkinLocation(3, value); }
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked hover state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedHoverSkinLocation
        {
            set { SetSkinLocation(4, value); }
        }

        /// <summary>
        /// Sets the location on the source texture of the control skin, for
        /// the checked pressed state.
        /// </summary>
        /// <value>Width and Height values must be greater than 0.</value>
        public Rectangle CheckedPressedSkinLocation
        {
            set { SetSkinLocation(5, value); }
        }
        #endregion
    }
}