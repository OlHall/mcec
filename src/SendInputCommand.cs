﻿//-------------------------------------------------------------------
// Copyright © 2017 Kindel Systems, LLC
// http://www.kindel.com
// charlie@kindel.com
// 
// Published under the MIT License.
// Source control on SourceForge 
//    http://sourceforge.net/projects/mcecontroller/
//-------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using WindowsInput;
using WindowsInput.Native;

namespace MCEControl {
    /// <summary>
    /// Simulates a keystroke including shift, ctrl, alt, and windows key 
    /// modifiers.
    /// </summary>
    [Serializable]
    public class SendInputCommand : Command {
        private bool alt;
        private bool ctrl;
        private bool shift;
        private bool win;
        private string vk;

        [XmlAttribute("Alt")] public bool Alt { get => alt; set => alt = value; }
        [XmlAttribute("Ctrl")] public bool Ctrl { get => ctrl; set => ctrl = value; }
        [XmlAttribute("Shift")] public bool Shift { get => shift; set => shift = value; }
        [XmlAttribute("Win")] public bool Win { get => win; set => win = value; }
        [XmlAttribute("vk")] public string Vk { get => vk; set => vk = value; }

        public SendInputCommand() {

        }

        public static List<SendInputCommand> Commands = new List<SendInputCommand>();

        static SendInputCommand() {

            Commands.Add(new SendInputCommand($"shiftdown:", false, false, false, false));
            Commands.Add(new SendInputCommand($"shiftup:", false, false, false, false));

            // Populate default VK_ codes
            foreach (VirtualKeyCode vk in Enum.GetValues(typeof(VirtualKeyCode))) {
                string s;
                if (vk > VirtualKeyCode.HELP && vk < VirtualKeyCode.LWIN)
                    s = vk.ToString();  // already have VK_
                else
                    s = "VK_" + vk.ToString();

                Commands.Add(new SendInputCommand(s, false, false, false, false));
            }
        }
        public SendInputCommand(string vk, bool shift, bool ctrl, bool alt) {
            Key = Vk = vk;
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
            Win = false;
        }

        public SendInputCommand(string vk, bool shift, bool ctrl, bool alt, bool win) {
            Key = Vk = vk;
            Shift = shift;
            Ctrl = ctrl;
            Alt = alt;
            Win = win;
        }

        public override string ToString() {
            return $"Cmd=\"{Key}\" Vk=\"{Vk}\" Shift=\"{Shift}\" Ctrl=\"{Ctrl}\" Alt=\"{Alt}\" Win=\"{Win}\"";
        }

        public override void Execute(string args, Reply reply) {
            
            // Forms:
            // Vk = "VK_..." - Simulates keypress of VK_...
            // Vk = "0X_..." - Simulates keypress of keycode 0X..."
            // Vk = "<char>" - Simulates keypress of keycode for <char>

            try {
                VirtualKeyCode vkcode;
                if (!Vk.StartsWith("VK_", StringComparison.InvariantCultureIgnoreCase) ||
                    (!Enum.TryParse(Vk.ToUpperInvariant(), true, out vkcode) &&
                     !Enum.TryParse(Vk.ToUpperInvariant().Substring(3), true, out vkcode))) {
                    // Not a VK_ string
                    // Hex?
                    ushort num;
                    if ((!Vk.StartsWith("0X", StringComparison.InvariantCultureIgnoreCase) ||
                         !ushort.TryParse(Vk.Substring(2), NumberStyles.HexNumber,
                                          CultureInfo.InvariantCulture.NumberFormat, out num)) &&
                         !ushort.TryParse(Vk, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat,
                                         out num)) {
                        // Not hex
                        switch (Key.ToUpperInvariant()) {
                            case "SHIFTDOWN:":
                                // Modifyer key down
                                SendInputCommand.ShiftKey(args, true);
                                return;
                                break;

                            case "SHIFTUP:":
                                // Modifyer key down
                                SendInputCommand.ShiftKey(args, false);
                                return;
                                break;

                            default:
                                if (string.IsNullOrEmpty(Vk)) {
                                    // bad format. barf.
                                    Logger.Instance.Log4.Info($"Cmd: Invalid VK: {ToString()}");
                                    return;
                                }
                                num = Vk.ToUpperInvariant().ToCharArray()[0];
                                break;
                        }
                    }
                    vkcode = (VirtualKeyCode)num;
                }

                string s;
                if (vkcode > VirtualKeyCode.HELP && vkcode < VirtualKeyCode.LWIN)
                    s = $"{Char.ToUpper((char)vkcode, CultureInfo.InvariantCulture)}";
                else
                    s = "VK_" + vkcode.ToString();
                if (Alt) s = "Alt-" + s;
                if (Ctrl) s = "Ctrl-" + s;
                if (Shift) s = "Shift-" + s;
                if (Win) s = "Win-" + s;

                Logger.Instance.Log4.Info($"Cmd: Sending VK: '{ToString()}' ({s}) (0x{(ushort)vkcode:x2})");

                var sim = new KeyboardSimulator();

                if (Shift) {
                    sim.KeyDown(VirtualKeyCode.SHIFT);
                }
                if (Ctrl) {
                    sim.KeyDown(VirtualKeyCode.CONTROL);
                }
                if (Alt) {
                    sim.KeyDown(VirtualKeyCode.MENU);
                }
                if (Win) {
                    sim.KeyDown(VirtualKeyCode.LWIN);
                }

                sim.KeyPress(vkcode);

                // Key up shift, ctrl, and/or alt
                if (Shift) {
                    sim.KeyUp(VirtualKeyCode.SHIFT);
                }
                if (Ctrl) {
                    sim.KeyUp(VirtualKeyCode.CONTROL);
                }
                if (Alt) {
                    sim.KeyUp(VirtualKeyCode.MENU);
                }
                if (Win) {
                    sim.KeyUp(VirtualKeyCode.LWIN);
                }
            }
            catch (Exception e) {
                Logger.Instance.Log4.Info("Cmd: SendInput failed:" + e.Message);
            }
        }

        public static void ShiftKey(String key, Boolean down) {
            Logger.Instance.Log4.Info($"Cmd: {key} {(down ? "down" : "up")}");

            var sim = new InputSimulator();
            switch (key) {
                case "shift":
                    if (down) sim.Keyboard.KeyDown(VirtualKeyCode.SHIFT);
                    else sim.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
                    break;

                case "ctrl":
                    if (down) sim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                    else sim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                    break;

                case "alt":
                    if (down) sim.Keyboard.KeyDown(VirtualKeyCode.MENU);
                    else sim.Keyboard.KeyUp(VirtualKeyCode.MENU);
                    break;

                case "lwin":
                    if (down) sim.Keyboard.KeyDown(VirtualKeyCode.LWIN);
                    else sim.Keyboard.KeyUp(VirtualKeyCode.LWIN);
                    break;

                case "rwin":
                    if (down) sim.Keyboard.KeyDown(VirtualKeyCode.RWIN);
                    else sim.Keyboard.KeyUp(VirtualKeyCode.RWIN);
                    break;
            }
        }
    }
}
