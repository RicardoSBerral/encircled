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
		const float ARROW_PROPORTION = 0.8f;

		readonly Frame frame;
		readonly Arrow arrow;
		readonly b2World world;

		readonly List<Orb> shooting;
		readonly List<Orb> shot;
		readonly List<Orb> nextOrb;
		readonly CCPoint nextOrbPosition;
		readonly float width_to_orb_proportion;

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

		public GameField (float height, CCColor4F color, b2World world, float width_proportion = 720f / 1280f, float width_to_orb_proportion = 0.05f)
		{

			this.width_to_orb_proportion = width_to_orb_proportion;
			this.world = world;

			// MARCO
			frame = new Frame (height, color, world, width_proportion);
			this.AddChild (frame);

			// FLECHA
			float arrow_length = frame.Limit_Height * ARROW_PROPORTION;
			var arrow_origin = new CCPoint (frame.Width / 2, 0f);

			arrow = new Arrow (arrow_length, color) {
				Position = arrow_origin,
				AnchorPoint = CCPoint.Zero
			};
			this.AddChild (arrow);

			// ORBES
			shooting = new List<Orb> ();
			shot = new List<Orb> ();
			nextOrb = new List<Orb> ();
			nextOrbPosition = new CCPoint (frame.Width * 4 / 5, frame.Limit_Height * 1 / 2);
		}

		public void Shoot (float growing_time)
		{
			Orb orb;
			Orb newOrb;
			// Primera bola
			if (!nextOrb.Any ()) {
				orb = new Orb (this.frame.Width * width_to_orb_proportion, world);
				newOrb = new Orb (this.frame.Width * width_to_orb_proportion, world);
			} else {
				orb = nextOrb [0];
				nextOrb.Remove (orb);
				newOrb = new Orb (this.frame.Width * width_to_orb_proportion, world);
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
			// TODO
			foreach (var orb in shot) {
				var collision = frame.CheckCollisionOrb (orb);
				switch (collision) {
				default:
					break;
				}
			}
		}

		public void UpdateOrbs() {
			foreach (var orb in this.shooting) {
				orb.UpdateOrb ();
			}
			foreach (var orb in this.shot) {
				orb.UpdateOrb ();
			}
		}
	}
}