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
	public class Orb : CCNode
	{
		readonly static List<CCColor4B> colors = new List<CCColor4B> {
			CCColor4B.Red,
			CCColor4B.Blue,
			CCColor4B.Yellow,
			CCColor4B.Gray,
			CCColor4B.Green
		};

		readonly CCDrawNode node;
		readonly CCParticleSun sun;

		readonly float orb_speed;
		public float Radius { get; set; }
		// TODO readonly Dictionary<string,CCAction> actions;
		public CCPoint Direction { get; set; }

		public Orb (float radius, float orb_speed = 50f)
		{
			// Copiar parámetros
			this.Radius = radius;
			this.orb_speed = orb_speed;

			// Dibujar círculo
			node = new CCDrawNode ();
			CCColor4B color = colors [CCRandom.Next (colors.Count)];
			node.DrawSolidCircle (CCPoint.Zero, radius, color);
			this.AddChild (node);

			// Dibujar brillo
			if (color == CCColor4B.Gray) {
				sun = new CCParticleSun (CCPoint.Zero);
				sun.StartColor = new CCColor4F (CCColor3B.Red);
				sun.EndColor = new CCColor4F (CCColor3B.Black);
				this.AddChild (sun);
			} else {
				sun = null;
			}

			// TODO Variables
			// actions = new Dictionary<string, CCAction> ();
		}

		public static CCFiniteTimeAction Grow (float growing_time = 0.2f)
		{
			return new CCSequence (new CCScaleTo (0f, 0f), new CCScaleTo (growing_time, 1f));
		}

		public static CCFiniteTimeAction Shoot (float distance)
		{
			return new CCCallFuncN( (node) => {
				var orb = (Orb) node;
				var dt = distance / orb.orb_speed;
				orb.RunAction(new CCMoveTo(dt, orb.Position + orb.Direction * distance));
			});
		}

		public static CCFiniteTimeAction Teletransport (CCPoint position, float time = 0.2f) {
			return new CCMoveTo (0f, position);
//			CCFiniteTimeAction[] actions = new CCFiniteTimeAction[3];
//			// TODO CCEaseElastic
//			actions [0] = new CCFadeOut (time / 2);
//			actions [1] = new CCMoveTo (0f, position);
//			actions [2] = new CCFadeIn (time / 2);
//			return new CCSequence (actions);
		}

		public void TrembleStart ()
		{
		}

		public void TrembleStop ()
		{
		}

		public CCParticleExplosion Explode ()
		{
			this.RemoveFromParent();
			var explosion = new CCParticleExplosion (this.Position);
			explosion.TotalParticles = CCRandom.Next(8, 12);
			explosion.AutoRemoveOnFinish = true;
			return explosion;
		}
	}
}

