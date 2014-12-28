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
	public class NuevoJuegoScene : CCScene
	{
		public NuevoJuegoScene (CCWindow mainWindow) : base(mainWindow)
		{
			NuevoJuegoLayer layer = new NuevoJuegoLayer ();

			this.AddChild (layer);
		}
	}
}

