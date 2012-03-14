using System;
using System.Diagnostics;               // for stopwatch
using System.Collections;               // for ArrayList
using System.IO;                        // for StreamWriter
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AutoPlay
{
    class Program
    {
        [DllImport("LibJoy.dll")]
        extern static void initJoystick();
        [DllImport("LibJoy.dll")]
        extern static void closeJoystick();
        [DllImport("LibJoy.dll")]
        extern static void moveJoystick(int xaxis, int yaxis, int zrotate, int button1);

        static void Main(string[] args)
        {
            // open virtual joystick device
            initJoystick();

            // open script.txt file. save input data into arraylist
            StreamReader r = new StreamReader(@"script.txt");
            ArrayList script = new ArrayList();
            string line;
            while ((line = r.ReadLine()) != null)
            {
                script.Add(line);
            }

            // stopwatch start
            Stopwatch sw = new Stopwatch();
            sw.Start();

            foreach (string rec in script)
            {
                // split by tab
                string[] recArray = rec.Split('\t');

                // calc waiting time
                long elapsed = sw.ElapsedMilliseconds;
                int wait = int.Parse(recArray[0]) - (int)elapsed;
                Console.WriteLine("timecode\t" + recArray[0] + "\tElapsed time\t" + elapsed + "\twait\t" + wait);
                if (wait < 0) wait = 0;
                // wait
                System.Threading.Thread.Sleep(wait);

                // movejoystick
                moveJoystick(int.Parse(recArray[1]), int.Parse(recArray[2]), int.Parse(recArray[4]), int.Parse(recArray[5]));
            }

            // stopwatch stop
            sw.Stop();

            // close
            closeJoystick();
        }
    }
}
