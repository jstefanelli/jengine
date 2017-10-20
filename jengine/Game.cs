using jengine.Control.Keyboard;
using Valve.VR;
using jengine.Control.VR;
using jengine.Logic;
using jengine.Render;
using jengine.Render.GL3;
using jengine.Settings;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jengine.Render.Shapes.VR;
using System.Runtime.InteropServices;

namespace jengine
{
	public class Game
	{
		public const string TAG = "Game";
		public static Game Instance { get; private set; }

		public static Game StartNewGL3(string SettingsPath, string ResourcePath, string Title, IScene startingScene)
		{
			if (Instance != null)
				throw new Exception("Cannot Start a new Game");
			if (File.Exists(SettingsPath))
			{
				Log.V(TAG, "Loading settings from file");
				Instance = new Game()
				{
					LoadedSettingsPath = SettingsPath,
					ResourcePath = ResourcePath,
					LoadedSettings = SettingsManager.LoadFromFile(SettingsPath)
				};
			}
			else
			{
				Log.V(TAG, "Loading Default Settings");
				Instance = new Game()
				{
					LoadedSettingsPath = SettingsPath,
					ResourcePath = ResourcePath,
					LoadedSettings = SettingsManager.Default
				};
			}
			Instance.StartGL3(Title, startingScene);
			return Instance;
		}

		public SettingsManager LoadedSettings { get; private set; }
		public string LoadedSettingsPath { get; private set; }
		public string ResourcePath { get; set; }
		public GameWindow Window { get; private set; }

		public int Width
		{
			get
			{
				return Window.Width;
			}
			set
			{
				if (!FullScreen)
				{
					Window.Width = value;
				}
			}
		}

		public int Height
		{
			get
			{
				return Window.Height;
			}
			set
			{
				if (!FullScreen)
				{
					Window.Height = value;
				}
			}
		}

		public bool VSync
		{
			get
			{
				return Window.VSync == VSyncMode.On;
			}
			set
			{
				Window.VSync = (value) ? VSyncMode.On : VSyncMode.Off;
				LoadedSettings.VSync = value;
			}
		}

		public bool FullScreen
		{
			get
			{
				return (Window.WindowState == WindowState.Fullscreen);
			}
			set
			{
				Window.WindowState = (value) ? WindowState.Fullscreen : WindowState.Normal;
				LoadedSettings.Fullscreen = value;
			}
		}

		#region OpenVR

		public class OpenVRSettings
		{
			public const string TAG = "Game/OpenVRSettings";

			public Matrix4[] ViewMatrices { get; private set; }
			public int FrameSizeX { get; private set; }
			public int FrameSizeY { get; private set; }
			public Vector3 LocalHeadPosition { get; private set; }
			public Quaternion LocalHeadOrientation { get; private set; }

			public OpenVRSettings()
			{
				uint sizeX = 0, sizeY = 0;
				Instance.Vr.GetRecommendedRenderTargetSize(ref sizeX, ref sizeY);
				Log.V(TAG, "Frame Size: " + sizeX + "x" + sizeY);
				FrameSizeX = (int)sizeX;
				FrameSizeY = (int)sizeY;
				Update();
			}

			public Matrix4 GetProjectionMatrixPerEye(EVREye eye, float zNear, float zFar)
			{
				return opentk_interop.ConvertTK(Instance.Vr.GetProjectionMatrix(eye, zNear, zFar));
			}

			public void Update()
			{
				ViewMatrices = new Matrix4[2]
				{
					opentk_interop.ConvertTK(Instance.Vr.GetEyeToHeadTransform(EVREye.Eye_Left)),
					opentk_interop.ConvertTK(Instance.Vr.GetEyeToHeadTransform(EVREye.Eye_Right))
				};
			}

			public void UpdateHead(Matrix4 mat)
			{
				LocalHeadPosition = mat.ExtractTranslation();
				LocalHeadOrientation = mat.ExtractRotation();
			}
		}

