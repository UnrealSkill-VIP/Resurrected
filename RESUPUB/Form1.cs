using RESUPUB.EngineObjects;
using RESUPUB.Off;
using RESUPUB.Static;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RESUPUB
{
    public partial class Form1 : Form
    {
        #region "allocConsole"
        [DllImport("kernel32.dll",
    EntryPoint = "GetStdHandle",
    SetLastError = true,
    CharSet = CharSet.Auto,
    CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;
        #endregion

        private TimeSpan _last = TimeSpan.Zero;


        Local localPlayer { get; set; }

        public Hashtable _players = new Hashtable();

        public int ClientBase;
        public int EngineBase;

        public bool makeshit = true;

        public Thread t3;


        bool started = false;
        public int MyId = 0;
        public string MyName = "";
        public int Myhealth = 0;
        public int MyArmor = 0;
        public WeaponType MyWeaponID = 0;
        public PlayerTeam MyTeam = PlayerTeam.Neutral;



        List<ListViewItem> thePlayers = new List<ListViewItem>();



        public Form1()
        {
            InitializeComponent();



            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);

            Console.WriteLine("This text you can see in console window.");
            //while()
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //cManager = new Manager("csgo");
            //Canvas myCanvas = new Canvas();
            //myCanvas.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            started = true;
            this.DoubleBuffered = true;
            timer1.Interval = 20000;
            timer1.Start();

            t3 = new Thread(() => updateMe());
            t3.Start();
            //smallUpdate();
        }
        void smallUpdate()
        {
            var proc = Process.GetProcessesByName("csgo")[0];
            Manager.Attach(proc);
            Manager.Objects.Update();
            if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
            {

                var me = Manager.Me;
                MyName = me.Name;
                MyId = me.Id;
                MyArmor = me.Armor;
                Myhealth = me.Health;
                MyTeam = me.Team;
                MyWeaponID = me.WeaponID;
                //MessageBox.Show(me.Health.ToString());
                // UpdateLabel(this.labelHealth, me.Health.ToString());
                foreach (var player in Manager.Objects.Players)
                {
                    //if(player.ModelName.Contains("bomb") || player.ModelName.Contains("C4") || player.ModelName.Contains("c4") || player.ModelName.Contains("PlantedC4"))
                    //{
                    ListViewItem myItem = new ListViewItem();
                    myItem.Text = player.Id.ToString();
                    myItem.SubItems.Add(player.Health.ToString());
                    myItem.SubItems.Add(player.Armor.ToString());
                    myItem.SubItems.Add(player.Team.ToString());
                    myItem.SubItems.Add(player.Distance.ToString());
                    myItem.SubItems.Add(player.IsAlive.ToString());
                    myItem.SubItems.Add(player.Position.ToString());
                    myItem.SubItems.Add(player.Name);
                    myItem.SubItems.Add(player.Head.ToString());
                    //myItem.SubItems.Add(((WeaponType)player.eWeaponIndex).ToString() + " - " + player.eWeaponIndex.ToString());
                    myItem.SubItems.Add(player.ClsId.ToString());
                    myItem.SubItems.Add(player.ModelName);
                    //}

                }
            }
        }
        void updateMe()
        {
            var proc = Process.GetProcessesByName("csgo")[0];
            Manager.Attach(proc);
            while (started)
            {
                Manager.Objects.Update();
                if (Manager.gClient.InGame && Manager.Me != null && Manager.Me.IsValid)
                {
                    
                    var me = Manager.Me;
                    MyName = me.Name;
                    MyId = me.Id;
                    MyArmor = me.Armor;
                    Myhealth = me.Health;
                    MyTeam = me.Team;
                    MyWeaponID = me.WeaponID;
                    //MessageBox.Show(me.Health.ToString());
                    // UpdateLabel(this.labelHealth, me.Health.ToString());
                    thePlayers.Clear();
                    foreach(var player in Manager.Objects.Players)
                    {
                        //if(player.ModelName.Contains("bomb") || player.ModelName.Contains("C4") || player.ModelName.Contains("c4") || player.ModelName.Contains("PlantedC4"))
                        //{
                            ListViewItem myItem = new ListViewItem();
                            myItem.Text = player.Id.ToString();
                            myItem.SubItems.Add(player.Health.ToString());
                            myItem.SubItems.Add(player.Armor.ToString());
                            myItem.SubItems.Add(player.Team.ToString());
                            myItem.SubItems.Add(player.Distance.ToString());
                            myItem.SubItems.Add(player.IsAlive.ToString());
                            myItem.SubItems.Add(player.Position.ToString());
                            myItem.SubItems.Add(player.Name);
                            myItem.SubItems.Add(player.Head.ToString());
                        //myItem.SubItems.Add(((WeaponType)player.eWeaponIndex).ToString() + " - " + player.eWeaponIndex.ToString());
                            myItem.SubItems.Add(player.ClsId.ToString());
                            myItem.SubItems.Add(player.ModelName);
                            thePlayers.Add(myItem);
                        //}

                    }
                    Thread.Sleep(100);
                }

            }
        }

        Entity GetById(int i)
        {
            if (_players.Count < i)
                return null;
            return (Entity)_players[i];
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            labelId.Text = "Id:" + MyId.ToString();
            labelName.Text = "Name: " + MyName;
            labelHealth.Text = "HP: " + Myhealth.ToString();
            labelArmor.Text = "Kevlar :" + MyArmor.ToString();
            labelTeam.Text = "Team: " + MyTeam.ToString();       
            labelPos.Text = "WPID: " + (MyWeaponID).ToString();


            List<ListViewItem> another = new List<ListViewItem>(thePlayers);
            listView1.Items.Clear();
            foreach (ListViewItem item in another)
                listView1.Items.Add(item);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            labelId.Text = "Id:" + MyId.ToString();
            labelName.Text = "Name: " + MyName;
            labelHealth.Text = "HP: " + Myhealth.ToString();
            labelArmor.Text = "Kevlar :" + MyArmor.ToString();
            labelTeam.Text = "Team: " + MyTeam.ToString();
            labelPos.Text = "WPID: " + (MyWeaponID).ToString();


            List<ListViewItem> another = new List<ListViewItem>(thePlayers);
            listView1.Items.Clear();
            foreach (ListViewItem item in another)
                listView1.Items.Add(item);
        }
    }

}
