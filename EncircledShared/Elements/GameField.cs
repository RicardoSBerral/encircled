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

		readonly CCDrawNode bottomAndSides;
		readonly CCDrawNode ceiling;
		readonly CCDrawNode limit;
		readonly Arrow arrow;

		readonly List<Orb> shooting;
		readonly List<Orb> shot;
		readonly List<Orb> nextOrb;
		readonly CCPoint nextOrbPosition;

		public CCPoint Head {
			get {
				return this.WorldToParentspace (this.ConvertToWorldspace (arrow.Head));
			}
		}

		public CCPoint Direction {
			get {
				return arrow.Direction;
			}
			set {
				var origin = this.WorldToParentspace (this.ConvertToWorldspace (arrow.Position));
				var sub = origin.Sub (ref value);
				arrow.Angle = CCMacros.CCRadiansToDegrees (sub.Angle);
				foreach (var orb in shooting) {
					orb.Position = arrow.Head;
					orb.Direction = arrow.Direction;
				}
			}
		}

		public GameField (float height, CCColor4F color, float width_proportion = 720f / 1280f)
		{

			// TAMAÑO TOTAL
			float width = height * width_proportion;
			this.ContentSize = new CCSize (width, height);

			// FONDO Y LADOS
			CCPoint corner1 = CCPoint.Zero;
			CCPoint corner2 = new CCPoint (width, 0f);
			CCPoint corner3 = new CCPoint (width, height);
			CCPoint corner4 = new CCPoint (0f, height);

			bottomAndSides = new CCDrawNode ();
			bottomAndSides.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			bottomAndSides.DrawSegment (corner2, corner3, LINE_WIDTH, color);
			bottomAndSides.DrawSegment (corner4, corner1, LINE_WIDTH, color);
			this.AddChild (bottomAndSides);

			ceiling = new CCDrawNode ();
			ceiling.DrawSegment (corner3, corner4, LINE_WIDTH, color);
			this.AddChild (ceiling);

			// LÍMITE
			float limit_height = height * LIMIT_PROPORTION;
			corner1 = new CCPoint (0f, limit_height);
			corner2 = new CCPoint (width, limit_height);

			limit = new CCDrawNode ();
			limit.DrawSegment (corner1, corner2, LINE_WIDTH, color);
			this.AddChild (limit);

			// FLECHA
			float arrow_length = limit_height * ARROW_PROPORTION;
			corner1 = new CCPoint (width / 2, 0f);

			arrow = new Arrow (arrow_length, color) {
				Position = corner1,
				AnchorPoint = CCPoint.Zero
			};
			this.AddChild (arrow);

			// ORBES
			shooting = new List<Orb> ();
			shot = new List<Orb> ();
			nextOrb = new List<Orb> ();
			nextOrbPosition = new CCPoint (width * 4 / 5, limit_height * 1 / 2);
		}

		public void Shoot (float growing_time, float width_to_orb_proportion = 0.05f)
		{
			Orb orb;
			Orb newOrb;
			// Primera bola
			if (!nextOrb.Any ()) {
				orb = new Orb (this.ContentSize.Width * width_to_orb_proportion);
				newOrb = new Orb (this.ContentSize.Width * width_to_orb_proportion);
			} else {
				orb = nextOrb [0];
				nextOrb.Remove (orb);
				newOrb = new Orb (this.ContentSize.Width * width_to_orb_proportion);
			}

			shooting.Add (orb);
			nextOrb.Add (newOrb);
			orb.Direction = arrow.Direction;

			CCFiniteTimeAction[] actions = new CCFiniteTimeAction [3];
			actions [0] = Orb.Teletransport (arrow.Head, growing_time);
			// TODO actions [1] = new CCDelayTime (growing_time);
			actions [1] = Orb.Shoot (this.ContentSize.Height);
			actions [2] = new CCCallFunc (
				() => {
					// this.RemoveChild(shooting);
					shooting.Remove (orb);
					shot.Add (orb);
				}
			);

			var sequence = new CCSequence (actions);

			orb.RunAction (sequence);

			newOrb.Position = nextOrbPosition;
			newOrb.RunAction (Orb.Grow (growing_time));

			this.AddChild (orb);
			this.AddChild (newOrb);
		}

		public void CheckOrbsCollision ()
		{
			foreach (var orb in shot) {
				var box1 = orb.BoundingBoxTransformedToParent;
				var box2 = bottomAndSides.BoundingBoxTransformedToParent;
				if (orb.BoundingBoxTransformedToParent.IntersectsRect (bottomAndSides.BoundingBoxTransformedToParent)) {
					orb.StopAllActions ();
					orb.Direction = orb.Direction.InvertX;
					orb.RunAction (Orb.Shoot (this.ContentSize.Height));
				}
			}
		}
	}
}

