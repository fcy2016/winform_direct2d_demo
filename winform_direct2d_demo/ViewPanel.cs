using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using winform_direct2d_demo.DIRECT;

namespace winform_direct2d_demo
{
    internal class ViewPanel:Panel
    {

        internal string PanelName { get; set; } //panel名称

        internal Bitmap Pic { get; set; }//需要绘制图片
        internal CoordinateConversion cc;

        public ViewPanel() {
            
            cc = new CoordinateConversion();
            


            this.Pic = null;
            

        }

        public void setDoubleBuffer() {
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            
        }


        public void InitImage() {
            this.Pic = null;
        }

        public void setPic(string picPath) {
            this.Pic = new Bitmap(picPath);
            this.cc.setRectangeF(new RectangleF(0,0,this.Pic.Width,this.Pic.Height));
            
            

        }




        internal bool leftMouseDown = false;

        /// <summary>
        /// 操作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            DragEventArgs de = (DragEventArgs)drgevent;
            string path = ((System.Array)de.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();       //获得路径
            string[] paths = path.Split('.');
            string extName = paths[paths.Length - 1];
            if (extName == "jpg" || extName == "jpeg" || extName == "png")
            {
                this.setPic(path);
                this.cc.fitToWindow(this.Pic.Size, this.ClientSize);
            }
            this.Refresh();
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            DragEventArgs de = (DragEventArgs)drgevent;
            if (de.Data.GetDataPresent(DataFormats.FileDrop))
                de.Effect = DragDropEffects.All;      //重要代码：表明是所有类型的数据，比如文件路径
            else
                de.Effect = DragDropEffects.None;
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            this.cc.zoom((DIRECT)e.Delta, this.Pic.Size, e.Location);
            this.Refresh();
        }


        protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) {
                this.leftMouseDown = true;
                this.cc.setOffsetSize(e.Location);
            }
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                this.leftMouseDown = false;
            }
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
             if (this.leftMouseDown) {
                this.cc.moveTo(e.Location);
                this.Refresh();       
            }
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
            if (this.Pic == null)
            {
                return;
            }
            if (e.KeyCode == Keys.F1)
            {
                this.imageFitPanel();
            }

            if (e.KeyCode == Keys.F2)
            {
                this.imageActualSize();
            }

            if (e.KeyCode == Keys.F3)
            {
                this.imageMoveToCenter();
            }
            this.Refresh();
        }
		protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;            g.Clear(Color.White);
            
            this.drawPic(g);

            this.drawTitle(g);
            //g.Dispose();

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Focus(); 
        }

        public void keyDown(KeyEventArgs e)
        {
            //base.OnKeyDown(e);

            if (e.KeyCode == Keys.F1)
            {
                this.imageFitPanel();
                return;
            }

            if (e.KeyCode == Keys.F2)
            {
                this.imageActualSize();
                return;
            }

            if (e.KeyCode == Keys.F3)
            {
                this.imageMoveToCenter();
                return;
            }

        }

        public void imageFitPanel() {
            this.cc.fitToWindow(this.Pic.Size, this.ClientSize);
        }
        public void imageActualSize() {
            this.cc.actualSize(this.Pic.Size);
        }
        public void imageMoveToCenter() { 
            this.cc.moveToWindowCenter(this.ClientSize);
        }






        internal void drawTitle(Graphics g) {
            g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 100, 30));
            g.DrawString(this.PanelName, new Font("Javanese Text", 14), new SolidBrush(Color.White), new Point(2, 0));
           

        }

        internal void drawPic(Graphics g) {
            if (this.Pic == null) {
                return;
            }
            g.DrawImage(this.Pic, this.cc.getImageRect());
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ViewPanel
            // 
            this.AllowDrop = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResumeLayout(false);

        }
    }
}
