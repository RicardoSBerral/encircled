using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

using Encircled.Orbs;

namespace Encircled.Field
{
	public enum FramePart {BottomAndSides, Ceiling, Limit, None};
	public class Frame : CCNode
	{
		public const float LIMIT_PROPORTION = 0.15f;

		private readonly CCSize playSize;
		private readonly CCSize startSize;

		public CCSize PlaySize { get { return playSize; } }
		public CCSize StartSize { get { return startSize; } }

		private readonly b2Body sides;

		public Frame (float height, CCColor4F color, b2World world, float line_width, float width_proportion = 720f / 1280f)
		{

			// TAMAÑO TOTAL
			var width = height * width_proportion;
			var limit_height = height * LIMIT_PROPORTION;

			playSize = new CCSize (width, height - limit_height);
			startSize = new CCSize (width, limit_height);
			CCPoint corner1 = CCPoint.Zero;
			CCPoint corner2 = new CCPoint (width, 0f);
			CCPoint corner3 = new CCPoint (width, height);
			CCPoint corner4 = new CCPoint (0f, height);
			CCPoint corner5 = new CCPoint (0f, limit_height);
			CCPoint corner6 = new CCPoint (width, limit_height);
			var frame = new CCDrawNode ();

			// LADOS
			frame.DrawSegment (corner2, corner3, line_width, color);
			frame.DrawSegment (corner4, corner1, line_width, color);
			b2Vec2[] sidesV = new b2Vec2[4];
			sidesV [0] = new b2Vec2 (corner4.X / GameLayer.PTM_RATIO, corner4.Y / GameLayer.PTM_RATIO);
			sidesV [1] = new b2Vec2 (corner1.X / GameLayer.PTM_RATIO, corner1.Y / GameLayer.PTM_RATIO);
			sidesV [2] = new b2Vec2 (corner2.X / GameLayer.PTM_RATIO, corner2.Y / GameLayer.PTM_RATIO);
			sidesV [3] = new b2Vec2 (corner3.X / GameLayer.PTM_RATIO, corner3.Y / GameLayer.PTM_RATIO);
			var sidesS1 = new b2EdgeShape ();
			sidesS1.Set (sidesV [0], sidesV [1]);
			var sidesS2 = new b2EdgeShape ();
			sidesS2.Set (sidesV [2], sidesV [3]);
			var sidesF1 = new b2FixtureDef ();
			sidesF1.shape = sidesS1;
			var sidesF2 = new b2FixtureDef ();
			sidesF2.shape = sidesS2;
			var sidesD = new b2BodyDef ();
			sidesD.type = b2BodyType.b2_staticBody;
			sidesD.position.Set (0f, 0f);
			sides = world.CreateBody (sidesD);
			sides.CreateFixture (sidesF1);
			sides.CreateFixture (sidesF2);

			// FONDO
			frame.DrawSegment (corner1, corner2, line_width, color);

			// TECHO
			frame.DrawSegment (corner3, corner4, line_width, color);

			// LÍMITE
			frame.DrawSegment (corner5, corner6, line_width, color);

			this.AddChild (frame);
		}
	}
}

