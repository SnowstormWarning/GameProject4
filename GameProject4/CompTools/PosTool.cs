using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Tools.Computation
{
    public static class PosTool
    {
        public static Microsoft.Xna.Framework.Vector2 RelativeVector(float xFactor, float yFactor)
        {
            return new Microsoft.Xna.Framework.Vector2(0 * xFactor, 0 * yFactor);
        }

        public static Microsoft.Xna.Framework.Vector2 RelativeVector(float xFactor, float yFactor, float width, float height)
        {
            return new Microsoft.Xna.Framework.Vector2(0 * xFactor - width / 2f, 0 * yFactor - width / 2f);
        }

        public static bool IsBetween(float num, float min, float max)
        {
            return num >= min && num <= max;
        }

        public static bool IsTouching(Microsoft.Xna.Framework.Vector2 circle, float radius, Microsoft.Xna.Framework.Vector2 bounds)
        {
            //if()
            return true;
        }

        public static Microsoft.Xna.Framework.Vector2 VectorSubtraction(Microsoft.Xna.Framework.Vector2 a, Microsoft.Xna.Framework.Vector2 b)
        {
            return new Microsoft.Xna.Framework.Vector2(a.X - b.X, a.Y - b.Y);
        }

        /// <summary>
        /// Detects a collision between two BoundingCircles
        /// </summary>
        /// <param name="a">Circle one</param>
        /// <param name="b">Circle two</param>
        /// <returns>true if collision</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            return Math.Pow(a.Radius + b.Radius, 2) >= (Math.Pow(a.Center.X - b.Center.X, 2) + Math.Pow(a.Center.Y - b.Center.Y, 2));
        }

        /// <summary>
        /// Detects collisions between two rectangles
        /// </summary>
        /// <param name="a">the first rectangle</param>
        /// <param name="b">The second rectangle</param>
        /// <returns>If they are colliding</returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !((a.Right < b.Left || a.Left > b.Right) || (a.Top > b.Bottom || a.Bottom < b.Top));
        }

        public static bool Collides(BoundingCircle c, BoundingRectangle r)
        {
            float nearestX = MathHelper.Clamp(c.Center.X, r.Left, r.Right);
            float nearestY = MathHelper.Clamp(c.Center.Y, r.Top, r.Bottom);
            return Math.Pow(c.Radius, 2) >= Math.Pow(c.Center.X - nearestX, 2) + Math.Pow(c.Center.Y - nearestY, 2);
        }

        public static bool Collides(BoundingRectangle r, BoundingCircle c)
        {
            return Collides(c, r);
        }
    }
    public struct BoundingCircle
    {
        /// <summary>
        /// The center of the bounding circle
        /// </summary>
        public Microsoft.Xna.Framework.Vector2 Center;

        /// <summary>
        /// The radius of the bounding circle
        /// </summary>
        public float Radius;

        /// <summary>
        /// Contructs a new bounding circle from the center and radius.
        /// </summary>
        /// <param name="center">The center vector</param>
        /// <param name="radius">The radius float</param>
        public BoundingCircle(Microsoft.Xna.Framework.Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;

        }

        /// <summary>
        /// Tests for collision with other bounding circle.
        /// </summary>
        /// <param name="other">The other bounding circle.</param>
        /// <returns>If it collides</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return PosTool.Collides(this, other);
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return PosTool.Collides(this, other);
        }
    }

    public struct BoundingRectangle
    {
        /// <summary>
        /// 
        /// </summary>
        public float X;
        /// <summary>
        /// 
        /// </summary>
        public float Y;
        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public float Width;
        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public float Height;
        public float Left => X;
        public float Right => X + Width;
        public float Top => Y;
        public float Bottom => Y + Height;

        public BoundingRectangle(Microsoft.Xna.Framework.Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;

            Width = width;
            Height = height;
        }

        public bool CollidesWith(BoundingRectangle other)
        {
            return PosTool.Collides(this, other);
        }

        public bool CollidesWith(BoundingCircle other)
        {
            return PosTool.Collides(this, other);
        }
    }


}
