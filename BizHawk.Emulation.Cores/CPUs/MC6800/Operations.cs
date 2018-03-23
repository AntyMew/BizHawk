﻿using BizHawk.Common.NumberExtensions;
using System;

namespace BizHawk.Emulation.Common.Cores.MC6800
{
	public partial class MC6800
	{
		// NOTE: Reading automatically increments source address
		public void Read_Func(ushort dest, ushort src_l, ushort src_h)
		{
			Regs[dest] = ReadMemory((ushort)(Regs[src_l] | (Regs[src_h]) << 8));
		}

		// speical read for POP P 
		public void Read_Func_P(ushort dest, ushort src_l, ushort src_h)
		{
			Regs[dest] = (ushort)(ReadMemory((ushort)(Regs[src_l] | (Regs[src_h]) << 8)) | 0xC0);
		}

		public void Write_Func(ushort dest_l, ushort dest_h, ushort src)
		{
			WriteMemory((ushort)(Regs[dest_l] | (Regs[dest_h]) << 8), (byte)Regs[src]);
		}

		public void TR_Func(ushort dest, ushort src)
		{
			Regs[dest] = Regs[src];
		}

		public void TR_16_Func(ushort dest_l, ushort dest_h, ushort src_l, ushort src_h)
		{
			Regs[dest_l] = Regs[src_l];
			Regs[dest_h] = Regs[src_h];
		}

		public void NEG_8_Func(ushort src)
		{
			int Reg16_d = 0;
			Reg16_d -= Regs[src];

			FlagC = Regs[src] != 0x0;
			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);
			// redo for half carry flag
			Reg16_d = 0;
			Reg16_d -= (Regs[src] & 0xF);
			FlagH = Reg16_d.Bit(4);
			Regs[src] = ans;
			FlagN = true;
		}

		public void CLR_Func(ushort src)
		{
			Regs[src] = 0;
		}

		public void ADD8_Func(ushort dest, ushort src)
		{
			int Reg16_d = Regs[dest];
			Reg16_d += Regs[src];

			FlagC = Reg16_d.Bit(8);
			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[dest] & 0xF;
			Reg16_d += (Regs[src] & 0xF);

			FlagH = Reg16_d.Bit(4);

			FlagN = false;

			Regs[dest] = ans;
		}

		public void SUB8_Func(ushort dest, ushort src)
		{
			int Reg16_d = Regs[dest];
			Reg16_d -= Regs[src];

			FlagC = Reg16_d.Bit(8);
			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[dest] & 0xF;
			Reg16_d -= (Regs[src] & 0xF);

			FlagH = Reg16_d.Bit(4);
			FlagN = true;

			Regs[dest] = ans;
		}

		public void BIT_Func(ushort bit, ushort src)
		{
			FlagZ = !Regs[src].Bit(bit);
			FlagH = true;
			FlagN = false;
		}

		public void SET_Func(ushort bit, ushort src)
		{
			Regs[src] |= (ushort)(1 << bit);
		}

		public void RES_Func(ushort bit, ushort src)
		{
			Regs[src] &= (ushort)(0xFF - (1 << bit));
		}

		public void ASGN_Func(ushort src, ushort val)
		{
			Regs[src] = val;
		}

		public void BIT8_Func(ushort dest, ushort src)
		{
			
		}

		public void AND8_Func(ushort dest, ushort src)
		{
			Regs[dest] = (ushort)(Regs[dest] & Regs[src]);

			FlagZ = Regs[dest] == 0;
			FlagC = false;
			FlagH = true;
			FlagN = false;
		}

		public void OR8_Func(ushort dest, ushort src)
		{
			Regs[dest] = (ushort)(Regs[dest] | Regs[src]);

			FlagZ = Regs[dest] == 0;
			FlagC = false;
			FlagH = false;
			FlagN = false;
		}

		public void XOR8_Func(ushort dest, ushort src)
		{
			Regs[dest] = (ushort)(Regs[dest] ^ Regs[src]);

			FlagZ = Regs[dest] == 0;
			FlagC = false;
			FlagH = false;
			FlagN = false;
		}

