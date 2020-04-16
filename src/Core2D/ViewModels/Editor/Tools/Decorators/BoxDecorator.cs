﻿using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Input;
using Core2D.Layout;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;

namespace Core2D.Editor.Tools.Decorators
{
    /// <summary>
    /// Box decorator.
    /// </summary>
    public class BoxDecorator : ObservableObject, IDrawable, IDecorator
    {
        private bool _isVisible;
        private readonly IServiceProvider _serviceProvider;
        private IShapeStyle _style;
        private IMatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        public IList<IBaseShape> _shapes;
        private readonly ILayerContainer _layer;
        private readonly IFactory _factory;
        private readonly double _sizeLarge;
        private readonly double _sizeSmall;
        private readonly double _rotateDistance;
        private GroupBox _groupBox;
        private readonly IShapeStyle _handleStyle;
        private readonly IShapeStyle _boundsStyle;
        private readonly IRectangleShape _moveHandle;
        private readonly ILineShape _rotateLine;
        private readonly IEllipseShape _rotateHandle;
        private readonly IEllipseShape _topLeftHandle;
        private readonly IEllipseShape _topRightHandle;
        private readonly IEllipseShape _bottomLeftHandle;
        private readonly IEllipseShape _bottomRightHandle;
        private readonly IRectangleShape _topHandle;
        private readonly IRectangleShape _bottomHandle;
        private readonly IRectangleShape _leftHandle;
        private readonly IRectangleShape _rightHandle;

        /// <inheritdoc/>
        public IList<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        /// <inheritdoc/>
        public IShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        /// <inheritdoc/>
        public IMatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

        /// <inheritdoc/>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="BoxDecorator"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="shapes">The shapes collection.</param>
        /// <param name="layer">The layer container.</param>
        public BoxDecorator(IServiceProvider serviceProvider, List<IBaseShape> shapes, ILayerContainer layer)
        {
            _serviceProvider = serviceProvider;
            _shapes = shapes;
            _layer = layer;
            _factory = _serviceProvider.GetService<IFactory>();
            _sizeLarge = 3.75;
            _sizeSmall = 2.8125;
            _rotateDistance = -16.875;

            _groupBox = new GroupBox(_shapes);

            _handleStyle = _factory.CreateShapeStyle("Handle", 255, 0, 191, 255, 255, 255, 255, 255, 2.0);
            _boundsStyle = _factory.CreateShapeStyle("Bounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);

            _moveHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Left,
                _groupBox.Bounds.Top,
                _groupBox.Bounds.Right,
                _groupBox.Bounds.Bottom,
                _boundsStyle, true, false, name: "_moveHandle");

            _rotateLine = _factory.CreateLineShape(
                _groupBox.Bounds.CenterX,
                _groupBox.Bounds.Top,
                _groupBox.Bounds.CenterX,
                _groupBox.Bounds.Top + _rotateDistance,
                _boundsStyle, true, name: "_rotateLine");

            _rotateHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.CenterX - _sizeLarge,
                _groupBox.Bounds.Top + _rotateDistance - _sizeLarge,
                _groupBox.Bounds.CenterX + _sizeLarge,
                _groupBox.Bounds.Top + _rotateDistance + _sizeLarge,
                _handleStyle, true, true, name: "_rotateHandle");

            _topLeftHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Left - _sizeLarge,
                _groupBox.Bounds.Top - _sizeLarge,
                _groupBox.Bounds.Left + _sizeLarge,
                _groupBox.Bounds.Top + _sizeLarge,
                _handleStyle, true, true, name: "_topLeftHandle");

