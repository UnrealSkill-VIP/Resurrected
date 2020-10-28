using SharpDX;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectWrite;

namespace RESUPUB.Helpers
{
    public class OutlineTextRender : SharpDX.DirectWrite.TextRenderer
    {
        readonly SharpDX.Direct2D1.Factory _factory;
        readonly RenderTarget _surface;
        readonly Brush _brush;

        public OutlineTextRender(RenderTarget surface, Brush brush)
        {
            _factory = surface.Factory;
            _surface = surface;
            _brush = brush;
        }

        public Result DrawGlyphRun(object clientDrawingContext, float baselineOriginX, float baselineOriginY, MeasuringMode measuringMode, SharpDX.DirectWrite.GlyphRun glyphRun, SharpDX.DirectWrite.GlyphRunDescription glyphRunDescription, ComObject clientDrawingEffect)
        {
            using (PathGeometry path = new PathGeometry(_factory))
            {
                using (GeometrySink sink = path.Open())
                {
                    glyphRun.FontFace.GetGlyphRunOutline(glyphRun.FontSize, glyphRun.Indices, glyphRun.Advances, glyphRun.Offsets, glyphRun.IsSideways, (glyphRun.BidiLevel % 2) > 0, sink);

                    sink.Close();
                }

                Matrix matrix = Matrix.Identity;
                matrix = matrix * Matrix.Translation(baselineOriginX, baselineOriginY, 0);

                using (TransformedGeometry transformedGeometry = new TransformedGeometry(_factory, path, matrix))
                {
                    _surface.DrawGeometry(transformedGeometry, _brush);
                } 

            }
            return new Result();
        }

        public Result DrawInlineObject(object clientDrawingContext, float originX, float originY, SharpDX.DirectWrite.InlineObject inlineObject, bool isSideways, bool isRightToLeft, ComObject clientDrawingEffect)
        {
            return new Result();
        }

        public Result DrawStrikethrough(object clientDrawingContext, float baselineOriginX, float baselineOriginY, ref SharpDX.DirectWrite.Strikethrough strikethrough, ComObject clientDrawingEffect)
        {
            return new Result();
        }

        public Result DrawUnderline(object clientDrawingContext, float baselineOriginX, float baselineOriginY, ref SharpDX.DirectWrite.Underline underline, ComObject clientDrawingEffect)
        {
            return new Result();
        }

        //public SharpDX.DirectWrite.Matrix GetCurrentTransform(object clientDrawingContext)
        //{
        //    return new SharpDX.DirectWrite.Matrix();
        //}

        public float GetPixelsPerDip(object clientDrawingContext)
        {
            return 0;
        }

        public bool IsPixelSnappingDisabled(object clientDrawingContext)
        {
            return true; ;
        }

        public IDisposable Shadow
        {
            get
            {
                return null;
            }
            set
            {
                // throw new NotImplementedException();
            }
        }

        public void Dispose()
        {

        }

        Matrix3x2 PixelSnapping.GetCurrentTransform(object clientDrawingContext)
        {
            throw new NotImplementedException();
        }


        public void strokeText(string text, float x, float y, float maxWidth, SharpDX.Color clr, float size, float width = 200)
        {
            // http://msdn.microsoft.com/en-us/library/windows/desktop/dd756692(v=vs.85).aspx

            // FIXME make field


            using (SharpDX.DirectWrite.Factory factory = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared))
            {
                using (TextFormat format = new TextFormat(factory, "Tahoma", FontWeight.Normal, FontStyle.Normal, FontStretch.Normal, size))
                {
                    using (TextLayout layout = new SharpDX.DirectWrite.TextLayout(factory, text, format, width, 20))
                    {
                        using (var brush = new SolidColorBrush(_surface, clr))
                        {
                            //var render = new OutlineTextRender(_surface, brush);

                            layout.Draw(this, x, y);

                            //_surface.DrawTextLayout(new DrawingPointF(x, y), layout, brush);

                            //   _surface.DrawGlyphRun(new DrawingPointF(x, y), run, brush, MeasuringMode.Natural);
                        }
                    }
                }
            }
        }
    }
}
