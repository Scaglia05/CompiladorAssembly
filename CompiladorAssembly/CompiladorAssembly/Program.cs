using CompiladorAssembly.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

Console.WriteLine("==== Simulador MIPS ====");

// Entrada do caminho do arquivo
Console.Write("Por favor, informe o caminho completo do arquivo .asm: ");
string caminhoArquivo = Console.ReadLine()?.Trim();

while (string.IsNullOrEmpty(caminhoArquivo) || !File.Exists(caminhoArquivo)) {
    Console.WriteLine("Caminho inválido ou arquivo não encontrado. Tente novamente.");
    Console.Write("Informe o caminho do arquivo .asm: ");
    caminhoArquivo = Console.ReadLine()?.Trim();
}

// Entrada do clock da CPU
Console.Write("Informe o clock da CPU (em milissegundos): ");
int clockMs;
while (!int.TryParse(Console.ReadLine(), out clockMs) || clockMs <= 0) {
    Console.Write("Valor inválido. Informe um número positivo para o clock (em ms): ");
}

// Lê todas as linhas do arquivo
string[] lines = File.ReadAllLines(caminhoArquivo);

// Tenta criar as instruções a partir das linhas do arquivo, trata instruções inválidas
List<Instruction> instructions = new List<Instruction>();
foreach (var line in lines) {
    try {
        var instruction = new Instruction(line);
        if (instruction != null) {
            instructions.Add(instruction);
        }
    } catch (ArgumentException ex) {
        Console.WriteLine($"Aviso: Linha malformada ignorada - {ex.Message}");
    }
}

// Instancia a CPU com o clock informado
var cpu = new CPU(clockMs);
int tempoTotal = 0;
int tipoR = 0, tipoI = 0, tipoJ = 0;

foreach (var inst in instructions) {
    string operandos = (inst.Operands != null && inst.Operands.Length > 0) ? string.Join(", ", inst.Operands) : "Nenhum operando";

    try {
        Console.Clear();
        Console.WriteLine($"CICLO CLOCK: {tempoTotal / cpu.ClockCycleMs + 1}");
        Console.WriteLine($"Instrução: {inst.OpCode} {operandos}");
        Console.WriteLine($"PC: {cpu.PC}");
        Console.WriteLine($"Binário: {inst.ToBinary()}");
        Console.WriteLine($"Hexadecimal: {inst.ToHexadecimal()}");

        cpu.Execute(inst);
        cpu.ShowRegisters();
        cpu.ShowMemory();

        tempoTotal += cpu.ClockCycleMs;

        // Classificação da instrução
        switch (inst.OpCode) {
            case "add":
            case "sub":
            case "and":
            case "or":
            case "slt":
                tipoR++; break;

            case "lw":
            case "sw":
            case "beq":
            case "bne":
            case "addi":
                tipoI++; break;

            case "j":
                tipoJ++; break;
        }

        cpu.PC += 4;
        Thread.Sleep(cpu.ClockCycleMs);
    } catch (Exception ex) {
        Console.WriteLine($"Erro ao executar a instrução {inst.OpCode}: {ex.Message}");
        continue;
    }
}

// Exibição de estatísticas
Console.WriteLine($"\nTempo Total de Execução: {tempoTotal / 1000.0} segundos");
Console.WriteLine($"Instruções Tipo R: {tipoR}, Tipo I: {tipoI}, Tipo J: {tipoJ}");
