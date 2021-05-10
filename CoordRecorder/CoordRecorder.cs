using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.UI;

namespace CoordinateRecorder
{
    public class CoordRecorder : Script
    {
        const int PANEL_WIDTH = 360;
        const int PANEL_HEIGHT = 20;
        Color backColor = Color.FromArgb(100, 255, 255, 255);
        Color textColor = Color.Black;
        const string CSV_FILE = @".\scripts\CoordRecorder_CSV.txt";
        CultureInfo cInfo = CultureInfo.GetCultureInfo("en-US");

        ContainerElement container;
        TextElement text;
        Keys enableKey;
        Keys saveKey;
        bool enable;

        Vector3 pos;
        float heading;
        List<Checkpoint> cpList = new List<Checkpoint>();
        List<Blip> bList = new List<Blip>();
        int next;

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
                    text.Caption = String.Format("next:{0} x:{1} y:{2} z:{3} heading:{4}", next, pos.X.ToString("0.0"),
                                                 pos.Y.ToString("0.0"), pos.Z.ToString("0.0"), heading.ToString("0.0"));
                    container.Draw();
                }
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == enableKey)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    if (Game.IsWaypointActive)
                        Teleport(World.WaypointPosition);
                    else
                        Notification.Show("Select teleport destination");
                }
                else if (e.Modifiers == (Keys.Control | Keys.Shift))
                {
                    var last = GetLastCheckpoint();
                    if (last != Vector3.Zero)
                        Teleport(last);
                }
                else
                {
                    enable = !enable;
                    if (enable)
                    {
                        var last = GetLastCheckpoint();
                        if (last != Vector3.Zero)
                        {
                            Teleport(last);
                            var lines = File.ReadAllLines(CSV_FILE);
                            for (int i = 9; i >= 0; i--)
                            {
                                if (lines.Length > i)
                                {
                                    var line = lines[lines.Length - i - 1].Split(',');
                                    if (line.Length >= 3)
                                    {
                                        var cp = new Vector3(float.Parse(line[0], cInfo), float.Parse(line[1], cInfo), float.Parse(line[2], cInfo));
                                        AddCheckpoint(cp, lines.Length - i, !(line.Length >= 6 && line[5] == "False"));
                                    }
                                }
                            }
                            for (int i = 0; i < lines.Length; i++)
                            {
                                var line = lines[i].Split(',');
                                if (line.Length >= 3)
                                {
                                    var b = new Vector3(float.Parse(line[0], cInfo), float.Parse(line[1], cInfo), float.Parse(line[2], cInfo));
                                    AddBlip(b, i + 1, !(line.Length >= 6 && line[5] == "False"));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (Checkpoint cp in cpList)
                            cp.Delete();
                        cpList.Clear();
                        foreach (Blip b in bList)
                            b.Delete();
                        bList.Clear();
                    }
                }
            }
            if (enable)
            {
                if (e.KeyCode == saveKey)
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
                else if (e.KeyCode == Keys.Back && e.Modifiers == Keys.Shift)
                {
                    if (cpList.Count > 0)
                    {
                        cpList[cpList.Count - 1].Delete();
                        cpList.RemoveAt(cpList.Count - 1);
                    }
                    if (bList.Count > 0)
                    {
                        bList[bList.Count - 1].Delete();
                        bList.RemoveAt(bList.Count - 1);
                    }
                    if (File.Exists(CSV_FILE))
                    {
                        var lines = File.ReadAllLines(CSV_FILE);
                        if (lines.Length > 0)
                            File.WriteAllLines(CSV_FILE, lines.Take(lines.Length - 1).ToArray());
                    }
                    if (next > 1)
                    {
                        next--;
                        Notification.Show($"Coords {next} deleted");
                    }
                }
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
            var last = GetLastCheckpoint();
            if (World.GetDistance(last, pos) > 20f)
            {
                AddCheckpoint(pos, next, routed);
                if (cpList.Count > 10)
                {
                    cpList[0].Delete();
                    cpList.RemoveAt(0);
                }
                AddBlip(pos, next, routed);
                try
                {
                    using (StreamWriter sw = new StreamWriter(CSV_FILE, true))
                    {
                        string line = String.Format(cInfo, "{0},{1},{2},{3},{4},{5}", pos.X, pos.Y, pos.Z, heading, closeby, routed);
                        sw.WriteLine(line);
                    }
                    Notification.Show($"Coords {next} saved");
                    next++;
                }
                catch
                {
                    Notification.Show($"Failed to save coords {next}");
                }
            }
            else
                Notification.Show($"Too close to coords {next - 1}");
        }

        void Teleport(Vector3 location)
        {
            int i = 0;
            float groundHeight;
            location.Z = -50;
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

        Vector3 GetLastCheckpoint()
        {
            next = 1;
            if (File.Exists(CSV_FILE))
            {
                var lines = File.ReadAllLines(CSV_FILE);
                if (lines.Length > 0)
                {
                    next = lines.Length + 1;
                    var line = lines[lines.Length - 1].Split(',');
                    if (line.Length >= 3)
                        return new Vector3(float.Parse(line[0], cInfo), float.Parse(line[1], cInfo), float.Parse(line[2], cInfo));
                }
            }
            return Vector3.Zero;
        }

        void AddCheckpoint(Vector3 position, int number, bool routed)
        {
            while (number > 99)
                number -= 100;
            CheckpointCustomIcon icon = new CheckpointCustomIcon(CheckpointCustomIconStyle.Number, Convert.ToByte(number));
            Color cpColor;
            if (routed)
                cpColor = Color.GreenYellow;
            else
                cpColor = Color.OrangeRed;
            cpList.Add(World.CreateCheckpoint(icon, position, position, 10f, cpColor));
        }

        void AddBlip(Vector3 position, int number, bool routed)
        {
            while (number > 99)
                number -= 100;
            BlipColor bColor;
            if (routed)
                bColor = BlipColor.Yellow;
            else
                bColor = BlipColor.Red;
            bList.Add(World.CreateBlip(position));
            bList[bList.Count - 1].NumberLabel = number;
            bList[bList.Count - 1].Color = bColor;
        }
    }
}
