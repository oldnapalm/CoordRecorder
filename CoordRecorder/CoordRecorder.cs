using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.UI;

namespace CoordinateRecorder
{
    public class CoordRecorder : Script
    {
        const int PANEL_WIDTH = 340;
        const int PANEL_HEIGHT = 20;
        Color backColor = Color.FromArgb(100, 255, 255, 255);
        Color textColor = Color.Black;

        ContainerElement container;
        TextElement text;
        Keys enableKey;
        Keys saveKey;
        bool enable;

        Vector3 pos;
        float heading;

        public CoordRecorder()
        {
            LoadSettings();
            this.Tick += OnTick;
            this.KeyDown += OnKeyDown;
        }

        void OnTick(object sender, EventArgs e)
        {
            if (enable)
            {
                Player player = Game.Player;
                if (player != null && player.CanControlCharacter && player.IsAlive && player.Character != null)
                {
                    pos = player.Character.Position;
                    heading = player.Character.Heading;
                    text.Caption = String.Format("x:{0} y:{1} z:{2} angle:{3}", pos.X.ToString("0.000"),
                        pos.Y.ToString("0.000"), pos.Z.ToString("0.000"), heading.ToString("0.000"));
                    container.Draw();
                }
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == enableKey)
            {
                if (e.Modifiers == Keys.Shift)
                    Teleport();
                else
                    enable = !enable;
            }
            if (enable && e.KeyCode == saveKey)
            {
                if (e.Modifiers == (Keys.Control | Keys.Shift))
                    WriteToFile(10, false);
                else if (e.Modifiers == Keys.Control)
                    WriteToFile(10, true);
                else if (e.Modifiers == Keys.Shift)
                    WriteToFile(20, false);
                else
                    WriteToFile(20, true);
            }
        }

        void LoadSettings()
        {
            ScriptSettings settings = ScriptSettings.Load(@".\scripts\CoordRecorder.ini");
            this.enable = settings.GetValue<bool>("Core", "Enable", false);
            this.enableKey = (Keys)Enum.Parse(typeof(Keys), settings.GetValue<string>("Core", "EnableKey", "F9"), true);
            this.saveKey = (Keys)Enum.Parse(typeof(Keys), settings.GetValue<string>("Core", "SaveKey", "F10"), true);

            container = new ContainerElement(new Point((int)GTA.UI.Screen.Width / 2 - PANEL_WIDTH / 2, 0), new Size(PANEL_WIDTH, PANEL_HEIGHT), backColor);
            text = new TextElement("", new Point(PANEL_WIDTH / 2, 0), 0.42f, textColor, GTA.UI.Font.Pricedown, Alignment.Center);
            container.Items.Add(text);
        }

        void WriteToFile(float closeby, bool routed)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(@".\scripts\CoordRecorder_CSV.txt", true))
                {
                    string line = String.Format(CultureInfo.GetCultureInfo("en-US"), "{0},{1},{2},{3},{4},{5}", pos.X, pos.Y, pos.Z, heading, closeby, routed);
                    sw.WriteLine(line);
                }
                Notification.Show("Coords saved! " + text.Caption);
            }
            catch
            {
                Notification.Show("Failed to save coords!");
            }
        }

        private void Teleport()
        {
            Vector3 location = World.WaypointPosition;
            if (location == Vector3.Zero)
            {
                Notification.Show("Select teleport destination first!");
                return;
            }
            int i = 0;
            float groundHeight;
            do
            {
                Wait(50);
                groundHeight = World.GetGroundHeight(location);
                if (groundHeight == 0)
                    location.Z += 50;
                else
                    location.Z = groundHeight;
                if (Game.Player.Character.CurrentVehicle != null)
                    Game.Player.Character.CurrentVehicle.Position = location;
                else
                    Game.Player.Character.Position = location;
                i++;
            }
            while (groundHeight == 0 && i < 20);
        }
    }
}
