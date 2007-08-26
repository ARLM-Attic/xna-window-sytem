#region File Description
//-----------------------------------------------------------------------------
// File:      MessageBox.cs
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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InputEventSystem;
#endregion

namespace WindowSystem
{
    /// <summary>
    /// The type type of information being displayed. An icon representing type
    /// is shown.
    /// </summary>
    public enum MessageBoxType
    {
        None,
        Info,
        Error,
        Warning,
        Question
    }

    /// <summary>
    /// Type of buttons to appear on message box.
    /// </summary>
    public enum MessageBoxButtons
    {
        OK,
        OK_Cancel,
        Yes_No,
        Yes_No_Cancel
    }

    /// <summary>
    /// A popup message box, that displays a message to the user, and waits for
    /// a reply. Message boxes can contain an OK button, or an OK and Cancel
    /// button.
    /// </summary>
    public class MessageBox : Dialog
    {
        #region DefaultProperties
        private static DefaultSingleSkin defaultInfoSkin = new DefaultSingleSkin(
            new Rectangle(1, 91, 25, 25)
            );
        private static DefaultSingleSkin defaultErrorSkin = new DefaultSingleSkin(
            new Rectangle(27, 91, 25, 25)
            );
        private static DefaultSingleSkin defaultWarningSkin = new DefaultSingleSkin(
            new Rectangle(53, 91, 25, 25)
            );
        private static DefaultSingleSkin defaultQuestionSkin = new DefaultSingleSkin(
            new Rectangle(79, 91, 25, 25)
            );

        /// <summary>
        /// Gets the default skin interface.
        /// </summary>
        public static ISingleSkin DefaultInfoSkin
        {
            get { return defaultInfoSkin; }
        }

        /// <summary>
        /// Gets the default skin interface.
        /// </summary>
        public static ISingleSkin DefaultErrorSkin
        {
            get { return defaultErrorSkin; }
        }

        /// <summary>
        /// Gets the default skin interface.
        /// </summary>
        public static ISingleSkin DefaultWarningSkin
        {
            get { return defaultWarningSkin; }
        }

        /// <summary>
        /// Gets the default skin interface.
        /// </summary>
        public static ISingleSkin DefaultQuestionSkin
        {
            get { return defaultQuestionSkin; }
        }
        #endregion

        #region Fields
        private const int LargeSeperation = 10;
        private const int SmallSeperation = 5;
        private Icon icon;
        private Label message;
        private MessageBoxButtons buttons;
        private List<TextButton> buttonList;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="game">The currently running Game object.</param>
        /// <param name="guiManager">GUIManager that this control is part of.</param>
        /// <param name="message">Message to display.</param>
        /// <param name="title">Window title.</param>
        /// <param name="buttons">Tyoe of buttons to display.</param>
        /// <param name="type">Type of icon to use.</param>
        public MessageBox(
            Game game,
            GUIManager guiManager,
            string message,
            string title,
            MessageBoxButtons buttons,
            MessageBoxType type
            )
            : base(game, guiManager)
        {
            this.buttonList = new List<TextButton>();

            #region Create Child Controls
            this.icon = new Icon(game, guiManager);
            this.message = new Label(game, guiManager);
            #endregion

            this.buttons = buttons;

            if (this.buttons == MessageBoxButtons.OK || this.buttons == MessageBoxButtons.Yes_No)
                HasCloseButton = false;

            bool showIcon = true;
            // Check which icon to display
            switch (type)
            {
                case MessageBoxType.Info:
                    this.icon.SetSkinsFromDefaults(defaultInfoSkin);
                    break;
                case MessageBoxType.Error:
                    this.icon.SetSkinsFromDefaults(defaultErrorSkin);
                    break;
                case MessageBoxType.Warning:
                    this.icon.SetSkinsFromDefaults(defaultWarningSkin);
                    break;
                case MessageBoxType.Question:
                    this.icon.SetSkinsFromDefaults(defaultQuestionSkin);
                    break;
                default:
                    showIcon = false;
                    break;
            }

            if (showIcon)
            {
                Add(this.icon);

                this.icon.X = LargeSeperation;
                this.icon.Y = LargeSeperation;
                this.icon.ResizeToFit();
                this.icon.CanHaveFocus = false;
                this.message.X = this.icon.X + this.icon.Width + LargeSeperation;
            }
            else
                this.message.X = LargeSeperation;

            Add(this.message);
            this.message.Y = LargeSeperation;
            this.message.Text = message;
            this.message.Width = this.message.TextWidth;
            this.message.Height = this.message.TextHeight;

            this.TitleText = title;
            this.Resizable = false;
            this.ClientWidth = this.message.X + this.message.Width + LargeSeperation;

            int numButtons = 0;

            if (this.buttons == MessageBoxButtons.OK)
                numButtons = 1;
            else if (this.buttons == MessageBoxButtons.OK_Cancel || this.buttons == MessageBoxButtons.Yes_No)
                numButtons = 2;
            else if (this.buttons == MessageBoxButtons.Yes_No_Cancel)
                numButtons = 3;

            int width = 0;

            for (int i = 0; i < numButtons; i++)
            {
                TextButton newButton = new TextButton(game, guiManager);
                this.buttonList.Add(newButton);
                Add(newButton);

                if (i == 0)
                {
                    if (this.buttons == MessageBoxButtons.OK || this.buttons == MessageBoxButtons.OK_Cancel)
                        newButton.Text = "OK";
                    else
                        newButton.Text = "Yes";
                }
                else if (i == 1)
                {
                    if (this.buttons == MessageBoxButtons.OK_Cancel)
                        newButton.Text = "Cancel";
                    else
                        newButton.Text = "No";
                }
                else
                    newButton.Text = "Cancel";

                newButton.Y = this.message.Y + this.message.Height + (LargeSeperation * 2);
                newButton.Click += new ClickHandler(OnClick);

                width += newButton.Width;
                if (i != numButtons - 1)
                    width += SmallSeperation;
            }

            // See if client needs to be enlarged to hold buttons
            if (ClientWidth < width + (2 * LargeSeperation))
                ClientWidth = width + (2 * LargeSeperation);

            int lastX = (ClientWidth - width) / 2;

            foreach (TextButton button in this.buttonList)
            {
                button.X = lastX;
                lastX += button.Width + SmallSeperation;
            }

            // Assumes message box will always have at least one button (which
            // is currently the case.
            this.ClientHeight = this.buttonList[0].Y + this.buttonList[0].Height + LargeSeperation;

            // Centre message box on the screen
            CenterWindow();
        }
        #endregion

        /// <summary>
        /// Ensure controls that haven't been added are cleaned up.
        /// </summary>
        public override void CleanUp()
        {
            this.icon.CleanUp();

            base.CleanUp();
        }

        /// <summary>
        /// Close the message box when button is clicked.
        /// </summary>
        /// <param name="sender">Clicked control.</param>
        private void OnClick(UIComponent sender)
        {
            // Go through button list, check what result to return.
            // Note that cancel is the default result, so no need to look for
            // that.
            for (int i = 0; i < this.buttonList.Count; i++)
            {
                if (sender == this.buttonList[i])
                {
                    if (i == 0)
                    {
                        if (this.buttons == MessageBoxButtons.OK || this.buttons == MessageBoxButtons.OK_Cancel)
                            SetDialogResult(DialogResult.OK);
                        else
                            SetDialogResult(DialogResult.Yes);
                    }
                    else if (i == 1)
                    {
                        if (this.buttons != MessageBoxButtons.OK_Cancel)
                            SetDialogResult(DialogResult.No);
                    }

                    break;
                }
            }

            CloseWindow();
        }
    }
}