		public CVRSystem Vr { get; private set; }
		public Dictionary<uint, IVRTrackedDrawable> VrRenderModels { get; private set; }
		public IOpenVRControllerReceiver CurrentVRControllerReceiver { get; set; } = null;
		public TrackedDevicePose_t[] VrPoses { get; private set; }
		public TrackedDevicePose_t[] VrGamePoses { get; private set; }
		public OpenVRSettings VrSettings { get; private set; }

		private void LoadOpenVR()
		{
			VrPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
			VrGamePoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
			VrRenderModels = new Dictionary<uint, IVRTrackedDrawable>();
			EVRInitError initError = EVRInitError.None;
			Vr = OpenVR.Init(ref initError, EVRApplicationType.VRApplication_Scene);
			VrSettings = new OpenVRSettings();
			for (uint i = 1; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
			{
				if (Vr.IsTrackedDeviceConnected(i))
				{
					StringBuilder b = new StringBuilder(256);
					ETrackedPropertyError err = ETrackedPropertyError.TrackedProp_Success;

					int deviceClass = Vr.GetInt32TrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceClass_Int32, ref err);
					ETrackedDeviceClass DeviceClass = (ETrackedDeviceClass)deviceClass;
					if (err != ETrackedPropertyError.TrackedProp_Success)
					{
#if DEBUG
						Log.W(TAG, "OpenVR Error: " + err.ToString());
#endif
						continue;
					}

					if (DeviceClass == ETrackedDeviceClass.Controller || DeviceClass == ETrackedDeviceClass.GenericTracker || (LoadedSettings.OpenVRDisplayLighthouses && DeviceClass == ETrackedDeviceClass.TrackingReference))
					{
						IVRTrackedDrawable d = Render.ShapeFactory.MakeRenderModel(i);
						d.Active = true;
						VrRenderModels.Add(i, d);
					}
				}
			}
		}

		private void UpdateOpenVR()
		{
			VREvent_t ev = new VREvent_t();
			while (Vr.PollNextEvent(ref ev, (uint)Marshal.SizeOf(typeof(VREvent_t))))
			{
				switch ((EVREventType)ev.eventType)
				{
					case EVREventType.VREvent_TrackedDeviceActivated:
					case EVREventType.VREvent_TrackedDeviceUpdated:
						uint index = ev.trackedDeviceIndex;
						if (VrRenderModels.ContainsKey(index))
						{
							VrRenderModels[index].Active = true;
						}
						else
						{
							Render.ShapeFactory.MakeRenderModel(index);
						}
						Log.V(TAG, "New Device: " + index);
						break;

					case EVREventType.VREvent_TrackedDeviceDeactivated:
						if (VrRenderModels.ContainsKey(ev.trackedDeviceIndex))
							VrRenderModels[ev.trackedDeviceIndex].Active = false;
						break;

					case EVREventType.VREvent_Quit:
						Window.Close();
						break;

					//TODO: Add Press and Unpress callbacks

					case EVREventType.VREvent_ButtonTouch:
						CurrentVRControllerReceiver?.OnButtonTouch(ev);
						break;

					case EVREventType.VREvent_ButtonUntouch:
						CurrentVRControllerReceiver?.OnButtonUnTouch(ev);
						break;

					default:
						Log.V(TAG, "OpenVR event: " + ((EVREventType)ev.eventType).ToString());
						break;
				}
			}
		}

		private void UpdateOpenVRPositions()
		{
			OpenVR.Compositor.WaitGetPoses(VrPoses, VrGamePoses);

			for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
			{
				if (VrGamePoses[i].bPoseIsValid && VrGamePoses[i].bDeviceIsConnected)
				{
					if (i == 0)
					{
						VrSettings?.UpdateHead(opentk_interop.ConvertTK(VrGamePoses[i].mDeviceToAbsoluteTracking));
						continue;
					}
					if (VrRenderModels.ContainsKey(i))
					{
						IVRTrackedDrawable p = VrRenderModels[i];
						p.Active = true;
						Matrix4 newMat = opentk_interop.ConvertTK(VrGamePoses[i].mDeviceToAbsoluteTracking);
						Vector3 position = newMat.ExtractTranslation();
						Quaternion orientation = newMat.ExtractRotation();
						p.TrackedPosition = position;
						p.TrackedOrientation = orientation;
					}
				}
			}
		}

