using ImGuiNET;
using UImGui.Assets;
using UnityEngine;
using System.Collections.Generic;

namespace UImGui.Platform
{
	// TODO: Check this feature and remove from here when checked and done.
	// Implemented features:
	// [x] Platform: Clipboard support.
	// [x] Platform: Mouse cursor shape and visibility. Disable with io.ConfigFlags |= ImGuiConfigFlags.NoMouseCursorChange.
	// [x] Platform: Keyboard arrays indexed using KeyCode codes, e.g. ImGui.IsKeyPressed(KeyCode.Space).
	// [ ] Platform: Gamepad support. Enabled with io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad.
	// [~] Platform: IME support.
	// [~] Platform: INI settings support.

	/// <summary>
	/// Platform bindings for ImGui in Unity in charge of: mouse/keyboard/gamepad inputs, cursor shape, timing, windowing.
	/// </summary>
	internal sealed class InputManagerPlatform : PlatformBase
	{
		private readonly Event _textInputEvent = new Event();

		private Dictionary<KeyCode, ImGuiKey> _mainKeys = new();

		public InputManagerPlatform(CursorShapesAsset cursorShapes, IniSettingsAsset iniSettings) :
			base(cursorShapes, iniSettings)
		{ }

		public override bool Initialize(ImGuiIOPtr io, UIOConfig config, string platformName)
		{
			base.Initialize(io, config, platformName);

			SetupKeyboard(io);

			return true;
		}

		public override void PrepareFrame(ImGuiIOPtr io, Rect displayRect)
		{
			base.PrepareFrame(io, displayRect);

			UpdateKeyboard(io);
			UpdateMouse(io);
			UpdateCursor(io, ImGui.GetMouseCursor());
		}

		private void SetupKeyboard(ImGuiIOPtr io)
		{
			// Map and store new keys by assigning io.KeyMap and setting value of array
			_mainKeys = new() {
				{ KeyCode.A, ImGuiKey.A }, // For text edit CTRL+A: select all.
				{ KeyCode.C, ImGuiKey.C }, // For text edit CTRL+C: copy.
				{ KeyCode.V, ImGuiKey.V }, // For text edit CTRL+V: paste.
				{ KeyCode.X, ImGuiKey.X }, // For text edit CTRL+X: cut.
				{ KeyCode.Y, ImGuiKey.Y }, // For text edit CTRL+Y: redo.
				{ KeyCode.Z, ImGuiKey.Z }, // For text edit CTRL+Z: undo.

				{ KeyCode.Tab, ImGuiKey.Tab },

				{ KeyCode.LeftArrow, ImGuiKey.LeftArrow },
				{ KeyCode.RightArrow, ImGuiKey.RightArrow },
				{ KeyCode.UpArrow, ImGuiKey.UpArrow },
				{ KeyCode.DownArrow, ImGuiKey.DownArrow },

				{ KeyCode.PageUp, ImGuiKey.PageUp },
				{ KeyCode.PageDown, ImGuiKey.PageDown },

				{ KeyCode.Home, ImGuiKey.Home },
				{ KeyCode.End, ImGuiKey.End },
				{ KeyCode.Insert, ImGuiKey.Insert },
				{ KeyCode.Delete, ImGuiKey.Delete },
				{ KeyCode.Backspace, ImGuiKey.Backspace },

				{ KeyCode.Space, ImGuiKey.Space },
				{ KeyCode.Escape, ImGuiKey.Escape },
				{ KeyCode.Return, ImGuiKey.Enter },
				{ KeyCode.KeypadEnter, ImGuiKey.KeypadEnter }
			};
		}

		private void UpdateKeyboard(ImGuiIOPtr io)
		{
			foreach (var key in _mainKeys)
			{
				if (Input.GetKey(key.Key))
				{
					io.AddKeyEvent(key.Value, true);
				}
			}

			// Keyboard modifiers.
			io.KeyShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			io.KeyCtrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			io.KeyAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			io.KeySuper = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) ||
				Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows);

			// Text input.
			while (Event.PopEvent(_textInputEvent))
			{
				if (_textInputEvent.rawType == EventType.KeyDown &&
					_textInputEvent.character != 0 && _textInputEvent.character != '\n')
				{
					io.AddInputCharacter(_textInputEvent.character);
				}
			}
		}

		private static void UpdateMouse(ImGuiIOPtr io)
		{
			io.MousePos = Utils.ScreenToImGui(Input.mousePosition);

			io.MouseWheel = Input.mouseScrollDelta.y;
			io.MouseWheelH = Input.mouseScrollDelta.x;

			io.MouseDown[0] = Input.GetMouseButton(0);
			io.MouseDown[1] = Input.GetMouseButton(1);
			io.MouseDown[2] = Input.GetMouseButton(2);
		}
	}
}
