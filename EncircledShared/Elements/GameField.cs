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
	public class GameField : CCNode
	{
		const float LINE_WIDTH = 1f;
		const float LIMIT_PROPORTION = 0.15f;
		const float ARROW_PROPORTION = 0.8f;

		readonly CCDrawNode frame;
		readonly CCDrawNode limit;
		readonly Arrow arrow;

		public GameField (float height, CCColor4F color, float width_proportion = 720f / 1280f)
		{

			// TAMAÑO TOTAL
			float width = height * width_proportion;
			this.ContentSize = new CCSize (width, height);

			// MARCO
			CCPoint corner1 = CCPoint.Zero;
			CCPoint corner2 = new CCPoint (width, 0f);
			CCPoint corner3 = new CCPoint (width, height);
			CCPoint corner4 = new CCPoint (0f, height);

			frame = new CCDrawNode ();
			frame.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			frame.DrawSegment (corner2, corner3, LINE_WIDTH, color);
			frame.DrawSegment (corner3, corner4, LINE_WIDTH, color);
			frame.DrawSegment (corner4, corner1, LINE_WIDTH, color);
			this.AddChild (frame);

			// LÍMITE
			float limit_height = height * LIMIT_PROPORTION;
			corner1 = new CCPoint (0f, limit_height);
			corner2 = new CCPoint (width, limit_height);

			limit = new CCDrawNode ();
			limit.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			this.AddChild (limit);

			// FLECHA
			float arrow_length = limit_height * ARROW_PROPORTION;
			corner1 = new CCPoint(width / 2, 0f);

			arrow = new Arrow (arrow_length, color)
			{
				Position = corner1,
				AnchorPoint = CCPoint.Zero
			};
			this.AddChild (arrow);
		}

		public void Aim(CCPoint direction)
		{
			CCPoint origin = new CCPoint (arrow.Position);
			arrow.Direction = direction.Sub(ref origin);
		}
	}
}

