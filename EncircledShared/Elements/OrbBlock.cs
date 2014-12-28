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
	public class OrbBlock : CCNode
	{
		readonly List<Orb> orbs;
		public OrbBlock ()
		{
			orbs = new List<Orb> ();
		}
	}
}

