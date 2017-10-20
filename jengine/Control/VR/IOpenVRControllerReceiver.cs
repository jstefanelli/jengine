using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valve.VR;

namespace jengine.Control.VR
{
	public interface IOpenVRControllerReceiver
	{
		void OnButtonTouch(VREvent_t EventParams);

		void OnButtonUnTouch(VREvent_t EvemtParams);

		void OnButtonPress(VREvent_t EventParams);

		void OnButtonUnPress(VREvent_t EventParams);
	}
}