            _topRightHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Right - _sizeLarge,
                _groupBox.Bounds.Top - _sizeLarge,
                _groupBox.Bounds.Right + _sizeLarge,
                _groupBox.Bounds.Top + _sizeLarge,
                _handleStyle, true, true, name: "_topRightHandle");

            _bottomLeftHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Left - _sizeLarge,
                _groupBox.Bounds.Bottom - _sizeLarge,
                _groupBox.Bounds.Left + _sizeLarge,
                _groupBox.Bounds.Bottom + _sizeLarge,
                _handleStyle, true, true, name: "_bottomLeftHandle");

            _bottomRightHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Right - _sizeLarge,
                _groupBox.Bounds.Bottom - _sizeLarge,
                _groupBox.Bounds.Right + _sizeLarge,
                _groupBox.Bounds.Bottom + _sizeLarge,
                _handleStyle, true, true, name: "_bottomRightHandle");

            _topHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.CenterX - _sizeSmall,
                _groupBox.Bounds.Top - _sizeSmall,
                _groupBox.Bounds.CenterX + _sizeSmall,
                _groupBox.Bounds.Top + _sizeSmall,
                _handleStyle, true, true, name: "_topHandle");

            _bottomHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.CenterX - _sizeSmall,
                _groupBox.Bounds.Bottom - _sizeSmall,
                _groupBox.Bounds.CenterX + _sizeSmall,
                _groupBox.Bounds.Bottom + _sizeSmall,
                _handleStyle, true, true, name: "_bottomHandle");

            _leftHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Left - _sizeSmall,
                _groupBox.Bounds.CenterY - _sizeSmall,
                _groupBox.Bounds.Left + _sizeSmall,
                _groupBox.Bounds.CenterY + _sizeSmall,
                _handleStyle, true, true, name: "_leftHandle");

            _rightHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Right - _sizeSmall,
                _groupBox.Bounds.CenterY - _sizeSmall,
                _groupBox.Bounds.Right + _sizeSmall,
                _groupBox.Bounds.CenterY + _sizeSmall,
                _handleStyle, true, true, name: "_rightHandle");
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual object BeginTransform(object dc, IShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual void EndTransform(object dc, IShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        /// <inheritdoc/>
        public virtual void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            var state = BeginTransform(dc, renderer);

            if (_isVisible)
            {
                _moveHandle.Draw(dc, renderer, dx, dy);
                _rotateLine.Draw(dc, renderer, dx, dy);
                _rotateHandle.Draw(dc, renderer, dx, dy);
                _topLeftHandle.Draw(dc, renderer, dx, dy);
                _topRightHandle.Draw(dc, renderer, dx, dy);
                _bottomLeftHandle.Draw(dc, renderer, dx, dy);
                _bottomRightHandle.Draw(dc, renderer, dx, dy);
                _topHandle.Draw(dc, renderer, dx, dy);
                _bottomHandle.Draw(dc, renderer, dx, dy);
                _leftHandle.Draw(dc, renderer, dx, dy);
                _rightHandle.Draw(dc, renderer, dx, dy);
            }

            EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public virtual bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Update(bool rebuild)
        {
            if (rebuild)
            {
                _groupBox = new GroupBox(_shapes);
            }
            else
            {
                _groupBox.Update();
            }

            _moveHandle.TopLeft.X = _groupBox.Bounds.Left;
            _moveHandle.TopLeft.Y = _groupBox.Bounds.Top;
            _moveHandle.BottomRight.X = _groupBox.Bounds.Right;
            _moveHandle.BottomRight.Y = _groupBox.Bounds.Bottom;

            _rotateLine.Start.X = _groupBox.Bounds.CenterX;
            _rotateLine.Start.Y = _groupBox.Bounds.Top;
            _rotateLine.End.X = _groupBox.Bounds.CenterX;
            _rotateLine.End.Y = _groupBox.Bounds.Top + _rotateDistance;

            _rotateHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeLarge;
            _rotateHandle.TopLeft.Y = _groupBox.Bounds.Top + _rotateDistance - _sizeLarge;
            _rotateHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeLarge;
            _rotateHandle.BottomRight.Y = _groupBox.Bounds.Top + _rotateDistance + _sizeLarge;

            _topLeftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeLarge;
            _topLeftHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeLarge;
            _topLeftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeLarge;
            _topLeftHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeLarge;

            _topRightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeLarge;
            _topRightHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeLarge;
            _topRightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeLarge;
            _topRightHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeLarge;

            _bottomLeftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeLarge;
            _bottomLeftHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeLarge;
            _bottomLeftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeLarge;
            _bottomLeftHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeLarge;

            _bottomRightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeLarge;
            _bottomRightHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeLarge;
            _bottomRightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeLarge;
            _bottomRightHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeLarge;

            _topHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeSmall;
            _topHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeSmall;
            _topHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeSmall;
            _topHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeSmall;

            _bottomHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeSmall;
            _bottomHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeSmall;
            _bottomHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeSmall;
            _bottomHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeSmall;

            _leftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeSmall;
            _leftHandle.TopLeft.Y = _groupBox.Bounds.CenterY - _sizeSmall;
            _leftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeSmall;
            _leftHandle.BottomRight.Y = _groupBox.Bounds.CenterY + _sizeSmall;

            _rightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeSmall;
            _rightHandle.TopLeft.Y = _groupBox.Bounds.CenterY - _sizeSmall;
            _rightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeSmall;
            _rightHandle.BottomRight.Y = _groupBox.Bounds.CenterY + _sizeSmall;

            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public void Show()
        {
            _mode = Mode.None;
            _isVisible = true;

            var shapesBuilder = _layer.Shapes.ToBuilder();
            shapesBuilder.Add(_moveHandle);
            shapesBuilder.Add(_rotateLine);
            shapesBuilder.Add(_rotateHandle);
            shapesBuilder.Add(_topLeftHandle);
            shapesBuilder.Add(_topRightHandle);
            shapesBuilder.Add(_bottomLeftHandle);
            shapesBuilder.Add(_bottomRightHandle);
            shapesBuilder.Add(_topHandle);
            shapesBuilder.Add(_bottomHandle);
            shapesBuilder.Add(_leftHandle);
            shapesBuilder.Add(_rightHandle);
            _layer.Shapes = shapesBuilder.ToImmutable();
            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public void Hide()
        {
            _mode = Mode.None;
            _isVisible = false;

            var shapesBuilder = _layer.Shapes.ToBuilder();
            shapesBuilder.Remove(_moveHandle);
            shapesBuilder.Remove(_rotateLine);
            shapesBuilder.Remove(_rotateHandle);
            shapesBuilder.Remove(_topLeftHandle);
            shapesBuilder.Remove(_topRightHandle);
            shapesBuilder.Remove(_bottomLeftHandle);
            shapesBuilder.Remove(_bottomRightHandle);
            shapesBuilder.Remove(_topHandle);
            shapesBuilder.Remove(_bottomHandle);
            shapesBuilder.Remove(_leftHandle);
            shapesBuilder.Remove(_rightHandle);
            _layer.Shapes = shapesBuilder.ToImmutable();
            _layer.Invalidate();
        }

        private Mode _mode = Mode.None;
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;

        private enum Mode
        {
            None,
            Move,
            Rotate,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <inheritdoc/>
        public bool HitTest(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double x, double y) = args;
            (double sx, double sy) = editor.TryToSnap(args);

            var result = editor.HitTest.TryToGetShape(_shapes, new Point2(x, y), editor.Project.Options.HitThreshold / editor.PageState.ZoomX);
            if (result != null)
            {
                _mode = Mode.None;

                if (result == _moveHandle)
                {
                    _mode = Mode.Move;
                }
                else if (result == _rotateHandle)
                {
                    _mode = Mode.Rotate;
                }
                else if (result == _topLeftHandle)
                {
                    _mode = Mode.TopLeft;
                }
                else if (result == _topRightHandle)
                {
                    _mode = Mode.TopRight;
                }
                else if (result == _bottomLeftHandle)
                {
                    _mode = Mode.BottomLeft;
                }
                else if (result == _bottomRightHandle)
                {
                    _mode = Mode.BottomRight;
                }
                else if (result == _topHandle)
                {
                    _mode = Mode.Top;
                }
                else if (result == _bottomHandle)
                {
                    _mode = Mode.Bottom;
                }
                else if (result == _leftHandle)
                {
                    _mode = Mode.Left;
                }
                else if (result == _rightHandle)
                {
                    _mode = Mode.Right;
                }

                if (_mode != Mode.None)
                {
                    _startX = sx;
                    _startY = sy;
                    _historyX = _startX;
                    _historyY = _startY;
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            double dx = sx - _startX;
            double dy = sy - _startY;

            _startX = sx;
            _startY = sy;

            switch (_mode)
            {
                case Mode.None:
                    break;
                case Mode.Move:
                    {
                        foreach (var shape in _shapes)
                        {
                            shape.Move(null, dx, dy);
                        }
                    }
                    break;
                case Mode.Rotate:
                    {
                        // TODO:
                    }
                    break;
                case Mode.Top:
                    {
                        // TODO:
                    }
                    break;
                case Mode.Bottom:
                    {
                        // TODO:
                    }
                    break;
                case Mode.Left:
                    {
                        // TODO:
                    }
                    break;
                case Mode.Right:
                    {
                        // TODO:
                    }
                    break;
                case Mode.TopLeft:
                    {
                        // TODO:
                    }
                    break;
                case Mode.TopRight:
                    {
                        // TODO:
                    }
                    break;
                case Mode.BottomLeft:
                    {
                        // TODO:
                    }
                    break;
                case Mode.BottomRight:
                    {
                        // TODO:
                    }
                    break;
            }
        }
    }
}