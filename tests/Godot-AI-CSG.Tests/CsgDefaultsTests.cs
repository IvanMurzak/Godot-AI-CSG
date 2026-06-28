/*
┌──────────────────────────────────────────────────────────────────┐
│  Author: Ivan Murzak (https://github.com/IvanMurzak)             │
│  Copyright (c) 2026 Ivan Murzak                                  │
│  Licensed under the Apache License, Version 2.0.                 │
│  See the LICENSE file in the project root for more information.  │
└──────────────────────────────────────────────────────────────────┘
*/
#nullable enable
using com.IvanMurzak.Godot.MCP.CSG;
using Xunit;

namespace com.IvanMurzak.Godot.MCP.CSG.Tests
{
    /// <summary>
    /// Pure-managed specs for <see cref="CsgDefaults"/> — the recommended starter config AND the value-clamping
    /// rules the editor tools reuse before writing to a live node. These are the testable core that backs the
    /// editor-only <c>csg-*-create</c> / <c>csg-set-operation</c> tools (which themselves need a live Godot
    /// editor, exercised by the E2E leg).
    /// </summary>
    public class CsgDefaultsTests
    {
        [Fact]
        public void For_Box_ReturnsPopulatedStarterConfig()
        {
            var info = CsgDefaults.For(CsgPrimitiveKind.Box);

            Assert.Equal("Box", info.Kind);
            Assert.Equal(string.Empty, info.NodePath); // not bound to a node
            Assert.Equal("Union", info.Operation);
            Assert.Equal(CsgDefaults.DefaultBoxSize, info.SizeX);
            Assert.Equal(CsgDefaults.DefaultBoxSize, info.SizeY);
            Assert.Equal(CsgDefaults.DefaultBoxSize, info.SizeZ);
            Assert.Equal(0, info.RadialSegments);
        }

        [Fact]
        public void For_Sphere_ReturnsRadiusAndSegments()
        {
            var info = CsgDefaults.For(CsgPrimitiveKind.Sphere);

            Assert.Equal("Sphere", info.Kind);
            Assert.Equal(CsgDefaults.DefaultRadius, info.Radius);
            Assert.Equal(CsgDefaults.DefaultRadialSegments, info.RadialSegments);
        }

        [Fact]
        public void For_Cylinder_ReturnsRadiusAndHeight()
        {
            var info = CsgDefaults.For(CsgPrimitiveKind.Cylinder);

            Assert.Equal("Cylinder", info.Kind);
            Assert.Equal(CsgDefaults.DefaultRadius, info.Radius);
            Assert.Equal(CsgDefaults.DefaultHeight, info.Height);
        }

        [Fact]
        public void For_Combiner_HasNoPrimitiveScalars()
        {
            var info = CsgDefaults.For(CsgPrimitiveKind.Combiner);

            Assert.Equal("Combiner", info.Kind);
            Assert.Equal("Union", info.Operation);
            Assert.Equal(0, info.SizeX);
            Assert.Equal(0, info.Radius);
            Assert.Equal(0, info.Height);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-2.0)]
        [InlineData(double.NaN)]
        public void ClampSize_NonPositive_BecomesMinPositive(double input)
        {
            Assert.Equal(CsgDefaults.MinDimension, CsgDefaults.ClampSize(input));
        }

        [Fact]
        public void ClampSize_PositivePassesThrough()
        {
            Assert.Equal(3.5, CsgDefaults.ClampSize(3.5));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-1.0)]
        [InlineData(double.NaN)]
        public void ClampRadius_NonPositive_BecomesMinPositive(double input)
        {
            Assert.Equal(CsgDefaults.MinDimension, CsgDefaults.ClampRadius(input));
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(-4.0)]
        [InlineData(double.NaN)]
        public void ClampHeight_NonPositive_BecomesMinPositive(double input)
        {
            Assert.Equal(CsgDefaults.MinDimension, CsgDefaults.ClampHeight(input));
        }

        [Fact]
        public void ClampHeight_PositivePassesThrough()
        {
            Assert.Equal(4.0, CsgDefaults.ClampHeight(4.0));
        }

        [Theory]
        [InlineData(0, 3)]
        [InlineData(-5, 3)]
        [InlineData(2, 3)]
        [InlineData(3, 3)]
        [InlineData(24, 24)]
        public void ClampRadialSegments_FloorsAtThree(int input, int expected)
        {
            Assert.Equal(expected, CsgDefaults.ClampRadialSegments(input));
        }
    }
}
