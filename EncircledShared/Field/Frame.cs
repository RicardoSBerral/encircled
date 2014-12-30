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
	public enum FramePart {BottomAndSides, Ceiling, Limit, None};
	public class Frame : CCNode
	{
		const float LINE_WIDTH = 1f;
		const float LIMIT_PROPORTION = 0.15f;

		private readonly float width;
		private readonly float height;
		private readonly float limit_height;

		public float Width { get { return width; } }
		public float Height { get { return height; } }
		public float Limit_Height { get { return limit_height; } }

		readonly b2Body bottomAndSides;
		readonly b2Body ceiling;
		readonly b2Body limit;

		public Frame (float height, CCColor4F color, b2World world, float width_proportion = 720f / 1280f)
		{

			// TAMAÑO TOTAL
			this.height = height;
			width = height * width_proportion;
			limit_height = height * LIMIT_PROPORTION;
			this.ContentSize = new CCSize (width, height);

			var frame = new CCDrawNode ();
			// FONDO Y LADOS
			CCPoint corner1 = CCPoint.Zero;
			CCPoint corner2 = new CCPoint (width, 0f);
			CCPoint corner3 = new CCPoint (width, height);
			CCPoint corner4 = new CCPoint (0f, height);
			frame.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			frame.DrawSegment (corner2, corner3, LINE_WIDTH, color);
			frame.DrawSegment (corner4, corner1, LINE_WIDTH, color);
			b2Vec2[] bottomAndSidesV = new b2Vec2[4];
			bottomAndSidesV [0] = new b2Vec2 (corner4.X, corner4.Y);
			bottomAndSidesV [1] = new b2Vec2 (corner1.X, corner1.Y);
			bottomAndSidesV [2] = new b2Vec2 (corner2.X, corner2.Y);
			bottomAndSidesV [3] = new b2Vec2 (corner3.X, corner3.Y);
			var bottomAndSidesS = new b2ChainShape ();
			bottomAndSidesS.CreateChain (bottomAndSidesV, 4);
			var bottomAndSidesF = new b2FixtureDef ();
			bottomAndSidesF.shape = bottomAndSidesS;
			var bottomAndSidesD = new b2BodyDef ();
			bottomAndSidesD.type = b2BodyType.b2_staticBody;
			bottomAndSidesD.position.Set (0f, 0f);
			bottomAndSides = world.CreateBody (bottomAndSidesD);
			bottomAndSides.CreateFixture (bottomAndSidesF);

			// TECHO
			frame.DrawSegment (corner3, corner4, LINE_WIDTH, color);
			b2Vec2[] ceilingV = new b2Vec2[2];
			ceilingV [0] = new b2Vec2 (corner3.X, corner3.Y);
			ceilingV [1] = new b2Vec2 (corner4.X, corner4.Y);
			var ceilingS = new b2ChainShape ();
			ceilingS.CreateChain (ceilingV, 2);
			var ceilingF = new b2FixtureDef ();
			ceilingF.shape = ceilingS;
			var ceilingD = new b2BodyDef ();
			ceilingD.type = b2BodyType.b2_staticBody;
			ceilingD.position.Set (0f, 0f);
			ceiling = world.CreateBody (ceilingD);
			ceiling.CreateFixture (ceilingF);

			// LÍMITE
			corner1 = new CCPoint (0f, limit_height);
			corner2 = new CCPoint (width, limit_height);
			frame.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			b2Vec2[] limitV = new b2Vec2[2];
			limitV [0] = new b2Vec2 (corner1.X, corner1.Y);
			limitV [1] = new b2Vec2 (corner2.X, corner2.Y);
			var limitS = new b2ChainShape ();
			limitS.CreateChain (limitV, 2);
			var limitF = new b2FixtureDef ();
			limitF.shape = limitS;
			var limitD = new b2BodyDef ();
			limitD.type = b2BodyType.b2_staticBody;
			limitD.position.Set (0f, 0f);
			limit = world.CreateBody (limitD);
			limit.CreateFixture (limitF);

			this.AddChild (frame);
		}

		public FramePart CheckCollisionOrb (Orb orb)
		{
			// TODO
			return FramePart.None;
		}
	}
}

