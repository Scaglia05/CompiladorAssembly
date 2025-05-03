using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAssembly.Modelos {
    public class Instruction {

        public string OpCode;
        public string[] Operands;

        public Instruction(string line) {
            var parts = line.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) {
                OpCode = parts[0].ToLower();
                Operands = parts.Skip(1).ToArray();
            }
        }

        public string ToBinary() => Convert.ToString(GetHashCode(), 2).PadLeft(32, '0');
        public string ToHexadecimal() => "0x" + GetHashCode().ToString("X");
    }
}
