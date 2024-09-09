using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RectF = System.Drawing.RectangleF;

using System.Windows.Forms;

using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using DXRectF=SharpDX.Mathematics.Interop.RawRectangleF;


namespace winform_direct2d_demo
{
    internal class Direct2dPanel:ViewPanel
    {
        internal WindowRenderTarget target;//窗口渲染目标
        internal SharpDX.Direct2D1.Factory factory;//direct2d 工厂
        internal Color4 backColor = Color4.White;//背景色
        internal ImagingFactory wicFactory;//wic工厂
        internal BitmapDecoder bitmapDecoder;//图片解码器
        internal SharpDX.Direct2D1.Bitmap bitmapDirect = null;//direct2d使用的图片对象
        internal SharpDX.DirectWrite.Factory WirteFactory = null;//direct2d文字工厂

        public Direct2dPanel() {
           
        }

        internal void createDxTarget()//创建渲染窗口目标
        {
            this.factory = new SharpDX.Direct2D1.Factory();
            this.WirteFactory=new SharpDX.DirectWrite.Factory();


            RenderTargetProperties targetProperties = new RenderTargetProperties()
			{
				DpiX = 0,
				DpiY = 0,
				MinLevel = FeatureLevel.Level_10,
				PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm,
				SharpDX.Direct2D1.AlphaMode.Premultiplied),
				Type = RenderTargetType.Default,
				Usage = RenderTargetUsage.None
			};

			HwndRenderTargetProperties windowProperties = new HwndRenderTargetProperties()
			{
				Hwnd = this.Handle,
				PixelSize = new SharpDX.Size2(this.ClientSize.Width, this.ClientSize.Height),
				PresentOptions = PresentOptions.None

			};

			target = new WindowRenderTarget(factory, targetProperties, windowProperties);
			target.AntialiasMode = AntialiasMode.PerPrimitive;//设置抗锯齿
															  //target.AntialiasMode = AntialiasMode.Aliased;

			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.Opaque, true);
			this.SetStyle(ControlStyles.UserPaint, true);

			//this.initialiseTarget();

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
                this.createBitmap(path);
                this.cc.fitToWindow(new SizeF(this.bitmapDirect.Size.Width,this.bitmapDirect.Size.Height),this.ClientSize);
            }
            this.Refresh();
        }

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
            this.initialiseTarget();

        }

        internal void drawTitle()//绘制文字
        {
            target.FillRectangle(new DXRectF(0, 0, 100, 30),new SolidColorBrush(target,new Color4(1.0f,0f,0f,1.0f)));
			target.DrawText(this.PanelName, new SharpDX.DirectWrite.TextFormat(WirteFactory, "Javanese Text", 18)
            , new DXRectF(5, 1.5f, 100, 31), new SolidColorBrush(target, Color4.White));

		}

		protected override void OnPaint(PaintEventArgs e)
		{
            //base.OnPaint(e);
            this.target.BeginDraw();
			this.target.Clear(this.backColor);
            this.drawImage();
            
            


            this.drawTitle();




			this.target.EndDraw();
            
 

        }
        internal void drawImage() //绘制图片
        {
            if (this.bitmapDirect == null) {
                return;
            }
            RectF rect = this.cc.getImageRect();
            target.DrawBitmap(this.bitmapDirect, new DXRectF(rect.X,rect.Y,rect.Right,rect.Bottom), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);


        }
        protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
            this.createDxTarget();
		}




		internal void createBitmap(string picPath)//创建direct2d图片
        {
            wicFactory = new ImagingFactory();
            bitmapDecoder = new BitmapDecoder(wicFactory, picPath, DecodeOptions.CacheOnLoad);
            BitmapFrameDecode frame = bitmapDecoder.GetFrame(0);
            var convert = new FormatConverter(wicFactory);
            convert.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);
            this.bitmapDirect = SharpDX.Direct2D1.Bitmap.FromWicBitmap(target, convert);
            this.cc.setRectangeF(new RectF(0,0,this.bitmapDirect.Size.Width,this.bitmapDirect.Size.Height));
        }


        internal void initialiseTarget() //甚至绘图区域大小
        {
            if (this.target != null)
            {
                target.Resize(new SharpDX.Size2(this.ClientSize.Width, this.ClientSize.Height));

            }
        }


        protected override void Dispose(bool dispose)
        {
            if (dispose)
            {
                target.Dispose();
                factory.Dispose();
            }
            base.Dispose(dispose);
        }

    }
}
