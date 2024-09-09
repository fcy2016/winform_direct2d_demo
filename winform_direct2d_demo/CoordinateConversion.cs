using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

enum DIRECT
{
    UP = 1,
    DOWN = -1,
}


namespace winform_direct2d_demo
{
    internal class CoordinateConversion
    {
        private float multiple = 1;
        private float x;
        private float y;
        private float width;
        private float height;

        private float offsetW;
        private float offsetH;

        private float zoomStep = 0.02f;


        public float Multiple { get => multiple; set => multiple = value; }
        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }
        public float ZoomStep { get => zoomStep; set => zoomStep = value; }

        public void setRectangeF(RectangleF rect)
        {
            this.x = rect.X;
            this.y = rect.Y;
            this.width = rect.Width;
            this.height = rect.Height;
        }

        public RectangleF screenToImage(RectangleF screenRect)
        {
            PointF newPoint = this.screenPointToImagePoint(screenRect.Location);
            SizeF newSize = new SizeF(screenRect.Width / this.multiple, screenRect.Height / this.multiple);
            return new RectangleF(newPoint, newSize);
        }

        public RectangleF imageToScreen(RectangleF imageRect)
        {

            PointF newPoint = this.imagePointToScreenPoint(imageRect.Location);
            SizeF newSize = new SizeF(imageRect.Width * this.multiple, imageRect.Height * this.multiple);
            return new RectangleF(newPoint, newSize);
        }

        public RectangleF getImageRect()
        {
            return new RectangleF(x, y, width, height);
        }


        public void moveTo(Point pos)
        {
            this.x = pos.X - this.offsetW;
            this.y = pos.Y - this.offsetH;
        }

        public void setOffsetSize(PointF pos)
        {
            this.offsetW = pos.X - this.x;
            this.offsetH = pos.Y - this.y;
        }

        public void zoom(DIRECT direct, SizeF imageSize, Point pos)
        {
            if (direct > 0) {
                direct = DIRECT.UP;
            }
            if (direct < 0) { 
                direct=DIRECT.DOWN;
            }

            if (direct == 0) { return; }

            if (direct == DIRECT.UP)
            {
                if (this.multiple > 3) { return; }
            }
            else
            {
                if (this.multiple < 0.05) { return; }
            }



            PointF oldPos = this.screenPointToImagePoint(pos);

            //if (direct > 0)
            //{
            //    this.multiple += this.zoomStep;
            //}
            //else
            //{
            //    this.multiple -= this.zoomStep;
            //}


            float step = this.zoomStep * (int)direct;

            this.multiple += step;


            this.width = imageSize.Width * multiple;
            this.height = imageSize.Height * multiple;

            //PointF curPos = new PointF(oldPos.X * multiple, oldPos.Y * multiple);


            PointF newPos = this.imagePointToScreenPoint(oldPos);

            float offsetX = newPos.X - pos.X;
            float offsetY = newPos.Y - pos.Y;
            this.x -= offsetX;
            this.y -= offsetY;

        }

        public PointF screenPointToImagePoint(PointF screenPos)
        {
            float x = screenPos.X - this.x;
            float y = screenPos.Y - this.y;

            x = x / this.multiple;
            y = y / this.multiple;


            return new PointF(x, y);
        }

        public PointF imagePointToScreenPoint(PointF imagePos)
        {

            float x = imagePos.X * this.multiple;
            float y = imagePos.Y * this.multiple;

            x = x + this.x;
            y = y + this.y;

            return new PointF(x, y);

        }


        public void fitToWindow(SizeF imageSize, SizeF screenSize)
        {

            float imageScale = imageSize.Width / imageSize.Height;
            float screenScale = screenSize.Width / screenSize.Height;

            if (imageScale > screenScale)
            {

                this.width = screenSize.Width;
                this.height = imageSize.Height * (screenSize.Width / imageSize.Width);

                this.multiple = this.width / imageSize.Width;

                this.x = 0;
                this.y = (screenSize.Height - this.height) / 2;

            }
            else
            {

                this.width = imageSize.Width * (screenSize.Height / imageSize.Height);
                this.height = screenSize.Height;

                this.multiple = this.height / imageSize.Height;

                this.x = (screenSize.Width - this.width) / 2;
                this.y = 0;
            }
        }

        public void actualSize(SizeF imageSize)
        {
            this.width = imageSize.Width;
            this.height = imageSize.Height;
            this.multiple = 1;
        }

        public void moveToWindowCenter(SizeF screenSize)
        {
            this.x = (screenSize.Width - this.width) / 2;
            this.y = (screenSize.Height - this.height) / 2;

        }

    }
}
