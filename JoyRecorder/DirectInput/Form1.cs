using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;               // for stopwatch
using System.Collections;               // for ArrayList
using System.IO;                        // for StreamWriter
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace DirectInput
{
    public partial class Form1 : Form
    {
        Device joystick;
        Stopwatch sw;
        bool isRecording = false;       // being recording input or not.
        ArrayList inputRecord;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //create joystick device.
            foreach (
                DeviceInstance di in
                Manager.GetDevices(
                    DeviceClass.GameControl,
                    EnumDevicesFlags.AttachedOnly))
            {
                joystick = new Device(di.InstanceGuid);
                break;
            }

            if (joystick == null)
            {
                //Throw exception if joystick not found.
                throw new Exception("No joystick found.");
            }

            //Set joystick axis ranges.
            foreach (DeviceObjectInstance doi in joystick.Objects)
            {
                if ((doi.ObjectId & (int)DeviceObjectTypeFlags.Axis) != 0)
                {
                    joystick.Properties.SetRange(
                        ParameterHow.ById,
                        doi.ObjectId,
                        new InputRange(1, 32767));
                }
            }

            //Set joystick axis mode absolute.
            joystick.Properties.AxisModeAbsolute = true;

            //set cooperative level.
            joystick.SetCooperativeLevel(
                this,
                CooperativeLevelFlags.NonExclusive |
                CooperativeLevelFlags.Background);

            //Acquire devices for capturing.
            joystick.Acquire();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Get
            JoystickState state = joystick.CurrentJoystickState;
            byte[] buttons = state.GetButtons();
            int[] pov = state.GetPointOfView();

// debug
//            if (isRecording)
//                Console.WriteLine(isRecording + "\t" + sw.ElapsedMilliseconds + "\t" + state.X + "\t" + state.Y + "\t" + state.Z + "\t" + state.Rz + "\t" + buttons[0] + "\t" + buttons[1] + "\t" + buttons[2] + "\t" + buttons[3] + "\t" + buttons[4] + "\t" + buttons[5] + "\t" + buttons[6] + "\t" + buttons[7]);
//            else
//                Console.WriteLine(isRecording + "\t" + "     \t" + state.X + "\t" + state.Y + "\t" + state.Z + "\t" + state.Rz + "\t" + buttons[0] + "\t" + buttons[1] + "\t" + buttons[2] + "\t" + buttons[3] + "\t" + buttons[4] + "\t" + buttons[5] + "\t" + buttons[6] + "\t" + buttons[7]);

            if (isRecording) inputRecord.Add(sw.ElapsedMilliseconds + "\t" + state.X + "\t" + state.Y + "\t" + state.Z + "\t" + state.Rz + "\t" + buttons[0] + "\t" + buttons[1] + "\t" + buttons[2] + "\t" + buttons[3] + "\t" + buttons[4] + "\t" + buttons[5] + "\t" + buttons[6] + "\t" + buttons[7]);

            //Output
            trackBar1.Value = state.X;
            trackBar2.Value = state.Y;
            trackBar3.Value = state.Z;
            trackBar4.Value = state.Rz;
            checkBox1.Checked = (buttons[0] > 0);
            checkBox2.Checked = (buttons[1] > 0);
            checkBox3.Checked = (buttons[2] > 0);
            checkBox4.Checked = (buttons[3] > 0);
            textBox1.Text = pov[0] / 100 + "°";
        }

        private void recButton_Click(object sender, EventArgs e)
        {
            // ui change
            recButton.Enabled = false;
            stopButton.Enabled = true;

            // for start recording 
            isRecording = true; 
            inputRecord = new ArrayList();

            //create stopwatch
            sw = new Stopwatch();
            sw.Start(); 
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            // ui change
            recButton.Enabled = true;
            stopButton.Enabled = false;

            // for stop recording 
            isRecording = false;
            sw.Stop();

            // write recorded input into file
            // ファイルにテキストを書き出し。
            using (StreamWriter w = new StreamWriter(@"script.txt"))
            {
                foreach (string rec in inputRecord)
                {
                    w.WriteLine(rec);
                }
            }

        }
    }
}