using CompiladorAssembly.Modelos;

static void Main(string[] args) {
    string[] lines = File.ReadAllLines("codigo.asm");
    List<Instruction> instructions = lines.Select(l => new Instruction(l)).ToList();

    var cpu = new CPU(clockCycleMs: 500);

    int tempoTotal = 0;
    int tipoR = 0, tipoI = 0, tipoJ = 0;

    foreach (var inst in instructions) {
        Console.Clear();
        Console.WriteLine($"CICLO CLOCK: {tempoTotal / cpu.ClockCycleMs + 1}");
        Console.WriteLine($"Instrução: {inst.OpCode} {string.Join(", ", inst.Operands)}");
        Console.WriteLine($"PC: {cpu.PC}");
        Console.WriteLine($"Binário: {inst.ToBinary()}");
        Console.WriteLine($"Hexadecimal: {inst.ToHexadecimal()}");

        cpu.Execute(inst);
        cpu.ShowRegisters();
        cpu.ShowMemory();

        tempoTotal += cpu.ClockCycleMs;

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
    }

    Console.WriteLine($"\nTempo Total de Execução: {tempoTotal / 1000.0} segundos");
    Console.WriteLine($"Instruções Tipo R: {tipoR}, Tipo I: {tipoI}, Tipo J: {tipoJ}");
}