		public void CP8_Func(ushort dest, ushort src)
		{
			int Reg16_d = Regs[dest];
			Reg16_d -= Regs[src];

			FlagC = Reg16_d.Bit(8);
			FlagZ = (Reg16_d & 0xFF) == 0;

			// redo for half carry flag
			Reg16_d = Regs[dest] & 0xF;
			Reg16_d -= (Regs[src] & 0xF);

			FlagH = Reg16_d.Bit(4);

			FlagN = true;
		}

		public void ASL_Func(ushort src)
		{
			FlagC = Regs[src].Bit(7);

			Regs[src] = (ushort)((Regs[src] << 1) & 0xFF);

			FlagZ = (Regs[src] == 0);
			FlagH = false;
			FlagN = false;
		}

		public void ROL_Func(ushort src)
		{
			ushort c = (ushort)(FlagC ? 1 : 0);
			FlagC = Regs[src].Bit(7);

			Regs[src] = (ushort)(((Regs[src] << 1) & 0xFF) | c);

			FlagZ = (Regs[src] == 0);
			FlagH = false;
			FlagN = false;
		}

		public void ASR_Func(ushort src)
		{
			FlagC = Regs[src].Bit(0);

			Regs[src] = (ushort)((Regs[src] & 0x80) | (Regs[src] >> 1));

			FlagZ = (Regs[src] == 0);
			FlagH = false;
			FlagN = false;
		}

		public void ROR_Func(ushort src)
		{
			ushort c = (ushort)(FlagC ? 0x80 : 0);

			FlagC = Regs[src].Bit(0);

			Regs[src] = (ushort)(c | (Regs[src] >> 1));

			FlagZ = (Regs[src] == 0);
			FlagH = false;
			FlagN = false;
		}

		public void TST_Func(ushort src)
		{
			FlagZ = (Regs[src] == 0);
			FlagN = ((Regs[src] & 0x80) == 0x80);
			FlagC = true;
			FlagH = false;
		}

		public void LSR_Func(ushort src)
		{
			FlagC = Regs[src].Bit(0);

			Regs[src] = (ushort)((Regs[src] >> 1) & 0xFF);

			FlagZ = Regs[src] == 0;
			FlagH = false;
			FlagN = false;
		}

		public void CPL_Func(ushort src)
		{
			Regs[src] = (ushort)((~Regs[src]) & 0xFF);

			FlagH = true;
			FlagN = true;
		}

		public void INC8_Func(ushort src)
		{
			int Reg16_d = Regs[src];
			Reg16_d += 1;

			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[src] & 0xF;
			Reg16_d += 1;

			FlagH = Reg16_d.Bit(4);
			FlagN = false;

			Regs[src] = ans;
		}

		public void DEC8_Func(ushort src)
		{
			int Reg16_d = Regs[src];
			Reg16_d -= 1;

			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[src] & 0xF;
			Reg16_d -= 1;

			FlagH = Reg16_d.Bit(4);
			FlagN = true;

			Regs[src] = ans;
		}

		public void INC16_Func(ushort src_l, ushort src_h)
		{
			int Reg16_d = Regs[src_l] | (Regs[src_h] << 8);

			Reg16_d += 1;

			Regs[src_l] = (ushort)(Reg16_d & 0xFF);
			Regs[src_h] = (ushort)((Reg16_d & 0xFF00) >> 8);
		}

		public void DEC16_Func(ushort src_l, ushort src_h)
		{
			int Reg16_d = Regs[src_l] | (Regs[src_h] << 8);

			Reg16_d -= 1;

			Regs[src_l] = (ushort)(Reg16_d & 0xFF);
			Regs[src_h] = (ushort)((Reg16_d & 0xFF00) >> 8);
		}

