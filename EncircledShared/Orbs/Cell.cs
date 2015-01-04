using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;

using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision;
using Box2D.Collision.Shapes;
using Box2D.Dynamics.Joints;

namespace Encircled.Orbs
{
	public class Cell
	{
		// Parámetros
		private CCPoint position;

		// Almacenamiento
		private Orb currentOrb;

		// Propiedades
		public int Y { get; set; }
		public int X { get; set; }
		public CCPoint Position { 
			get { return position; } 
			set {
				if (!Empty) {
					currentOrb.Position = value;
				}
				position = value;
			}
		}
		public bool Empty { get { return currentOrb == null; } }
		public Orb CurrentOrb { 
			get {
				if (!Empty) {
					return currentOrb;
				} else {
					throw new AccessViolationException ("This cell doesn't house any orb.");
				}
			}
			set {
				if (Empty) {
					currentOrb = value;
					value.Position = position;
					currentOrb.Freeze (position);
				} else {
						throw new AccessViolationException ("This cell already houses an orb, of id " + currentOrb.Id + ".");
				}
			}
		}

		public Cell (int y, int x, CCPoint position)
		{
			this.Y = y;
			this.X = x;
			Position = position;
			currentOrb = null;
		}

		public Orb DestroyOrb ()
		{
			if (!Empty) {
				var orb = currentOrb;
				currentOrb = null;
				orb.Destroy ();
				return orb;
			} else {
				throw new AccessViolationException ("This cell doesn't house any orb.");
			}
		}

		public Orb LetFall ()
		{
			if (!Empty) {
				var orb = currentOrb;
				currentOrb = null;
				orb.Fall ();
				return orb;
			} else {
				throw new AccessViolationException ("This cell doesn't house any orb.");
			}
		}
	}

	public class CellComparer : EqualityComparer<Cell>
	{
		public override bool Equals(Cell b1, Cell b2)
		{
			return b1.X == b2.X && b1.Y == b2.Y;
		}

		public override int GetHashCode(Cell bx)
		{
			int suma = bx.X * 100 + bx.Y;
			return suma.GetHashCode();
		}
	}
}

