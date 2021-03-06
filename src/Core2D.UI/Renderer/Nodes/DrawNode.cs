﻿using System;
using Core2D.Renderer;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;

namespace Core2D.UI.Renderer
{
    internal abstract class DrawNode : IDrawNode
    {
        public IShapeStyle Style { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public AM.IBrush Fill { get; set; }
        public AM.IPen Stroke { get; set; }
        public A.Point Center { get; set; }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = AvaloniaDrawUtil.ToBrush(Style.Fill);
            Stroke = AvaloniaDrawUtil.ToPen(Style, Style.Thickness);
        }

        public virtual void Draw(object dc, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = Style.Thickness;

            if (ScaleThickness)
            {
                thickness /= zoom;
            }

            if (scale != 1.0)
            {
                thickness /= scale;
            }

            if (Stroke.Thickness != thickness)
            {
                Stroke = AvaloniaDrawUtil.ToPen(Style, thickness);
            }

            var context = dc as AM.DrawingContext;
            var translateDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            OnDraw(dc, zoom);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        public abstract void OnDraw(object dc, double zoom);

        public virtual void Dispose()
        {
        }
    }
}
