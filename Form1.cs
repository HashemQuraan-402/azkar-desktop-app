using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using اذكاري.Models;
using اذكاري.Services;
using اذكاري.Properties;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace اذكاري
{
    public partial class frmMain : Form
    {
        public static DateTime timeNow = DateTime.Now;
        public Azkar azkar;
        public AzkarServices azkarServices;
        public ZekerService zekerService;
        public List<Zeker> CompleteZekerList;
        public Button clickedbutton;
        ProgressBar progressBar;
        public List<Button> btnsList;
        public List<Button> ResetList;
        public int tabIndex;
        public List<Control> NavigationList;
        
        public List<Masbaha> MasbahaList;

        DataGridView dataGridForAzkar = new DataGridView();
        DataGridView dataGridForMasbaha = new DataGridView();


        public frmMain()
        {
            InitializeComponent();
            azkar = new Azkar();
            azkarServices = new AzkarServices();
            zekerService = new ZekerService();
            CompleteZekerList = new List<Zeker>();
            clickedbutton = null;
            btnsList = new List<Button>();
            ResetList = new List<Button>();
            NavigationList = new List<Control>();
            
            MasbahaList = new List<Masbaha>();
        }

     
        private void makeTheAzkar() {
            int timeNow;
            int.TryParse(DateTime.Now.ToString("HH"), out timeNow);
            if (timeNow >= 07 && timeNow <= 15)
            {
                azkar.DayState = DayState.الصباح;
                azkar.zekers = zekerService.getMorningZekers();
            }
            else {
                azkar.DayState = DayState.المساء;
                azkar.zekers = zekerService.getEvningZekers();
            }
        }
        List<Panel>panels = new List<Panel>();
        Label lblAzkarState = new Label();
        private void makeAzkarStructure() {

            // Head Label
            
            lblAzkarState.Text = $"أذكار {azkar.DayState}";
            lblAzkarState.RightToLeft = RightToLeft.Yes;
            lblAzkarState.TextAlign = ContentAlignment.MiddleCenter;
            lblAzkarState.ForeColor = Color.Green;
            lblAzkarState.Font = new Font("Segoe UI" , 20 , FontStyle.Bold);
            lblAzkarState.Width = flowLayoutPanel1.Width - 20;
            lblAzkarState.TabStop = false;
            lblAzkarState.Height = lblAzkarState.GetPreferredSize(new Size(lblAzkarState.Width, 0)).Height;
            flowLayoutPanel1.Controls.Add(lblAzkarState);

            // zekder component

            foreach (Zeker z in azkar.zekers)
            {
                z.Counter = z.Number;
                
                
                //Panel for the Zeker
                Panel plZeker = new Panel();
                plZeker.Width = flowLayoutPanel1.Width - 20;
                plZeker.BackColor = Color.White;
                plZeker.BorderStyle = BorderStyle.FixedSingle;
                plZeker.TabStop = false;
                panels.Add(plZeker);
                //Label for zeker
                Label lblZeker = new Label();
                lblZeker.Text = z.Text;
                lblZeker.Font = new Font("Segoe UI", 14, FontStyle.Regular);
                //lblZeker.RightToLeft = RightToLeft.Yes;
                lblZeker.TextAlign = ContentAlignment.MiddleLeft;
                //lblZeker.MaximumSize = new Size(plZeker.Width - 140, 0);
                lblZeker.Width = plZeker.Width - 140;
                lblZeker.Height = lblZeker.GetPreferredSize(new Size(lblZeker.Width, 0)).Height;
                lblZeker.Location = new Point(10, 10);
                lblZeker.AutoSize = false;
                //button for counting
                Button btnZeker = new Button();
                btnZeker.Text = z.Number.ToString();
                btnZeker.Width = 120;
                btnZeker.Height = 70;
                btnZeker.BackColor = Color.Green;
                btnZeker.ForeColor = Color.White;
                btnZeker.Font = new Font("Segoe UI" , 14 , FontStyle.Bold);
                btnZeker.RightToLeft = RightToLeft.No;
                btnZeker.Location = new Point(plZeker.Width - btnZeker.Width - 10,10);
                btnZeker.Click += btnZeker_click;
                btnZeker.Tag = z;
                
                btnsList.Add(btnZeker);
                NavigationList.Add(btnZeker);
                // Event for foucs
                btnZeker.Enter += (s,e) => {
                    indexForFoucs = NavigationList.IndexOf((Button)s);
                    //selection(btnZeker);
                    btnZeker.BackColor = Color.Red;
                    btnZeker.ForeColor= Color.White;
                };
                btnZeker.Leave += (s, e) => {
                    btnZeker.BackColor= Color.Green;
                    btnZeker.ForeColor = Color.White;
                };
                // Event for press button
                
                //Button for Reset the Counter
                Button btnReset = new Button();
                btnReset.Width = btnZeker.Width;
                btnReset.Height = 30;
                btnReset.Location = new Point(btnZeker.Location.X ,btnZeker.Location.Y + btnZeker.Height + 10);
                btnReset.Text = "اعادة";
                btnReset.Font = new Font("Segoe UI", 12 , FontStyle.Bold);
                ResetList.Add(btnReset);
                NavigationList.Add(btnReset);
                btnReset.Tag = btnZeker;
                btnReset.Enter += (s, e) => {
                    indexForFoucs = NavigationList.IndexOf((Button)s);
                };
                btnReset.Click += (s, e) => {
                    
                    if (MessageBox.Show("هل تريد الاعادة ؟", "تأكيد", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                        Button rbtn = (Button)(btnReset.Tag);
                        Zeker rz = (Zeker)rbtn.Tag;
                        rz.Number = z.Counter;
                        if(rz.Complete)
                        {
                            CompleteZekerList.Remove(rz);
                            rz.Complete = false;
                            changeProgress(rz.Complete);
                        }
                        
                        rbtn.BackColor = Color.Green;
                        ((Button)(btnReset.Tag)).Enabled = true;
                        rbtn.Text = rz.Number.ToString();
                        
                        
                    }
                };


                

                // return to panel
                
                plZeker.Controls.Add(lblZeker);
                plZeker.Controls.Add(btnZeker);
                plZeker.Controls.Add(btnReset);
                plZeker.Height = Math.Max((lblZeker.Bottom+10),(btnReset.Bottom + 10));
                if ((lblZeker.Bottom + 10) > (btnReset.Bottom + 10))
                {
                    btnReset.Location = new Point(btnReset.Location.X, plZeker.Height - btnReset.Height - 10);
                    btnZeker.Height = plZeker.Height - btnReset.Height - 30;
                }
                else {
                    lblZeker.Height = plZeker.Height - 30;
                }
                flowLayoutPanel1.Controls.Add(plZeker);
                
                
            }
        }
        Label lblProgress;
        private void changeProgress(bool state) {
            if (state)
            {
                progressBar.Value++;
                if (progressBar.Value == progressBar.Maximum)
                {
                    notifyIcon1.Icon = SystemIcons.Information;
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipTitle = "اكتمل";
                    notifyIcon1.BalloonTipText = $"لقد انهيت اذكار {azkar.DayState}";
                    notifyIcon1.ShowBalloonTip(10000);
                }
            }
            else { 
                progressBar.Value--;
            }
            lblProgress.Text = string.Format(CultureInfo.InvariantCulture, "{1}/{0} : نسبة الاكتمال", progressBar.Value, progressBar.Maximum);
        }

        private void makeProgrees() { 

            //panel for progerss
            Panel plProgress = new Panel();
            plProgress.Width = flowLayoutPanel1.Width - 20;
            plProgress.BorderStyle = BorderStyle.FixedSingle;
            plProgress.TabStop = false;
            panels.Add(plProgress);
            //progress bar
            progressBar = new ProgressBar();
            progressBar.Minimum = 0;
            if (azkar.DayState == DayState.الصباح)
            {
                progressBar.Maximum = 31;
            }
            else {
                progressBar.Maximum = 32;
            }
            progressBar.Value = 0;
            progressBar.Width = plProgress.Width - 20;
            progressBar.Height = 30;
            progressBar.Style = ProgressBarStyle.Continuous;
            
            //Label for progressbar
            lblProgress = new Label();
            lblProgress.RightToLeft = RightToLeft.No;
            lblProgress.TextAlign = ContentAlignment.MiddleRight;
            lblProgress.Text = string.Format(CultureInfo.InvariantCulture,"{1}/{0} : نسبة الاكتمال",progressBar.Value,progressBar.Maximum);
            
            lblProgress.Font = new Font("Segoe UI",16,FontStyle.Bold);
            lblProgress.Width = plProgress.Width - 10;
            lblProgress.Height = lblProgress.GetPreferredSize(new Size(lblProgress.Width, 0)).Height;
            progressBar.Location = new Point(10,lblProgress.Bottom+5);
            plProgress.Controls.Add(lblProgress);
            plProgress.Controls.Add(progressBar);
            plProgress.Height = progressBar.Bottom + 10;
            flowLayoutPanel1.Controls.Add(plProgress);
        }
        
        bool enableCompleteButtonFirstTime = true;
        private void btnZeker_click(object sender , EventArgs e) {
            Button b = (Button)sender;
            Zeker z = (Zeker)b.Tag;
            if (z.Number > 0)
            {
                z.Number--;
                b.Text = z.Number.ToString();
                zekerService.detrmineCompletionForZeker(z);
                //selection(b);
                //if (b.BackColor == Color.Green)
                //{
                    
                //    if (clickedbutton == null)
                //    {
                        
                //        clickedbutton = b;

                //        b.BackColor = Color.Red;
                //        b.ForeColor = Color.White;
                //    }
                //    else
                //    {
                        
                //        b.BackColor = Color.Red;
                //        b.ForeColor = Color.White;
                //        if (clickedbutton != b && clickedbutton.Enabled) {
                //            clickedbutton.BackColor = Color.Green;
                //            clickedbutton.ForeColor = Color.White;
                //            clickedbutton = b;
                //        }
                        
                //    }
                //}
                if (z.Complete) {
                    CompleteZekerList.Add(z);
                    if (enableCompleteButtonFirstTime)
                    {
                        btnComplete.Enabled = true;
                        enableCompleteButtonFirstTime = false;
                    }
                    changeProgress(z.Complete);
                    b.Enabled = false;
                    b.BackColor = Color.Gray;
                }
            }
            
        }
        Button btnComplete = new Button();
        List<int>notComplete = new List<int>();
        private void makeCompleteButton() {
            //Button for completion
            btnComplete.Enabled = false;
            btnComplete.Font = new Font("Segoe UI" , 16 );
            btnComplete.Text = "اكتمل";
            btnComplete.BackColor = Color.Gray;
            btnComplete.ForeColor = Color.White;
            btnComplete.Width = 200;
            btnComplete.Height = 80;
            btnComplete.Margin = new Padding(300,20,0,10);
            
            NavigationList.Add(btnComplete);

            bool changeViaMouse = true;
            bool changeViakey = true;

            btnComplete.MouseEnter += (s, e) => {
                if (changeViaMouse)
                {
                    changeViakey = false;
                    btnComplete.BackColor = Color.Red;
                    btnComplete.Width += 5;
                    btnComplete.Height += 5;
                }
                
            };
            btnComplete.MouseLeave += (s, e) => {
                if (changeViaMouse)
                {
                    changeViakey = false;
                    btnComplete.BackColor = Color.Green;
                    btnComplete.Width -= 5;
                    btnComplete.Height -= 5;
                    changeViakey = true;
                }
                
            };


            btnComplete.Enter += (s,e) => {
                if (changeViakey)
                {
                    changeViaMouse = false;
                    btnComplete.BackColor = Color.Red;
                    btnComplete.Width += 5;
                    btnComplete.Height += 5;
                }
            };
            btnComplete.Leave += (s,e) => {
                if (changeViakey)
                {
                    changeViaMouse = false;
                    btnComplete.BackColor = Color.Green;
                    btnComplete.Width -= 5;
                    btnComplete.Height -= 5;
                    changeViaMouse = true;
                }
            };
            btnComplete.EnabledChanged += (s, e) => {
                if (btnComplete.Enabled)
                {
                    btnComplete.BackColor = Color.Green;
                    btnComplete.ForeColor = Color.White;
                }
                else {
                    btnComplete.BackColor = Color.Gray;
                }
            };
            // some pre propariters for complete button
            btnComplete.Click += (s, e) => {
                if (MessageBox.Show("هل تريد الانهاء","تأكيد",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK) {
                    azkarServices.detrmineTheCompletionOfAll(azkar);
                    notComplete = azkar.indexsForNotCompleteZekers;
                    azkar.indexsForNotCompleteZekers.Clear();
                    azkar.Complete = false;
                    azkar.Date = DateTime.Now.ToString("dd / MM / yyyy , hh:mm tt");
                    List<Zeker> completeZekers = new List<Zeker>();
                    foreach (Zeker zeker in azkar.zekers)
                    {
                        if (zeker.Complete)
                        {
                            completeZekers.Add(zeker);
                        }
                    }
                    //function call for data grid view
                    
                    fillZkereGridView();
                    lblNoData.Visible = false;
                    btnSave.Enabled = true;
                    btnDeleteAzkar.Enabled = true;
                    flowLayoutPanel2.Visible = true;
                    flowLayoutPanel2.Controls.Add(((Panel)(dataGridForAzkar.Tag)));
                    foreach (Button b in btnsList)
                    {
                        Zeker z = (Zeker)b.Tag;
                        
                        z.Number = z.Counter;
                        b.Text = z.Number.ToString();
                        z.Complete = false;
                        b.Enabled = true;
                        clickedbutton = null;
                        b.BackColor = Color.Green;
                        b.ForeColor = Color.White;

                       
                    }
                    progressBar.Value = 0;
                    lblProgress.Text = string.Format(CultureInfo.InvariantCulture, "{1}/{0} : نسبة الاكتمال", progressBar.Value, progressBar.Maximum);


                    btnComplete.Enabled = false;
                    enableCompleteButtonFirstTime = true;

                }
            };
            flowLayoutPanel1.Controls.Add(btnComplete);
        }

        //function for fill data grid view with zeker


        private void fillZkereGridView() {
            dataGridForAzkar.DataSource = CompleteZekerList;
            dataGridForAzkar.DataBindingComplete += (s,e) => {
                if (dataGridForAzkar.Columns.Contains("Number")) {
                    dataGridForAzkar.Columns["Number"].Visible = false;
                }
                if (dataGridForAzkar.Columns.Contains("Counter"))
                {
                    dataGridForAzkar.Columns["Counter"].Visible = false;
                }
                if (dataGridForAzkar.Columns.Contains("Complete")) {
                    dataGridForAzkar.Columns["Complete"].Visible = false;
                }
            };
            
            
            
        }

        private void makeTime() {
            lblHour.Text = DateTime.Now.ToString("hh:mm tt", new CultureInfo("en-US"));
            lblDay.Text = DateTime.Now.ToString("ddd");
            lblDate.Text = DateTime.Now.ToString("dd-MM-yyyy", new CultureInfo("en-US"));
            lblHigriDate.Text = DateTime.Now.ToString("yyyy-MMMM-dd");
            
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            
            makeTime();
            timer1.Start();
            makeTheAzkar();
            makeAzkarStructure();
            makeProgrees();
            makeCompleteButton();
            NavigationList.Add(tabControl1);
            flowLayoutPanel2.Visible = false;
            createAreaForDataGrid();


            panels.Add(panel1);
            panels.Add(panel2);
            panels.Add(panel3);
            panels.Add(panel4);
            panels.Add(panel5);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (NavigationList.Count > 0 || NavigationList != null) {
                indexForFoucs = 0;
                NavigationList[indexForFoucs].Focus();
            }
        }

      
        
        int indexForFoucs = -1;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (NavigationList == null || NavigationList.Count == 0) { 
                return base.ProcessCmdKey(ref msg, keyData);
            }
            if (keyData == Keys.Down || keyData == Keys.Tab) { 
                indexForFoucs++;
                if (indexForFoucs >= NavigationList.Count) {
                    indexForFoucs = 0;
                }
                NavigationList[indexForFoucs].Focus();
                return true;
            }else if(keyData == Keys.Up || keyData == (Keys.Shift | Keys.Tab))
            {
                indexForFoucs--;
                if(indexForFoucs < 0) {
                    indexForFoucs = NavigationList.Count-1;
                }
                NavigationList[indexForFoucs].Focus();
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);    
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            makeTime();
            DateTime now = DateTime.Now;
            if (now.Hour == 07 && (now.Minute >= 00 && now.Minute <= 03)) 
            {
                makeTime();
                timer1.Start();
                makeTheAzkar();
                makeAzkarStructure();
                makeProgrees();
                makeCompleteButton();
                NavigationList.Add(tabControl1);
                flowLayoutPanel2.Visible = false;
                createAreaForDataGrid();


                panels.Add(panel1);
                panels.Add(panel2);
                panels.Add(panel3);
                panels.Add(panel4);
                panels.Add(panel5);
            } else if(now.Hour == 16 && (now.Minute >= 00 && now.Minute <= 03))
            {
                makeTime();
                timer1.Start();
                makeTheAzkar();
                makeAzkarStructure();
                makeProgrees();
                makeCompleteButton();
                NavigationList.Add(tabControl1);
                flowLayoutPanel2.Visible = false;
                createAreaForDataGrid();


                panels.Add(panel1);
                panels.Add(panel2);
                panels.Add(panel3);
                panels.Add(panel4);
                panels.Add(panel5);
            }
        }

        private void button_Enter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.BackColor = Color.Red;

        }

        private void button_Leave(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Enabled)
            {
                b.BackColor = Color.Lime;
            }
            else { 
                b.BackColor= Color.Gray;
            }
        }

        private void btnStartTImer_Click(object sender, EventArgs e)
        {
            
            btnStopTimer.Enabled = true;
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            masbaha.StartTime = DateTime.Now;
            btnStartTImer.Enabled = false;
        }
        
        private void btnStopTimer_Click(object sender, EventArgs e)
        {
            btnStartTImer.Enabled = true;
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            masbaha.EndTime = DateTime.Now;
            btnStopTimer.Enabled = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            if (changeFromNumaricUpDown && masbaha != null) { 
            masbaha.Number = (int)numericUpDown1.Value;
            lblNumber.Text = masbaha.Number.ToString();
                if (masbaha.Number < 1)
                {
                    btnSaveMasbaha.Enabled = false;
                }
                else if (masbaha.Number > 0 && !btnSaveMasbaha.Enabled)
                {
                    btnSaveMasbaha.Enabled=true;
                }
            }
            
        }

      
        bool changeFromNumaricUpDown = true;
        private void btnResetTasbeh_click(object sender, EventArgs e)
        {
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            if (MessageBox.Show("هل تريد الاعادة ؟","تأكيد",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes) {
                changeFromNumaricUpDown=false;
                masbaha.Number = 0;
                numericUpDown1.Value = masbaha.Number;
                lblNumber.Text = masbaha.Number.ToString();
                changeFromNumaricUpDown = true;
                btnSaveMasbaha.Enabled = false;
                btnStartTImer.Enabled = true;
                btnStopTimer.Enabled = false;
                masbaha.StartTime = DateTime.MinValue;
                masbaha.EndTime = DateTime.MinValue;
            }
        }

        private void btnIncremantTasbeh_click(object sender, EventArgs e)
        {
            Masbaha masbaha =(Masbaha)(btnStartTasbeh.Tag);
            changeFromNumaricUpDown = false;
            masbaha.Number++;
            numericUpDown1.Value = masbaha.Number;
            lblNumber.Text = masbaha.Number.ToString();
            changeFromNumaricUpDown = true;
            if (btnSaveMasbaha.Enabled == false)
            { 
                btnSaveMasbaha.Enabled= true;
            }
        }

        private void btnDecreaseTasbeh_Click(object sender, EventArgs e)
        {
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            
            if (masbaha.Number > 0)
            {
                changeFromNumaricUpDown = false;
                masbaha.Number--;
                numericUpDown1.Value = masbaha.Number;
                lblNumber.Text = masbaha.Number.ToString();
                changeFromNumaricUpDown = true;
                if (masbaha.Number <= 0)
                {
                    btnSaveMasbaha.Enabled = false;
                }
            }
            
        }

       
        DataGridView D2 = new DataGridView();
        Panel plD2 = new Panel();
        Label lblMasbaha = new Label();
        private void btnSaveMasbaha_Click(object sender, EventArgs e)
        {
            
            lblNoData.Visible = false;
            btnSave.Enabled = true;
            flowLayoutPanel2.Visible = true;
            flowLayoutPanel2.Controls.Add(((Panel)(dataGridForMasbaha.Tag)));
            Masbaha masbaha = (Masbaha)(btnStartTasbeh.Tag);
            if (MessageBox.Show("هل تريد الحفظ ؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                masbaha.Date = DateTime.Now.ToString("dd / MM / yyyy , hh:mm tt",new CultureInfo("en-US"));
                if(masbaha.StartTime != DateTime.MinValue && masbaha.EndTime == DateTime.MinValue)
                {
                    masbaha.EndTime = DateTime.Now;
                }
                masbaha.TimeToEnd = masbaha.EndTime - masbaha.StartTime;
                btnDeleteMasbaha.Enabled = true;
                masbahas.Add(masbaha);
                fillMasbahaData();
                resetMasbaha();
            }
        }
        BindingList<Masbaha> masbahas = new BindingList<Masbaha>();

        // fill Grid with Mashbaha data

        private void fillMasbahaData() {
            dataGridForMasbaha.DataSource = masbahas;
        }

        

        private void createAreaForDataGrid() {
            DataGridView D = null;
            string textForGridLabel = "";
            
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    D = dataGridForAzkar;
                    textForGridLabel = "الاذكار المكتملة :";
                    D.ReadOnly = true;
                }
                else if(i == 1) {
                    D = dataGridForMasbaha;
                    textForGridLabel = "التسابيح المكتملة :";
                    D.ReadOnly = true;
                }
                Panel plGrid = new Panel();
                Label lblGrid = new Label();


                //panel for data grid view

                plGrid.Width = flowLayoutPanel2.Width - 40 + 2;
                plGrid.BorderStyle = BorderStyle.FixedSingle;
                plGrid.Margin = new Padding(15, 10, 0, 10);

                //lable for data grid view

                lblGrid.Text = textForGridLabel;
                lblGrid.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                lblGrid.Width = plGrid.Width;
                lblGrid.Height = lblGrid.GetPreferredSize(new Size(lblGrid.Width, 0)).Height;
                lblGrid.Margin = new Padding(0, 10, 0, 10);

                //Data grid view for Azkar
                dataGridForMasbaha.AllowUserToAddRows = false;
                D.Location = new Point(5, lblGrid.Bottom + 5);
                D.Width = plGrid.Width - 5;
                D.Width = plGrid.Width - 5;
                D.Height = flowLayoutPanel2.Height / 2;
                D.BackgroundColor = Color.Green;
                D.RightToLeft = RightToLeft.Yes;
                D.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                D.CellFormatting -= Grid_Format;
                D.CellFormatting += Grid_Format;
                D.Tag = plGrid;
                plGrid.Controls.Add(lblGrid);
                plGrid.Controls.Add(D);
                plGrid.Height = D.Bottom + 5;
                
                
                panels.Add(plGrid);
            }
            
        }

        private void Grid_Format(Object sender, DataGridViewCellFormattingEventArgs e) {
            DataGridView d = (DataGridView)sender;
            string prop = d.Columns[e.ColumnIndex].DataPropertyName;
            
            if (d == dataGridForAzkar)
            {
                if (prop == "Id")
                {
                    e.Value = "\u202A" + e.Value.ToString();
                    e.FormattingApplied = true;
                }
            }
            else if (d == dataGridForMasbaha)
            {
                if (prop == "Number" || prop == "TimeToEnd" || prop == "Date" || prop == "StartTime" || prop == "EndTime")
                {
                    e.Value = "\u202A" + e.Value;
                    e.FormattingApplied = true;
                }
                else if (prop == "ZekerType") {
                    e.Value = "\u202B" + e.Value;
                    e.FormattingApplied = true;
                }
            }
        }


        private void btnBackColor_Click(object sender, EventArgs e)
        {
            
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                numericUpDown1.BackColor = colorDialog1.Color;
                foreach (Panel p in panels)
                {
                    
                    p.BackColor = colorDialog1.Color;
                }
            }

        }

       

        private void button7_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.BackColor = colorDialog1.Color;
            }
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                flowLayoutPanel1.BackColor = colorDialog1.Color;
                flowLayoutPanel2.BackColor = colorDialog1.Color;
                tbHistory.BackColor = colorDialog1.Color;
                tbMasbaha.BackColor = colorDialog1.Color;
            }
        }

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowColor = true;
            fontDialog1.ShowEffects = true;
            
            if (fontDialog1.ShowDialog() == DialogResult.OK) {
                btnComplete.Font = new Font(fontDialog1.Font.FontFamily,btnComplete.Font.Size,fontDialog1.Font.Style);
                btnComplete.ForeColor = fontDialog1.Color;
                numericUpDown1.Font = new Font(fontDialog1.Font.FontFamily, numericUpDown1.Font.Size, fontDialog1.Font.Style);
                numericUpDown1.ForeColor = fontDialog1.Color;
                tabControl1.Font = new Font(fontDialog1.Font.FontFamily, tabControl1.Font.Size, fontDialog1.Font.Style);
                tabControl1.ForeColor = fontDialog1.Color;
                flowLayoutPanel1.Controls[0].Font = new Font(fontDialog1.Font.FontFamily, flowLayoutPanel1.Controls[0].Font.Size, fontDialog1.Font.Style);
                flowLayoutPanel1.Controls[0].ForeColor = fontDialog1.Color;
                foreach (Panel p in panels)
                {
                    foreach (Control c in p.Controls)
                    {
                        c.Font = new Font(fontDialog1.Font.FontFamily, c.Font.Size, fontDialog1.Font.Style);
                        c.ForeColor = fontDialog1.Color;
                    }
                }

            }
        }

        
        private void button_MouseEnter(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.BackColor = Color.Red;
        }

        

        private void button_MouseLeave(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            b.BackColor = Color.Lime;
        }

        private void btnStartTasbeh_Click(object sender, EventArgs e)
        {
            if (tbTypeZeker.Text != "")
            {
                btnStartTImer.Enabled = true;
                btnStopTimer.Enabled = false;
                btnIncremantTasbeh.Enabled = true;
                btnDecreaseTasbeh.Enabled = true;
                btnResetTasbeh.Enabled = true;
                btnSaveMasbaha.Enabled = false;
                numericUpDown1.Enabled = true;
                btnChangeTasbeh.Enabled = true;
                Masbaha masbaha = new Masbaha();
                btnStartTasbeh.Tag = masbaha;
                //masbaha.StartTime = DateTime.MinValue;
                //masbaha.EndTime = DateTime.MinValue;
                masbaha.ZekerType = tbTypeZeker.Text;
                masbaha.Number = 0;
                

            }
            btnStartTasbeh.Enabled = false;
        }

        private void btnChangeTasbeh_Click(object sender, EventArgs e)
        {
            (((Button)sender).Tag) = null;
            
            resetMasbaha();
        }

        private void resetMasbaha() {

            Masbaha m = (Masbaha)btnStartTasbeh.Tag;

            btnStartTImer.Enabled = false;
            btnStopTimer.Enabled = false;
            
            btnIncremantTasbeh.Enabled = false;
            btnDecreaseTasbeh.Enabled = false;
            btnResetTasbeh.Enabled = false;
            btnSaveMasbaha.Enabled = false;
            numericUpDown1.Enabled = false;
            btnStartTasbeh.Enabled = true;
            btnChangeTasbeh.Enabled = false;
            lblNumber.Text = "0";
            btnStartTasbeh.Tag = null;
            numericUpDown1.Value = 0;
            tbTypeZeker.Text = "";
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TEXT|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog.FileName);
                if (dataGridForAzkar.Rows.Count != 0) {
                    printDataGridView(dataGridForAzkar,writer);
                }
                if(dataGridForMasbaha.Rows.Count != 0)
                {
                    printDataGridView(dataGridForMasbaha , writer);
                }
                writer.Close();
            }
        }
        //private string forceLTR(string s) {
        //    return "\u202A" + s; 
        //}
        //private string checkIfNull(Object v) {
        //    if (v == null)
        //    {
        //        return "***";
        //    }
        //    if (v is DateTime dt )
        //    {
        //        if (dt == DateTime.MinValue)
        //        {
        //            return "***";
        //        }

        //        return forceLTR(dt.ToString("hh:mm:ss tt", CultureInfo.InvariantCulture));
        //    }
        //    if (v is TimeSpan ts)
        //    {
        //        return ts.ToString(@"hh\:mm\:ss");
        //    }
        //    return v.ToString();
        //}

        private string checkForNUll(object v)
        {
            if (v == null)
            {
                return "***";
            }
            else if (v is DateTime dt)
            {
                return dt == DateTime.MinValue ? "***" : dt.ToString("HH:mm:ss tt",CultureInfo.InvariantCulture);
            }
            else if (v is TimeSpan ts)
            {
                return ts.ToString(@"hh\:mm\:ss");
            }
            else
            { 
                return v.ToString();
            }
        }

        private void printDataGridView(DataGridView d, StreamWriter w) {



            w.WriteLine(new string('=', 120));
            w.WriteLine(d == dataGridForAzkar ? "الاذكار المكتملة :" : "التسابيح المكتملة :");
            w.WriteLine(new string('=', 120));
            w.WriteLine();

            foreach (DataGridViewRow row in d.Rows)
            {
                foreach (DataGridViewColumn col in d.Columns)
                {
                    if (col.Visible)
                    {
                        string colHead = col.HeaderText;
                        object value = row.Cells[col.Index].Value;
                        string stringValue = checkForNUll(value);
                        w.WriteLine($"{colHead} : {stringValue}");
                    }
                }
                w.WriteLine();
                
                w.WriteLine(new string('-', 120));
            }
            w.WriteLine();


            //string LTR = "\u202A";
            //DataGridViewColumn zekerCol = null;
            //List<DataGridViewColumn> otherCols = new List<DataGridViewColumn>();
            //foreach (DataGridViewColumn col in d.Columns) {
            //    if(!col.Visible) { continue; }
            //    if (col.HeaderText == "ZekerType")
            //    {
            //        zekerCol = col;
            //    }
            //    else {
            //        otherCols.Add(col);
            //    }
            //}
            //w.WriteLine(new string('=',120));
            //w.WriteLine(d == dataGridForAzkar ?"الاذكار المكتملة :":"التسابيح المكتملة :");
            //w.WriteLine(new string('=', 120));
            //w.WriteLine();



            //foreach (DataGridViewColumn col in otherCols)
            //{
            //    if (col.Visible && col.HeaderText != "ZekerType") {
            //        // w.Write(col.HeaderText + " | ");
            //        w.Write($"{LTR}{col.HeaderText,-30}");
            //    }
            //}
            //w.WriteLine();
            //w.WriteLine(new string('-', 120));
            //foreach (DataGridViewRow row in d.Rows)
            //{
            //    if (row.IsNewRow) continue;
            //    foreach (DataGridViewColumn col in otherCols)
            //    {
            //        if (!col.Visible) continue;

            //        object value = row.Cells[col.Index].Value;
            //        string text = "";

            //            text = checkIfNull(value);


            //        w.Write($"{LTR}{text,-30}");
            //        //w.Write((LTR+text).PadRight(30));
            //        //w.Write(LTR + text + " | ");
            //    }

            //    w.WriteLine();
            //    w.WriteLine(new string('-', 120));
            //}


            //w.WriteLine();





            //for (int i = 0;i < d.Columns.Count; i++) {

            //    if (d.Columns[i].Visible) {
            //        w.Write(d.Columns[i].HeaderText + '\t');
            //    }
            //    if(i == d.Columns.Count - 1 )
            //    {
            //        w.Write('\n');
            //    }
            //}
            //for (int i = 0; i < d.Rows.Count; i++) {
            //    if (!d.Rows[i].IsNewRow)
            //    {
            //        for (int j = 0; j < d.Rows[i].Cells.Count; j++)
            //        {
            //            if (d.Columns[j].Visible)
            //            {
            //                string cellValue = d.Rows[i].Cells[j].Value.ToString();
            //                w.Write(cellValue + '\t');
            //            }
            //        }
            //        w.Write('\n');
            //    }
            //}
        }

        private void btnSave_EnabledChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled)
            {
                button.BackColor = Color.Red;
            }
            else {
                button.BackColor= Color.Gray;
            }
        }

        private void btnDeleteAzkar_Click(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Remove((Panel)(dataGridForAzkar.Tag));
            CompleteZekerList.Clear();
            btnDeleteAzkar.Enabled = false;
            if (masbahas.Count == 0) {
                flowLayoutPanel2.Visible = false;
                lblNoData.Visible = true;
                btnSave.Enabled = false;
            }
        }

        private void btnDeleteMasbaha_Click(object sender, EventArgs e)
        {
            flowLayoutPanel2.Controls.Remove((Panel)(dataGridForMasbaha.Tag));
            masbahas.Clear();
            btnDeleteMasbaha.Enabled = false;
            if (CompleteZekerList.Count == 0)
            {
                flowLayoutPanel2.Visible = false;
                lblNoData.Visible = true;
                btnSave.Enabled = false;
            }
        }

        


        private void btns_EnableChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Enabled)
            {
                button.BackColor = Color.Lime;
                button.ForeColor = Color.Black;
            }
            else
            {
                button.BackColor = Color.Gray;
                button.ForeColor = Color.White;
            }
        }



        //private void frmMain_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (NavigationList == null || NavigationList.Count == 0) {
        //        return;
        //    }
        //    if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Down) {
        //        indexForFoucs++;
        //        if (indexForFoucs >= NavigationList.Count) {
        //            indexForFoucs = 0;
        //        }
        //        NavigationList[indexForFoucs].Focus();
        //        e.Handled = true;
        //        e.SuppressKeyPress = true;
        //    }else if(e.KeyCode == Keys.Up) {
        //        indexForFoucs--;
        //        if (indexForFoucs < 0) {
        //            indexForFoucs = NavigationList.Count-1;
        //        }
        //        NavigationList[indexForFoucs - 1].Focus();
        //        e.Handled = true;
        //        e.SuppressKeyPress = true;
        //    }

    }
    
}
