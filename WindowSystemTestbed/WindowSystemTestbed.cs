using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using WindowSystem;
using InputEventSystem;

namespace WindowSystemTestbed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class WindowSystemTestbed : Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        SpriteBatch spriteBatch;

        InputEvents input;
        GUIManager gui;
        SpriteFont myFont;
        int windowNumber = 0;
        Skin skin;

        int before = 0;

        public WindowSystemTestbed()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 800;
            //graphics.PreferMultiSampling = false;
            //graphics.IsFullScreen = true;

            input = new InputEvents(this);
            this.Components.Add(input);

            gui = new GUIManager(this);
            this.Components.Add(gui);

            input.KeyDown += new KeyDownHandler(KeyDownFunction);

            this.IsFixedTimeStep = false;
        }

        void KeyDownFunction(KeyEventArgs args)
        {
            if (args.Key == Keys.Enter)
            {
                Window window = new Window(this, gui);
                window.Width = 320;
                window.Height = 380;
                window.Close += new CloseHandler(WindowCloseFunction);

                string text = "Test Window " + windowNumber;
                window.TitleText = text;
                windowNumber++;

                ComboBox comboBox = new ComboBox(this, gui);
                comboBox.IsEditable = true;
                comboBox.ZOrder = 1.0f;
                comboBox.X = 20;
                comboBox.Y = 54;

                for (int i = 0; i < 3; i++)
                    comboBox.AddEntry("Test");

                window.Add(comboBox);

                RadioButton radio1 = new RadioButton(this, gui);
                radio1.X = 0;
                radio1.Y = 0;
                radio1.Text = "Radio Test 1";

                RadioButton radio2 = new RadioButton(this, gui);
                radio2.X = 0;
                radio2.Y = 16;
                radio2.Text = "Radio Test 2";

                RadioButton radio3 = new RadioButton(this, gui);
                radio3.X = 0;
                radio3.Y = 32;
                radio3.Text = "Radio Test 3";

                RadioGroup group = new RadioGroup(this, gui);
                group.X = 20;
                group.Y = 89;
                group.Width = 200;
                group.Height = 48;
                group.ZOrder = 1.0f;
                group.Add(radio1);
                group.Add(radio2);
                group.Add(radio3);

                window.Add(group);

                ListBox listBox = new ListBox(this, gui);
                listBox.X = 20;
                listBox.Y = 144;
                listBox.ZOrder = 1.0f;

                for (int i = 0; i < 15; i++)
                    listBox.AddEntry("List Box Test " + (i + 1));

                window.Add(listBox);

                MenuBar menuBar = new MenuBar(this, gui);
                MenuButton item1 = new MenuButton(this, gui);
                item1.Text = "File";
                menuBar.Add(item1);
                MenuButton item2 = new MenuButton(this, gui);
                item2.Text = "Edit";
                item2.IsEnabled = false;
                item2.IsEnabled = true;
                menuBar.Add(item2);

                MenuButton fileItem1 = new MenuButton(this, gui);
                fileItem1.Text = "New";
                fileItem1.IconSource = new Rectangle(1, 189, 14, 14);
                fileItem1.IsEnabled = false;
                item1.Add(fileItem1);
                MenuSeparator fileItem2 = new MenuSeparator(this, gui);
                item1.Add(fileItem2);
                MenuButton fileItem3 = new MenuButton(this, gui);
                fileItem3.Text = "Close";
                fileItem3.IconSource = new Rectangle(16, 189, 15, 13);
                item1.Add(fileItem3);

                MenuButton item4 = new MenuButton(this, gui);
                item4.Text = "Community";
                item4.IsEnabled = true;
                item2.Add(item4);
                MenuSeparator sep2 = new MenuSeparator(this, gui);
                item2.Add(sep2);
                MenuButton item5 = new MenuButton(this, gui);
                item5.Text = "Next Test";
                item4.Add(item5);
                MenuButton item6 = new MenuButton(this, gui);
                item6.Text = "The Next Level!";
                item6.ShowMarginImage = false;
                item2.Add(item6);
                MenuButton item7 = new MenuButton(this, gui);
                item7.Text = "Booyeah ;-)";
                item6.Add(item7);
                window.Add(menuBar);

                gui.Add(window);

                MessageBox dialog = new MessageBox(this, gui, "Message box asking user a question.", "Message Box", MessageBoxButtons.Yes_No_Cancel, MessageBoxType.Question);
                dialog.Show(false);

                if (before == 1)
                    gui.ApplySkin(skin, true, true);

                before++;
            }
            else if (args.Key == Keys.A)
            {
                SkinnedComponent skin = new SkinnedComponent(this, gui);
            }
        }

        void WindowCloseFunction(object sender)
        {
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //this.IsMouseVisible = true;

            this.Window.Title = "Window System Testbed";

            this.spriteBatch = new SpriteBatch(this.graphics.GraphicsDevice);

            base.Initialize();
        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        protected override void LoadContent()
        {
            myFont = content.Load<SpriteFont>("Content/Fonts/Verdana");
            skin = content.Load<Skin>("Content/DefaultSkin");

            base.LoadContent();
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
        }

        float fps = 0.0f;
        int intFPS = 0;
        float deltaFPSTime = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // The time since Update was called last
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            fps = 1 / elapsed;
            deltaFPSTime += elapsed;
            if (deltaFPSTime > 1)
            {
                deltaFPSTime -= 1;
                intFPS = (int)fps;
            }

            // Allows the default game to exit on Xbox 360 and Windows
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Display frames per second
            string text = "FPS: " + intFPS.ToString();

            this.spriteBatch.Begin();

            this.spriteBatch.DrawString(myFont, text, new Vector2(50, 50), Color.Black);

            this.spriteBatch.End();

            //text = "UIComponent Count: " + UIComponent.InstanceCount.ToString();
            //myFont.DrawString(50, 50 + myFont.LineHeight, Color.Black, text);
            //text = "DrawableUIComponent Count: " + DrawableUIComponent.InstanceCount.ToString();
            //myFont.DrawString(50, 50 + (myFont.LineHeight * 2), Color.Black, text);

            base.Draw(gameTime);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game game;

            // Change commenting to run tutorials
            game = new WindowSystemTestbed();
            //game = new Tutorial1();
            //game = new Tutorial2();

            game.Run();
        }
    }
}