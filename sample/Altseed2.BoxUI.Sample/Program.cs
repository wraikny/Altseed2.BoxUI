﻿using System;

namespace Altseed2.BoxUI.Sample
{
    static class Params
    {
        public static readonly Color BackgroundColor = new Color(180, 180, 220);
        public static readonly Color DefaultColor = new Color(200, 200, 200);
        public static readonly Color HoverColor = new Color(180, 180, 180);
        public static readonly Color HoldColor = new Color(150, 150, 150);
        public static readonly Color TextColor = new Color(0, 0, 0);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration();
            config.ConsoleLoggingEnabled = true;
            config.IsResizable = true;
            if (!Engine.Initialize("Altseed2.BoxUI.Test", 800, 600, config))
            {
                return;
            }

            Engine.AddNode(new WindowsSample());

            while(Engine.DoEvents())
            {

                // Update BoxUISystem before Engine.Update
                BoxUISystem.Update();
                Engine.Update();
            }

            // Terminate BoxUISystem before Engine.Terminate
            BoxUISystem.Termiante();
            Engine.Terminate();
        }
    }
}
