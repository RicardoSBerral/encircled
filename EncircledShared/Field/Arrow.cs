﻿using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled.Field
{
	public class Arrow : CCNode
	{
		public const float SIDE_PROPORTION = 0.2f;
		public const int MAX_ANGLE_DEFAULT = 40;

		readonly float size;

		readonly CCDrawNode arrow;
		readonly float max_angle;

		public float Angle {
			set {
				value = - value - 90;
				if (value < -max_angle && value >= -180) {
					arrow.Rotation = -max_angle;
				} else if (value < -max_angle && value < -180 || value > max_angle) {
					arrow.Rotation = max_angle;
				} else {
					arrow.Rotation = value;
				}
			}
			get {
				return - (arrow.RotationX - 90);
			}
		}
		public CCVector2 Direction {
			get {
				var angleRadian = CCMacros.CCDegreesToRadians(this.Angle);
				return new CCPoint (
					Convert.ToSingle (Math.Cos (angleRadian)), 
					Convert.ToSingle (Math.Sin (angleRadian))
				);
			}
		}
		public CCPoint Head {
			get {
				var headFromZero = this.Direction * size;
				return this.WorldToParentspace(this.ConvertToWorldspace(headFromZero));
			}
		}

		public Arrow (float size, CCColor4F color, float line_width, float max_angle = MAX_ANGLE_DEFAULT)
		{
		
			// TAMAÑO TOTAL
			this.size = size;
			float side_X_projection = size * SIDE_PROPORTION;
			this.ContentSize = new CCSize (side_X_projection * 2, size);
			this.max_angle = max_angle;

			// MARCO
			CCPoint start = CCPoint.Zero;
			CCPoint end = new CCPoint (0f, size);
			CCPoint sideA = new CCPoint (side_X_projection, size - side_X_projection);
			CCPoint sideB = new CCPoint (- side_X_projection, size - side_X_projection);

			arrow = new CCDrawNode ();
			arrow.DrawSegment (start, end, line_width, color);
			arrow.DrawSegment (end, sideA, line_width, color);
			arrow.DrawSegment (end, sideB, line_width, color);
			arrow.AnchorPoint = CCPoint.Zero;
			this.AddChild (arrow);
		}
	}
}