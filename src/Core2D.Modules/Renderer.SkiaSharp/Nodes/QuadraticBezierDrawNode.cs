﻿using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class QuadraticBezierDrawNode : DrawNode, IQuadraticBezierDrawNode
    {
        public IQuadraticBezierShape QuadraticBezier { get; set; }
        public SKPath Geometry { get; set; }

        public QuadraticBezierDrawNode(IQuadraticBezierShape quadraticBezier, IShapeStyle style)
        {
            Style = style;
            QuadraticBezier = quadraticBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToSKPath(QuadraticBezier);
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (QuadraticBezier.IsFilled)
            {
                canvas.DrawPath(Geometry, Fill);
            }

            if (QuadraticBezier.IsStroked)
            {
                canvas.DrawPath(Geometry, Stroke);
            }
        }
    }
}
