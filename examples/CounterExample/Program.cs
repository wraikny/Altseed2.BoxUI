using System;
using Altseed2;
using Altseed2.BoxUI;

namespace CounterExample
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine.Initialize("CounterExample", 800, 600);

            Engine.AddNode(new CounterNode());

            while (Engine.DoEvents())
            {
                // BoxUIの更新
                BoxUISystem.Update();
                Engine.Update();
            }

            // BoxUIの終了
            BoxUISystem.Terminate();
            Engine.Terminate();
        }
    }
}