		#endregion OpenVR

		private IRender Render { get; set; }

		private IScene CurrentScene = null;

		public IScene NextScene { get; set; } = null;

		public IKeyboardControlReceiver CurrentKeyboardReceiver { get; set; } = null;

		private Game()
		{
		}

		public void StartGL3(string title, IScene startingScene)
		{
			Render = new GL3Render();
			Window = new GameWindow(LoadedSettings.WindowResolutionX, LoadedSettings.WindowResolutionY, new GraphicsMode(new ColorFormat(32), 16, 0), title)
			{
				VSync = (LoadedSettings.VSync) ? VSyncMode.On : VSyncMode.Off,
				WindowState = (LoadedSettings.Fullscreen) ? WindowState.Fullscreen : WindowState.Normal
			};
			Window.Load += Load;
			Window.UpdateFrame += Update;
			Window.RenderFrame += Draw;
			Window.Resize += Resize;
			Window.Closing += Closing;
			Window.KeyDown += KeyDown;
			Window.KeyUp += KeyUp;
			Window.KeyPress += KeyPress;
			CurrentScene = startingScene;
			Log.G(TAG, "Jengine Loading. ");
			Log.G(TAG, "WinSize: " + LoadedSettings.WindowResolutionX + "x" + LoadedSettings.WindowResolutionY);
			Log.G(TAG, "FrameSize: " + LoadedSettings.FrameResolutionX + "x" + LoadedSettings.FrameResolutionY);
			Log.G(TAG, "OpenVR: " + (LoadedSettings.OpenVREnabled ? "enabled" : "disabled"));
			int actualRefreshrate;
			if (LoadedSettings.OpenVREnabled)
			{
				actualRefreshrate = 90;
			}
			else
			{
				actualRefreshrate = LoadedSettings.RefreshRate;
			}
			Log.G(TAG, "Refreshrate (actual/config): " + actualRefreshrate + "/" + LoadedSettings.RefreshRate);
			Window.Run(60, actualRefreshrate);
		}

		#region Keyboard

		private void KeyPress(object sender, KeyPressEventArgs e)
		{
			CurrentKeyboardReceiver?.KeyPress(e);
		}

		private void KeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
		{
			CurrentKeyboardReceiver?.KeyUp(e);
		}

		private void KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
		{
			CurrentKeyboardReceiver?.KeyDown(e);
		}

		#endregion Keyboard

		private void Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (LoadedSettings.OpenVREnabled)
				OpenVR.Shutdown();
			CurrentScene?.Unload();
			Render?.Unload();
			LoadedSettings.SaveToFIle(LoadedSettingsPath);
		}

		private void Resize(object sender, EventArgs e)
		{
			if (!FullScreen)
			{
				LoadedSettings.WindowResolutionX = Window.Width;
				LoadedSettings.WindowResolutionY = Window.Height;
			}
			Log.V(TAG, "Resized to: " + Width + "x" + Height);
			if (Render?.Loaded == true)
				Render.Resize(Width, Height);
		}

		private void Draw(object sender, FrameEventArgs e)
		{
			if (LoadedSettings.OpenVREnabled)
				UpdateOpenVRPositions();
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			if (NextScene != null)
			{
				CurrentScene?.Unload();
				CurrentScene = NextScene;
				NextScene = null;
				CurrentScene.Load(Render);
			}
			CurrentScene?.Draw(e.Time, Render);
			Window.SwapBuffers();
		}

		private void Update(object sender, FrameEventArgs e)
		{
			if (LoadedSettings.OpenVREnabled)
			{
				UpdateOpenVR();
				VrSettings?.Update();
			}
			Render?.Update();
			CurrentScene?.Update(e.Time);
		}

		private void Load(object sender, EventArgs e)
		{
			Render?.Load();
			if (LoadedSettings.OpenVREnabled)
			{
				LoadOpenVR();
			}
			CurrentScene?.Load(Render);
		}
	}
}