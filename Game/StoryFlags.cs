using System;
using System.Runtime.InteropServices;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct StoryFlags
        {
            [FieldOffset(0x0)] private IntPtr SaveDataRTTI = IntPtr.Zero;
            [FieldOffset(0x8)] private bool IsValid = false;
            [FieldOffset(0x86 + 0x50)] private readonly byte _86 = 0;
            [FieldOffset(0x88 + 0x50)] private readonly byte _88 = 0; //Quick Cyloop
            [FieldOffset(0x89 + 0x50)] private readonly byte _89 = 0;
            [FieldOffset(0x8B + 0x50)] private readonly byte _8B = 0; //Recovery Smash
            [FieldOffset(0x8C + 0x50)] private readonly byte _8C = 0;
            [FieldOffset(0x8D + 0x50)] private readonly byte _8D = 0; //Sonic Boom
            [FieldOffset(0x8E + 0x50)] private readonly byte _8E = 0; //Wild Rush
            [FieldOffset(0x8F + 0x50)] private readonly byte _8F = 0; //Loop Kick
            [FieldOffset(0x90 + 0x50)] private readonly byte _90 = 0; //Spin Slash
            [FieldOffset(0x91 + 0x50)] private readonly byte _91 = 0;
            [FieldOffset(0x92 + 0x50)] private readonly byte _92 = 0; //Homing Shot
            [FieldOffset(0x94 + 0x50)] private readonly byte _94 = 0; //Auto Combo
            [FieldOffset(0xA45 + 0x50)] private readonly byte _A45 = 0;
            [FieldOffset(0xA48 + 0x50)] private readonly byte _A48 = 0;
            [FieldOffset(0xA49 + 0x50)] private readonly byte _A49 = 0;
            [FieldOffset(0xA4A + 0x50)] private readonly byte _A4A = 0;
            [FieldOffset(0xA5B + 0x50)] private readonly byte _A5B = 0;
            [FieldOffset(0xA85 + 0x50)] private readonly byte _A85 = 0;
            [FieldOffset(0x10C1 + 0x50)] private readonly byte _10C1 = 0;
            [FieldOffset(0x1201 + 0x50)] private readonly byte _1201 = 0;
            [FieldOffset(0x1783 + 0x50)] private readonly byte _1783 = 0;
            [FieldOffset(0x1784 + 0x50)] private readonly byte _1784 = 0;
            [FieldOffset(0x1789 + 0x50)] private readonly byte _1789 = 0;
            [FieldOffset(0x2CC0 + 0x50)] private readonly byte _2CC0 = 0;
            [FieldOffset(0x2CC1 + 0x50)] private readonly byte _2CC1 = 0;
            // [FieldOffset(0x339B + 0x50)] private readonly byte _339B = 0;
            [FieldOffset(0x339C + 0x50)] private readonly byte _339C = 0;
            [FieldOffset(0x339D + 0x50)] private readonly byte _339D = 0;
            [FieldOffset(0x33A3 + 0x50)] private readonly byte _33A3 = 0;
            [FieldOffset(0x48C0 + 0x50)] private readonly byte _48C0 = 0;
            [FieldOffset(0x48C1 + 0x50)] private readonly byte _48C1 = 0;
            [FieldOffset(0x4FB1 + 0x50)] private readonly byte _4FB1 = 0;
            [FieldOffset(0x4FB3 + 0x50)] private readonly byte _4FB3 = 0;
            [FieldOffset(0x4FBA + 0x50)] private readonly byte _4FBA = 0;
            [FieldOffset(0x80C0 + 0x50)] private readonly byte _80C0 = 0;
            [FieldOffset(0x80C1 + 0x50)] private readonly byte _80C1 = 0;

            public StoryFlags() {}

            // Validator
            public void ValidateRTTI(IntPtr RTTIValue)
            {
                IsValid = SaveDataRTTI == RTTIValue;
            }

            //Skills
            public bool Skill_Cyloop => IsValid && _86.BitCheck(0);
            public bool Skill_QuickCyloop => IsValid && _88.BitCheck(0);
            public bool Skill_PhantomRush => IsValid && _89.BitCheck(0);
            public bool Skill_RecoverySmash => IsValid && _8B.BitCheck(0);
            public bool Skill_AirTrick => IsValid && _8C.BitCheck(0);
            public bool Skill_SonicBoom => IsValid && _8D.BitCheck(0);
            public bool Skill_WildRush => IsValid && _8E.BitCheck(0);
            public bool Skill_LoopKick => IsValid && _8F.BitCheck(0);
            public bool Skill_SpinSlash => IsValid && _90.BitCheck(0);
            public bool Skill_StompAttack => IsValid && _91.BitCheck(0);
            public bool Skill_HomingShot => IsValid && _92.BitCheck(0);
            public bool Skill_AutoCombo => IsValid && _94.BitCheck(0);

            // Kronos
            public bool Kronos_Ninja => IsValid && _1201.BitCheck(2);
            public bool Kronos_Door => IsValid && _1201.BitCheck(0);
            public bool Kronos_Amy => IsValid && _1784.BitCheck(2);
            public bool Kronos_GigantosFirst => IsValid && _1789.BitCheck(7);
            public bool Kronos_GreenCE => IsValid && _1783.BitCheck(3);
            public bool Kronos_CyanCE => IsValid && _1783.BitCheck(4);
            public bool Kronos_Tombstones => IsValid && _A48.BitCheck(3);
            public bool Kronos_BlueCE => IsValid && _10C1.BitCheck(0);
            public bool Kronos_RedCE => IsValid && _10C1.BitCheck(2);
            public bool Kronos_YellowCE => IsValid && _10C1.BitCheck(1);
            public bool Kronos_WhiteCE => IsValid && _10C1.BitCheck(3);

            // Ares
            public bool Ares_Knuckles => IsValid && _339D.BitCheck(2);
            public bool Ares_WyvernFirst => IsValid && _33A3.BitCheck(3);
            public bool Ares_Water => IsValid && _A4A.BitCheck(3);
            // public bool Ares_KocoRoundup => _339B.BitCheck(4);
            public bool Ares_Crane => IsValid && _A4A.BitCheck(6);
            public bool Ares_GreenCE => IsValid && _339C.BitCheck(0);
            public bool Ares_CyanCE => IsValid && _339C.BitCheck(1);
            public bool Ares_BlueCE => IsValid && _2CC0.BitCheck(7);
            public bool Ares_RedCE => IsValid && _2CC1.BitCheck(0);
            public bool Ares_YellowCE => IsValid && _2CC1.BitCheck(1);
            public bool Ares_WhiteCE => IsValid && _2CC1.BitCheck(2);
            // Chaos
            public bool Chaos_Tails => IsValid && _4FB1.BitCheck(7);
            public bool Chaos_KnightFirst => IsValid && _4FBA.BitCheck(0);
            public bool Chaos_Hacking => IsValid && _A49.BitCheck(3);
            public bool Chaos_GreenCE => IsValid && _4FB3.BitCheck(6);
            public bool Chaos_CyanCE => IsValid && _4FB3.BitCheck(7);
            public bool Chaos_PinballStart => IsValid && _4FB3.BitCheck(3);
            public bool Chaos_PinballEnd => IsValid && _A45.BitCheck(2);
            public bool Chaos_BlueCE => IsValid && _48C0.BitCheck(7);
            public bool Chaos_RedCE => IsValid && _48C1.BitCheck(0);
            public bool Chaos_YellowCE => IsValid && _48C1.BitCheck(1);
            public bool Chaos_WhiteCE => IsValid && _48C1.BitCheck(2);
            public bool Chaos_KnightStart => IsValid && _A85.BitCheck(0);

            // Rhea
            public bool Rhea_Tower1 => IsValid && _A5B.BitCheck(5);
            public bool Rhea_Tower2 => IsValid && _A5B.BitCheck(4);
            public bool Rhea_Tower3 => IsValid && _A5B.BitCheck(3);
            public bool Rhea_Tower4 => IsValid && _A5B.BitCheck(2);
            public bool Rhea_Tower5 => IsValid && _A5B.BitCheck(1);
            public bool Rhea_Tower6 => IsValid && _A5B.BitCheck(0);

            // Ouranos
            public bool Ouranos_Bridge => IsValid && _A48.BitCheck(6);
            public bool Ouranos_BlueCE => IsValid && _80C0.BitCheck(7);
            public bool Ouranos_RedCE => IsValid && _80C1.BitCheck(0);
            public bool Ouranos_GreenCE => IsValid && _80C1.BitCheck(1);
            public bool Ouranos_YellowCE => IsValid && _80C1.BitCheck(2);
            public bool Ouranos_CyanCE => IsValid && _80C1.BitCheck(3);
            public bool Ouranos_WhiteCE => IsValid && _80C1.BitCheck(4);
        }
    }
}
