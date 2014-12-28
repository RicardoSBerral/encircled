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
		const float RADIUS = 10f;
		const float ORB_SPEED = 350f;
		const float GROWING_TIME = 0.2f;

		readonly CCDrawNode node;
		readonly CCParticleSun sun;

		public Orb (CCPoint location)
		{
			// Dibujar círculo
			node = new CCDrawNode ();
			CCColor4B color = colors [CCRandom.Next (colors.Count)];
			node.DrawSolidCircle (CCPoint.Zero, RADIUS, color);
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

			// Posición
			this.Position = location;

			// Animación
			this.Scale = 0f;
			var grow = new CCScaleTo (GROWING_TIME, 1);
			this.RunAction (grow);
		}

		public void Shoot (CCPoint origin, CCPoint direction)
		{
			this.StopAllActions ();
			float ds = CCPoint.Distance (this.Position, direction);

			var dt = ds / ORB_SPEED;

			var moveOrb = new CCMoveTo (dt, direction);
			// CCAction changeSize = new CC;
			this.RunAction (moveOrb);
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

