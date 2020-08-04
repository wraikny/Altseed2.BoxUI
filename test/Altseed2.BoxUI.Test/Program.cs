using System;

namespace Altseed2.BoxUI.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration();
            config.ConsoleLoggingEnabled = true;
            //config.IsResizable = true;
            if(!Engine.Initialize("Altseed2.BoxUI.Test", 800, 600, config))
            {
                return;
            }

            Engine.AddNode(new Counter());

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
