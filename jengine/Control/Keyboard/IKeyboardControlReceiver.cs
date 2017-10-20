using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jengine.Control.Keyboard
{
	public interface IKeyboardControlReceiver
	{
		void KeyPress(KeyPressEventArgs e);

		void KeyUp(KeyboardKeyEventArgs e);

		void KeyDown(KeyboardKeyEventArgs e);
	}
}