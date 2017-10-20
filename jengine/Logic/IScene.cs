using jengine.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace jengine.Logic
{
	public interface IScene
	{
		void Load(IRender render);

		void Update(double delta);

		void Draw(double delta, IRender render);

		void Unload();
	}
}