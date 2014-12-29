using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace Encircled
{
	public class Arrow : CCNode
	{
		const float LINE_WIDTH = 1f;
		const float SIDE_PROPORTION = 0.2f;

		readonly CCDrawNode arrow;
		public float Angle {
			set {
				if (value < -70 && value >= -180) {
					arrow.Rotation = -70;
				} else if (value < -70 && value < -180 || value > 70) {
					arrow.Rotation = 70;
				} else {
					arrow.Rotation = value;
				}
			}
		}

		public Arrow (float size, CCColor4F color)
		{

			// TAMAÑO TOTAL
			float side_X_projection = size * SIDE_PROPORTION;
			this.ContentSize = new CCSize (side_X_projection * 2, size);

			// MARCO
			CCPoint start = CCPoint.Zero;
			CCPoint end = new CCPoint (0f, size);
			CCPoint sideA = new CCPoint (side_X_projection, size - side_X_projection);
			CCPoint sideB = new CCPoint (- side_X_projection, size - side_X_projection);

			arrow = new CCDrawNode ();
			arrow.DrawSegment (start, end, LINE_WIDTH, color);
			arrow.DrawSegment (end, sideA, LINE_WIDTH, color);
			arrow.DrawSegment (end, sideB, LINE_WIDTH, color);
			arrow.AnchorPoint = CCPoint.Zero;
			this.AddChild (arrow);
		}
	}
}