﻿using Core2D;
using Xunit;

namespace Core2D.Shapes.UnitTests
{
    public class CubicBezierShapeTests
    {
        private readonly IFactory _factory = new Factory();

        [Fact]
        [Trait("Core2D.Shapes", "Shapes")]
        public void Inherits_From_BaseShape()
        {
            var style = _factory.CreateShapeStyle();
            var target = _factory.CreateCubicBezierShape(0, 0, 0, 0, 0, 0, 0, 0, style);
            Assert.True(target is BaseShape);
        }
    }
}
