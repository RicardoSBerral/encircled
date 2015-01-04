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
		public const float LINE_WIDTH = 1f;
		public const float LIMIT_PROPORTION = 0.15f;

		private readonly CCSize playSize;
		private readonly CCSize startSize;

		public CCSize PlaySize { get { return playSize; } }
		public CCSize StartSize { get { return startSize; } }

		private readonly b2Body sides;
		private readonly b2Body bottom;
		private readonly b2Body limit;

		public Frame (float height, CCColor4F color, b2World world, float width_proportion = 720f / 1280f)
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
			frame.DrawSegment (corner2, corner3, LINE_WIDTH, color);
			frame.DrawSegment (corner4, corner1, LINE_WIDTH, color);
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
			frame.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			var bottomS = new b2EdgeShape ();
			bottomS.Set (sidesV [1], sidesV [2]);
			var bottomF = new b2FixtureDef ();
			bottomF.shape = bottomS;
			bottomF.friction = 0f;
			bottomF.restitution = 0f;
			var bottomD = new b2BodyDef ();
			bottomD.type = b2BodyType.b2_staticBody;
			bottomD.position.Set (0f, 0f);
			bottom = world.CreateBody (bottomD);
			sides.CreateFixture (bottomF);

			// TECHO
			frame.DrawSegment (corner3, corner4, LINE_WIDTH, color);

			// LÍMITE
			frame.DrawSegment (corner5, corner6, LINE_WIDTH, color);
			b2Vec2[] limitV = new b2Vec2[2];
			limitV [0] = new b2Vec2 (corner5.X / GameLayer.PTM_RATIO, corner5.Y / GameLayer.PTM_RATIO);
			limitV [1] = new b2Vec2 (corner6.X / GameLayer.PTM_RATIO, corner6.Y / GameLayer.PTM_RATIO);
			var limitS = new b2ChainShape ();
			limitS.CreateChain (limitV, 2);
			var limitF = new b2FixtureDef ();
			limitF.shape = limitS;
			limitF.isSensor = true;
			var limitD = new b2BodyDef ();
			limitD.type = b2BodyType.b2_staticBody;
			limitD.position.Set (0f, 0f);
			limit = world.CreateBody (limitD);
			limit.CreateFixture (limitF);

			this.AddChild (frame);
		}
	}
}