		public void ADC8_Func(ushort dest, ushort src)
		{
			int Reg16_d = Regs[dest];
			int c = FlagC ? 1 : 0;

			Reg16_d += (Regs[src] + c);

			FlagC = Reg16_d.Bit(8);
			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[dest] & 0xF;
			Reg16_d += ((Regs[src] & 0xF) + c);

			FlagH = Reg16_d.Bit(4);
			FlagN = false;

			Regs[dest] = ans;
		}

		public void SBC8_Func(ushort dest, ushort src)
		{
			int Reg16_d = Regs[dest];
			int c = FlagC ? 1 : 0;

			Reg16_d -= (Regs[src] + c);

			FlagC = Reg16_d.Bit(8);
			FlagZ = (Reg16_d & 0xFF) == 0;

			ushort ans = (ushort)(Reg16_d & 0xFF);

			// redo for half carry flag
			Reg16_d = Regs[dest] & 0xF;
			Reg16_d -= ((Regs[src] & 0xF) + c);

			FlagH = Reg16_d.Bit(4);
			FlagN = true;

			Regs[dest] = ans;
		}

		// DA code courtesy of AWJ: http://forums.nesdev.com/viewtopic.php?f=20&t=15944
		public void DA_Func(ushort src)
		{
			byte a = (byte)Regs[src];

			if (!FlagN)
			{  // after an addition, adjust if (half-)carry occurred or if result is out of bounds
				if (FlagC || a > 0x99) { a += 0x60; FlagC = true; }
				if (FlagH || (a & 0x0f) > 0x09) { a += 0x6; }
			}
			else
			{  // after a subtraction, only adjust if (half-)carry occurred
				if (FlagC) { a -= 0x60; }
				if (FlagH) { a -= 0x6; }
			}

			a &= 0xFF;

			Regs[src] = a;

			FlagZ = a == 0; 
			FlagH = false;
		}

		// used for signed operations
		public void ADDS_Func(ushort dest_l, ushort dest_h, ushort src)
		{
			int Reg16_d = Regs[dest_l];
			int Reg16_s = Regs[src];

			Reg16_d += Reg16_s;

			ushort temp = 0;

			// since this is signed addition, calculate the high byte carry appropriately
			if (Reg16_s.Bit(7))
			{
				if (((Reg16_d & 0xFF) >= Regs[dest_l]))
				{
					temp = 0xFF;
				}
				else
				{
					temp = 0;
				}
			}
			else
			{
				temp = (ushort)(Reg16_d.Bit(8) ? 1 : 0);
			}

			ushort ans_l = (ushort)(Reg16_d & 0xFF);

			// JR operations do not effect flags
			if (dest_l != PCl)
			{
				FlagC = Reg16_d.Bit(8);

				// redo for half carry flag
				Reg16_d = Regs[dest_l] & 0xF;
				Reg16_d += Regs[src] & 0xF;

				FlagH = Reg16_d.Bit(4);
				FlagN = false;
				FlagZ = false; 			
			}

			Regs[dest_l] = ans_l;
			Regs[dest_h] += temp;
			Regs[dest_h] &= 0xFF;

		}

		public void CP16_Func(ushort dest_l, ushort dest_h, ushort src_l, ushort src_h)
		{
			int Reg16_d = Regs[dest_l] | (Regs[dest_h] << 8);
			int Reg16_s = Regs[src_l] | (Regs[src_h] << 8);

			Reg16_d += Reg16_s;

			FlagC = Reg16_d.Bit(16);

			ushort ans_l = (ushort)(Reg16_d & 0xFF);
			ushort ans_h = (ushort)((Reg16_d & 0xFF00) >> 8);

			// redo for half carry flag
			Reg16_d = Regs[dest_l] | ((Regs[dest_h] & 0x0F) << 8);
			Reg16_s = Regs[src_l] | ((Regs[src_h] & 0x0F) << 8);

			Reg16_d += Reg16_s;

			FlagH = Reg16_d.Bit(12);
			FlagN = false;

			Regs[dest_l] = ans_l;
			Regs[dest_h] = ans_h;
		}
	}
}
