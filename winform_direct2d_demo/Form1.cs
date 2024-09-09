using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace winform_direct2d_demo
{
    public partial class Form1 : Form
    {

        internal ViewPanel viewPanel=new ViewPanel();
        internal Direct2dPanel direct2dPanel=new Direct2dPanel();
        





        public Form1()
        {
            InitializeComponent();
            this.initializeForm(); 
        }

        private void initializeForm()//初始化Form
        {
            this.viewPanel.AllowDrop = true;
            this.viewPanel.PanelName = "GDI+";
            this.viewPanel.BackColor=Color.White;
            this.viewPanel.setDoubleBuffer();//设置双缓冲




            this.direct2dPanel.AllowDrop = true;
            this.direct2dPanel.PanelName = "Direct2D";
            this.direct2dPanel.BackColor=Color.White;
            //this.direct2dPanel.setDoubleBuffer();//设置双缓冲

            this.Controls.Add(this.viewPanel);
            

            this.Controls.Add(this.direct2dPanel);
            
            this.Form1_Resize(null,null);


        }






		private void Form1_Resize(object sender, EventArgs e)
        {
            
            this.viewPanel.Location=new Point(0,0);
            this.viewPanel.Size = new Size(this.ClientSize.Width / 2 - 5, this.ClientSize.Height);

            
            this.direct2dPanel.Location=new Point(this.ClientSize.Width/2+5,0);
            this.direct2dPanel.Size = new Size(this.ClientSize.Width / 2 - 5, this.ClientSize.Height);
            

        }
    }
}
