using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorAssembly.Modelos {
    public class Instruction {
        public string OpCode { get; set; }
        public string[] Operands { get; set; }

        public Instruction(string line) {
            // Ignorar linhas vazias ou com apenas espaços
            if (string.IsNullOrWhiteSpace(line)) {
                return; // Ignorar a linha sem lançar exceção
            }

            // Ignorar linhas de comentário (caso seu código tenha comentários, como # ou //)
            if (line.StartsWith("#") || line.StartsWith("//")) {
                return; // Ignorar a linha sem lançar exceção
            }

            // Dividir a linha em partes, considerando espaços e vírgulas
            var parts = line.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Verificar se a linha tem pelo menos um OpCode
            if (parts.Length < 1) {
                throw new ArgumentException("Instrução malformada: Nenhum código de operação encontrado.");
            }

            OpCode = parts[0].ToLower();  // Garantir que o OpCode esteja em minúsculas
            Operands = parts.Skip(1).ToArray();

            // Se não houver operandos, inicializar como uma lista vazia
            if (Operands == null || Operands.Length == 0) {
                Operands = new string[0];
            }

            // Verificar se algum operando está vazio
            if (Operands.Any(o => string.IsNullOrWhiteSpace(o))) {
                throw new ArgumentException($"Instrução malformada: Encontrado operando vazio em {OpCode}.");
            }

            // Validar a quantidade de operandos com base no OpCode (exemplo simplificado)
            ValidateOperands(OpCode, Operands);
        }

        // Método para validar a quantidade de operandos com base no OpCode (pode ser expandido conforme necessário)
        private void ValidateOperands(string opCode, string[] operands) {
            switch (opCode) {
                case "add":
                case "sub":
                case "and":
                case "or":
                case "slt":
                    // Exemplo: operações tipo R geralmente têm 3 operandos
                    if (operands.Length != 3) {
                        throw new ArgumentException($"Instrução {opCode} malformada: Esperado 3 operandos.");
                    }
                    break;

                case "lw":
                case "sw":
                    // Exemplo: instruções tipo I geralmente têm 2 operandos
                    if (operands.Length != 2) {
                        throw new ArgumentException($"Instrução {opCode} malformada: Esperado 2 operandos.");
                    }
                    break;

                case "j":
                    // Exemplo: instrução tipo J geralmente tem 1 operando
                    if (operands.Length != 1) {
                        throw new ArgumentException($"Instrução {opCode} malformada: Esperado 1 operando.");
                    }
                    break;

                default:
                    // Para outros tipos de instrução, se necessário
                    break;
            }
        }

        // Método ToBinary (ajustado para representar os dados da instrução, não o hash do objeto)
        public string ToBinary() {
            var operandosBinarios = Operands.Select(o => string.Join("", o.Select(c => Convert.ToString(c, 2).PadLeft(8, '0')))).ToArray();
            return string.Join("", operandosBinarios);
        }

        // Método ToHexadecimal (ajustado para representar os dados da instrução, não o hash do objeto)
        public string ToHexadecimal() {
            var operandosHexadecimais = Operands.Select(o => string.Join("", o.Select(c => Convert.ToInt32(c).ToString("X")))).ToArray();
            return "0x" + string.Join("", operandosHexadecimais);
        }
    }
}
