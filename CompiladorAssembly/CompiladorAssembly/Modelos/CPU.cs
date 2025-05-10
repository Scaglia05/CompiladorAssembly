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

            // Registrador constante
            Registers.Add("$zero", 0);

            // Registradores temporários: $t0 a $t9
            for (int i = 0; i <= 9; i++)
                Registers.Add($"$t{i}", 0);

            // Registradores salvos: $s0 a $s7
            for (int i = 0; i <= 7; i++)
                Registers.Add($"$s{i}", 0);

            // Registradores de argumentos: $a0 a $a3
            for (int i = 0; i <= 3; i++)
                Registers.Add($"$a{i}", 0);

            // Registradores de retorno: $v0 a $v1
            for (int i = 0; i <= 1; i++)
                Registers.Add($"$v{i}", 0);
        }

        public void Execute(Instruction inst) {
            string op = inst.OpCode;
            string[] args = inst.Operands;

            int GetReg(string name) => Registers.TryGetValue(name, out int val) ? val : 0;
            void SetReg(string name, int val) { if (name != "$zero") Registers[name] = val; }

            switch (op) {
                case "add":
                    SetReg(args[0], GetReg(args[1]) + GetReg(args[2]));
                    break;
                case "sub":
                    SetReg(args[0], GetReg(args[1]) - GetReg(args[2]));
                    break;
                case "and":
                    SetReg(args[0], GetReg(args[1]) & GetReg(args[2]));
                    break;
                case "or":
                    SetReg(args[0], GetReg(args[1]) | GetReg(args[2]));
                    break;
                case "slt":
                    SetReg(args[0], GetReg(args[1]) < GetReg(args[2]) ? 1 : 0);
                    break;
                case "addi":
                    SetReg(args[0], GetReg(args[1]) + ParseImmediate(args[2]));
                    break;
                case "lw":
                    var lwAddr = CalculateMemoryAddress(args[1], GetReg);
                    SetReg(args[0], Memory[lwAddr]);
                    break;
                case "sw":
                    var swAddr = CalculateMemoryAddress(args[1], GetReg);
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
                default:
                    throw new InvalidOperationException($"Instrução desconhecida: {op}");
            }
        }

        // Método auxiliar para tratar imediatos
        private int ParseImmediate(string immediate) {
            if (!int.TryParse(immediate, out int result)) {
                throw new ArgumentException($"Imediato inválido: {immediate}");
            }
            return result;
        }

        // Método auxiliar para calcular endereço de memória para instruções lw e sw
        private int CalculateMemoryAddress(string address, Func<string, int> GetReg) {
            var match = Regex.Match(address, @"(\d+)\((\$[a-z0-9]+)\)");
            if (!match.Success) {
                throw new ArgumentException($"Endereço de memória inválido: {address}");
            }
            int offset = int.Parse(match.Groups[1].Value);
            string register = match.Groups[2].Value;
            return GetReg(register) + offset;
        }

        public void ShowRegisters() {
            Console.WriteLine("\n--- REGISTRADORES ---");
            Console.WriteLine("| Nome  | Decimal           | Hexadecimal   |");
            Console.WriteLine("|-------|-------------------|---------------|");

            foreach (var reg in Registers.OrderBy(r => r.Key)) {
                Console.WriteLine($"| {reg.Key.PadRight(5)} | {reg.Value.ToString().PadLeft(17)} | 0x{reg.Value:X8} |");
            }
        }

        public void ShowMemory() {
            Console.WriteLine("\n--- MEMÓRIA (endereços com valor) ---");
            Console.WriteLine("| Endereço | Valor (Dec) | Valor (Hex) |");
            Console.WriteLine("|----------|-------------|--------------|");

            for (int i = 0; i < Memory.Length; i++) {
                if (Memory[i] != 0) {
                    Console.WriteLine($"| {i.ToString().PadLeft(8)} | {Memory[i].ToString().PadLeft(11)} | 0x{Memory[i]:X8} |");
                }
            }
        }
    }
}
