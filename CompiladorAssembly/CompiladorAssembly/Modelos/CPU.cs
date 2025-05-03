using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompiladorAssembly.Modelos {
    public class CPU {

        public Dictionary<string, int> Registers = new();
        public int[] Memory = new int[1024];
        public int PC = 0;
        public int ClockCycleMs;

        public CPU(int clockCycleMs) {
            ClockCycleMs = clockCycleMs;
            for (int i = 0; i <= 31; i++)
                Registers.Add("$t" + (i % 10), 0);
            Registers.Add("$zero", 0);
        }

        public void Execute(Instruction inst) {
            string op = inst.OpCode;
            string[] args = inst.Operands;

            int GetReg(string name) => Registers.TryGetValue(name, out int val) ? val : 0;
            void SetReg(string name, int val) { if (name != "$zero") Registers[name] = val; }

            switch (op) {
                case "add": SetReg(args[0], GetReg(args[1]) + GetReg(args[2])); break;
                case "sub": SetReg(args[0], GetReg(args[1]) - GetReg(args[2])); break;
                case "and": SetReg(args[0], GetReg(args[1]) & GetReg(args[2])); break;
                case "or": SetReg(args[0], GetReg(args[1]) | GetReg(args[2])); break;
                case "slt": SetReg(args[0], GetReg(args[1]) < GetReg(args[2]) ? 1 : 0); break;
                case "addi": SetReg(args[0], GetReg(args[1]) + int.Parse(args[2])); break;
                case "lw":
                    var lwParts = Regex.Match(args[1], @"(\d+)\((\$[a-z0-9]+)\)");
                    int lwAddr = GetReg(lwParts.Groups[2].Value) + int.Parse(lwParts.Groups[1].Value);
                    SetReg(args[0], Memory[lwAddr]);
                    break;
                case "sw":
                    var swParts = Regex.Match(args[1], @"(\d+)\((\$[a-z0-9]+)\)");
                    int swAddr = GetReg(swParts.Groups[2].Value) + int.Parse(swParts.Groups[1].Value);
                    Memory[swAddr] = GetReg(args[0]);
                    break;
                case "beq":
                    if (GetReg(args[0]) == GetReg(args[1])) PC = int.Parse(args[2]) - 4;
                    break;
                case "bne":
                    if (GetReg(args[0]) != GetReg(args[1])) PC = int.Parse(args[2]) - 4;
                    break;
                case "j":
                    PC = int.Parse(args[0]) - 4;
                    break;
            }
        }

        public void ShowRegisters() {
            Console.WriteLine(">> REGISTRADORES:");
            foreach (var reg in Registers)
                Console.WriteLine($"{reg.Key}: {reg.Value}");
        }

        public void ShowMemory() {
            Console.WriteLine(">> MEMÓRIA:");
            for (int i = 0; i < 10; i++)
                Console.WriteLine($"[{i}] = {Memory[i]}");
        }
    }

}

