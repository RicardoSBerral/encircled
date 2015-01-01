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

		private readonly b2Body bottomAndSides;
//		private readonly b2Body ceiling;
		private readonly b2Body limit;

		public Frame (float height, CCColor4F color, b2World world, float width_proportion = 720f / 1280f)
		{

			// TAMAÑO TOTAL
			var width = height * width_proportion;
			var limit_height = height * LIMIT_PROPORTION;

			playSize = new CCSize (width, height - limit_height);
			startSize = new CCSize (width, limit_height);

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
			bottomAndSidesV [0] = new b2Vec2 (corner4.X / GameLayer.PTM_RATIO, corner4.Y / GameLayer.PTM_RATIO);
			bottomAndSidesV [1] = new b2Vec2 (corner1.X / GameLayer.PTM_RATIO, corner1.Y / GameLayer.PTM_RATIO);
			bottomAndSidesV [2] = new b2Vec2 (corner2.X / GameLayer.PTM_RATIO, corner2.Y / GameLayer.PTM_RATIO);
			bottomAndSidesV [3] = new b2Vec2 (corner3.X / GameLayer.PTM_RATIO, corner3.Y / GameLayer.PTM_RATIO);
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
//			b2Vec2[] ceilingV = new b2Vec2[2];
//			ceilingV [0] = new b2Vec2 (corner3.X / GameLayer.PTM_RATIO, corner3.Y / GameLayer.PTM_RATIO);
//			ceilingV [1] = new b2Vec2 (corner4.X / GameLayer.PTM_RATIO, corner4.Y / GameLayer.PTM_RATIO);
//			var ceilingS = new b2ChainShape ();
//			ceilingS.CreateChain (ceilingV, 2);
//			var ceilingF = new b2FixtureDef ();
//			ceilingF.shape = ceilingS;
//			ceilingF.isSensor = true;
//			var ceilingD = new b2BodyDef ();
//			ceilingD.type = b2BodyType.b2_staticBody;
//			ceilingD.position.Set (0f, 0f);
//			ceiling = world.CreateBody (ceilingD);
//			ceiling.CreateFixture (ceilingF);

			// LÍMITE
			corner1 = new CCPoint (0f, limit_height);
			corner2 = new CCPoint (width, limit_height);
			frame.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			b2Vec2[] limitV = new b2Vec2[2];
			limitV [0] = new b2Vec2 (corner1.X / GameLayer.PTM_RATIO, corner1.Y / GameLayer.PTM_RATIO);
			limitV [1] = new b2Vec2 (corner2.X / GameLayer.PTM_RATIO, corner2.Y / GameLayer.PTM_RATIO);
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

		public FramePart CheckCollisionOrb (Orb orb)
		{
			// TODO
			return FramePart.None;
		}
	}
}

